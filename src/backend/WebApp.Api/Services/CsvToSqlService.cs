using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

/// <summary>
/// Утилита "CSV → SQL INSERT" ★★☆
/// Преобразует CSV-данные в SQL-скрипт: CREATE TEMP TABLE + INSERT.
///
/// Формат входа:
///   Строка 1: имя таблицы
///   Строка 2: заголовки колонок (через запятую)
///   Строки 3+: данные (по одной записи на строку)
///
/// Типы колонок определяются автоматически: INT / DOUBLE PRECISION / TEXT.
/// </summary>
public class CsvToSqlService : IUtilityService
{
    public string Endpoint => "csv-to-sql";

    public string Execute(string input)
    {
        // --- Валидация ---
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Первая строка — имя таблицы, вторая — заголовки, далее данные.";

        var lines = input.Split('\n', StringSplitOptions.None)
                         .Select(l => l.TrimEnd('\r'))
                         .ToList();

        // Убираем пустые строки в конце (пользователь мог случайно нажать Enter)
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
            lines.RemoveAt(lines.Count - 1);

        if (lines.Count < 2)
            return "Ошибка: нужно минимум 2 строки (имя таблицы и заголовки колонок).";

        // --- Парсинг ---
        var tableName = lines[0].Trim();
        if (string.IsNullOrWhiteSpace(tableName))
            return "Ошибка: имя таблицы не может быть пустым.";

        // Валидация имени таблицы: только буквы, цифры, подчёркивания
        if (!Regex.IsMatch(tableName, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            return "Ошибка: имя таблицы должно начинаться с буквы или подчёркивания и содержать только буквы, цифры и подчёркивания.";

        var headers = ParseCsvLine(lines[1]);
        if (headers.Count == 0)
            return "Ошибка: не указаны заголовки колонок.";

        var dataLines = lines.Skip(2).ToList();

        // --- Определение типов колонок ---

        // Для каждой колонки смотрим на значения ВО ВСЕХ строках
        // и выбираем самый общий тип, в который помещаются все непустые значения
        var columnTypes = new string[headers.Count];
        for (int col = 0; col < headers.Count; col++)
        {
            columnTypes[col] = InferColumnType(dataLines, col);
        }

        // --- Генерация SQL ---

        var sb = new StringBuilder();

        // CREATE TEMP TABLE
        sb.Append($"CREATE TEMP TABLE {EscapeSqlName(tableName)} (\n  ");
        var columnDefs = new List<string>();
        for (int i = 0; i < headers.Count; i++)
        {
            columnDefs.Add($"{EscapeSqlName(headers[i])} {columnTypes[i]}");
        }
        sb.Append(string.Join(", ", columnDefs));
        sb.Append("\n);");

        // INSERT для каждой строки данных
        foreach (var dataLine in dataLines)
        {
            var values = ParseCsvLine(dataLine);
            if (values.Count == 0)
                continue;

            // Если в строке меньше значений, чем колонок — дополняем NULL
            while (values.Count < headers.Count)
                values.Add(null);

            sb.Append($"\nINSERT INTO {EscapeSqlName(tableName)} VALUES (");
            var formattedValues = new List<string>();
            for (int i = 0; i < headers.Count; i++)
            {
                formattedValues.Add(FormatSqlValue(values[i], columnTypes[i]));
            }
            sb.Append(string.Join(", ", formattedValues));
            sb.Append(");");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Разбирает CSV-строку на отдельные значения.
    /// Поддерживает простые кавычки: "значение, с запятой" не разобьётся.
    /// </summary>
    private static List<string?> ParseCsvLine(string line)
    {
        var result = new List<string?>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                // Экранированная кавычка ""
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++; // пропускаем следующую кавычку
                }
                else
                {
                    inQuotes = !inQuotes; // переключаем режим кавычек
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.Length > 0 ? current.ToString() : null);
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        // Последнее значение
        result.Add(current.Length > 0 ? current.ToString() : null);

        return result;
    }

    /// <summary>
    /// Определяет тип колонки, анализируя все непустые значения.
    /// Логика (от наиболее специфичного к общему):
    ///   1. Если ВСЕ значения парсятся как int (целые числа) → INT
    ///   2. Если ВСЕ значения парсятся как double (дробные) → DOUBLE PRECISION
    ///   3. Иначе → TEXT
    ///
    /// Пустые/null значения игнорируются при проверке.
    /// Если колонка полностью пустая — возвращаем TEXT.
    /// </summary>
    private static string InferColumnType(List<string> dataLines, int colIndex)
    {
        bool allInt = true;
        bool allNumeric = true;
        bool hasNonNullValue = false;

        foreach (var line in dataLines)
        {
            var values = ParseCsvLine(line);
            if (colIndex >= values.Count)
                continue;

            var val = values[colIndex];
            if (string.IsNullOrEmpty(val))
                continue;

            hasNonNullValue = true;

            // Нормализуем десятичный разделитель: запятая → точка.
            // Это нужно, чтобы CSV с русским форматом чисел (10,5) тоже распознавался как DOUBLE PRECISION.
            var normalized = val.Trim().Replace(',', '.');

            // Проверяем, является ли значение целым числом
            if (!long.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                allInt = false;

            // Проверяем, является ли значение числом (целым или дробным)
            if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                allNumeric = false;
        }

        if (!hasNonNullValue)
            return "TEXT";

        if (allInt)
            return "INT";

        if (allNumeric)
            return "DOUBLE PRECISION";

        return "TEXT";
    }

    /// <summary>
    /// Форматирует значение для SQL-вставки:
    ///   - INT/DOUBLE → как есть (без кавычек)
    ///   - TEXT → в одинарных кавычках с экранированием '
    ///   - null → NULL
    /// </summary>
    private static string FormatSqlValue(string? value, string type)
    {
        if (value is null)
            return "NULL";

        var trimmed = value.Trim();

        if (trimmed.Length == 0)
            return "NULL";

        if (type == "INT" || type == "DOUBLE PRECISION")
        {
            // Числа вставляем как есть, заменяя запятую на точку
            return trimmed.Replace(',', '.');
        }

        // TEXT — экранируем одинарные кавычки и оборачиваем в кавычки
        return $"'{EscapeSqlString(trimmed)}'";
    }

    /// <summary>
    /// Экранирует одинарную кавычку для SQL ( ' → '' ).
    /// </summary>
    private static string EscapeSqlString(string value)
    {
        return value.Replace("'", "''");
    }

    /// <summary>
    /// Экранирует идентификатор (имя таблицы/колонки) для SQL.
    /// </summary>
    private static string EscapeSqlName(string? name)
    {
        if (name is null)
            return "NULL";

        // Для простоты используем двойные кавычки (SQL standard)
        return $"\"{name.Replace("\"", "\"\"")}\"";
    }
}
