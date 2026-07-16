using System.Text;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

/// <summary>
/// Конвертер регистров ★★☆
/// Преобразует текст в один из регистров: UPPER, lower, Title, camelCase, PascalCase, snake_case, kebab-case.
/// Endpoint: case-converter
/// </summary>
public class CaseConverterService : IUtilityService
{
    public string Endpoint => "case-converter";

    /// <summary>Извлекает слова (последовательности букв Unicode) из текста.</summary>
    private static readonly Regex WordRegex = new(@"\p{L}+", RegexOptions.Compiled);

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Первая строка — режим, далее — текст.";

        var lines = input.Split('\n', StringSplitOptions.None)
                         .Select(l => l.TrimEnd('\r'))
                         .ToList();

        if (lines.Count < 2 || string.IsNullOrWhiteSpace(lines[1]))
            return "Ошибка: после режима не указан текст для преобразования.";

        var mode = lines[0].Trim().ToLowerInvariant();
        var text = string.Join("\n", lines.Skip(1));

        // Если пустой текст после режима
        if (string.IsNullOrWhiteSpace(text))
            return "Ошибка: текст для преобразования не может быть пустым.";

        string[] validModes = ["upper", "lower", "title", "camel", "pascal", "snake", "snake_case", "kebab", "kebab-case"];
        if (!validModes.Contains(mode))
            return $"Ошибка: неизвестный режим «{mode}». Допустимые: {string.Join(", ", validModes)}.";

        return mode switch
        {
            "upper" => text.ToUpperInvariant(),
            "lower" => text.ToLowerInvariant(),
            "title" => ToTitleCase(text),
            "camel" => ToCamelCase(text),
            "pascal" => ToPascalCase(text),
            "snake" or "snake_case" => ToSnakeCase(text),
            "kebab" or "kebab-case" => ToKebabCase(text),
            _ => "Ошибка: неизвестный режим."
        };
    }

    /// <summary>Title Case: каждое слово с заглавной, остальные буквы строчные.</summary>
    private static string ToTitleCase(string text)
    {
        var words = ExtractWords(text);
        if (words.Count == 0) return text;

        var result = new StringBuilder();
        int lastIndex = 0;

        foreach (var (word, index) in words)
        {
            // Добавляем разделители (пробелы, знаки препинания) между словами
            if (index > lastIndex)
                result.Append(text.AsSpan(lastIndex, index - lastIndex));

            result.Append(char.ToUpperInvariant(word[0]));
            if (word.Length > 1)
                result.Append(word.Substring(1).ToLowerInvariant());

            lastIndex = index + word.Length;
        }

        // Оставшийся хвост после последнего слова
        if (lastIndex < text.Length)
            result.Append(text.AsSpan(lastIndex));

        return result.ToString();
    }

    /// <summary>camelCase: первое слово со строчной, остальные с заглавной, без разделителей.</summary>
    private static string ToCamelCase(string text)
    {
        var words = ExtractWords(text);
        if (words.Count == 0) return text.ToLowerInvariant();

        var result = new StringBuilder();
        for (int i = 0; i < words.Count; i++)
        {
            var word = words[i].Word.ToLowerInvariant();
            if (i == 0)
            {
                result.Append(word);
            }
            else
            {
                result.Append(char.ToUpperInvariant(word[0]));
                if (word.Length > 1)
                    result.Append(word.Substring(1));
            }
        }
        return result.ToString();
    }

    /// <summary>PascalCase: каждое слово с заглавной, без разделителей.</summary>
    private static string ToPascalCase(string text)
    {
        var words = ExtractWords(text);
        if (words.Count == 0) return text.ToLowerInvariant();

        var result = new StringBuilder();
        foreach (var (word, _) in words)
        {
            var lower = word.ToLowerInvariant();
            result.Append(char.ToUpperInvariant(lower[0]));
            if (lower.Length > 1)
                result.Append(lower.Substring(1));
        }
        return result.ToString();
    }

    /// <summary>snake_case: все строчные, слова через подчёркивание.</summary>
    private static string ToSnakeCase(string text)
    {
        var words = ExtractWords(text);
        if (words.Count == 0) return text.ToLowerInvariant();

        return string.Join("_", words.Select(w => w.Word.ToLowerInvariant()));
    }

    /// <summary>kebab-case: все строчные, слова через дефис.</summary>
    private static string ToKebabCase(string text)
    {
        var words = ExtractWords(text);
        if (words.Count == 0) return text.ToLowerInvariant();

        return string.Join("-", words.Select(w => w.Word.ToLowerInvariant()));
    }

    /// <summary>Извлекает слова (буквенные последовательности) и их позиции в тексте.</summary>
    private static List<(string Word, int Index)> ExtractWords(string text)
    {
        var matches = WordRegex.Matches(text);
        var result = new List<(string, int)>(matches.Count);
        foreach (Match match in matches)
        {
            result.Add((match.Value, match.Index));
        }
        return result;
    }
}
