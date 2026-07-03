using Microsoft.EntityFrameworkCore;
using WebApp.Api.Models;

namespace WebApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Utility> Utilities => Set<Utility>();
    public DbSet<ExecutionResult> ExecutionResults => Set<ExecutionResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ExecutionResult>()
            .HasOne(e => e.Utility)
            .WithMany()
            .HasForeignKey(e => e.UtilityId);

        // Начальные данные — список утилит-заготовок для студентов
        builder.Entity<Utility>().HasData(
            new Utility
            {
                Id = 1,
                Name = "Сумма чисел",
                Description = "Суммирует числа, переданные по одному на строку.",
                Endpoint = "sum-numbers",
                Category = "Числа",
                Difficulty = 1,
                IsImplemented = true
            },
            new Utility
            {
                Id = 2,
                Name = "Множественная замена в тексте",
                Description = "Заменяет несколько подстрок одновременно по словарю замен.",
                Endpoint = "multi-replace",
                Category = "Текст",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 3,
                Name = "CSV в SQL INSERT",
                Description = "Преобразует CSV-данные в SQL-скрипт для создания временной таблицы и вставки строк.",
                Endpoint = "csv-to-sql",
                Category = "Данные",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 4,
                Name = "Текст в список C# / JS",
                Description = "Преобразует строки (по одной на линию) в переменную-список на C# или JavaScript.",
                Endpoint = "text-to-list",
                Category = "Код",
                Difficulty = 1,
                IsImplemented = true
            },
            new Utility
            {
                Id = 5,
                Name = "Генератор паролей",
                Description = "Генерирует надёжный пароль заданной длины с выбором наборов символов.",
                Endpoint = "password-gen",
                Category = "Безопасность",
                Difficulty = 1,
                IsImplemented = true
            },
            new Utility
            {
                Id = 6,
                Name = "Подсчёт символов / слов",
                Description = "Считает символы, слова, строки и предложения в тексте.",
                Endpoint = "text-stats",
                Category = "Текст",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 7,
                Name = "Конвертер регистров",
                Description = "Преобразует текст в UPPER, lower, Title Case, camelCase, snake_case.",
                Endpoint = "case-converter",
                Category = "Текст",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 8,
                Name = "Base64 кодер / декодер",
                Description = "Кодирует и декодирует строки в формат Base64.",
                Endpoint = "base64",
                Category = "Кодирование",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 9,
                Name = "JSON-форматировщик",
                Description = "Форматирует (pretty-print) или минифицирует JSON-строку.",
                Endpoint = "json-formatter",
                Category = "Данные",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 10,
                Name = "Хеш-калькулятор",
                Description = "Вычисляет MD5 / SHA1 / SHA256 хеш переданной строки.",
                Endpoint = "hash-calc",
                Category = "Безопасность",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 11,
                Name = "Генератор UUID / GUID",
                Description = "Генерирует один или несколько UUID и копирует в буфер обмена.",
                Endpoint = "uuid-gen",
                Category = "Код",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 12,
                Name = "Сортировщик строк",
                Description = "Сортирует строки по алфавиту, длине, в обратном порядке, с удалением дубликатов.",
                Endpoint = "string-sort",
                Category = "Текст",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 13,
                Name = "Калькулятор дат",
                Description = "Вычисляет разницу между датами, добавляет дни / месяцы / годы к дате.",
                Endpoint = "date-calc",
                Category = "Дата и время",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 14,
                Name = "Конвертер чисел (системы счисления)",
                Description = "Переводит числа между DEC, HEX, BIN, OCT с произвольной точностью.",
                Endpoint = "number-base",
                Category = "Числа",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 15,
                Name = "Экранирование строк",
                Description = "Экранирует / деэкранирует строки для HTML, JSON, SQL, URL.",
                Endpoint = "string-escape",
                Category = "Кодирование",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 16,
                Name = "Генератор Lorem Ipsum",
                Description = "Генерирует заданное число абзацев / слов / символов текста-рыбы.",
                Endpoint = "lorem-ipsum",
                Category = "Текст",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 17,
                Name = "Валидатор и форматировщик JSON / YAML",
                Description = "Проверяет корректность структуры и форматирует с отступами.",
                Endpoint = "yaml-json",
                Category = "Данные",
                Difficulty = 3,
                IsImplemented = false
            },
            new Utility
            {
                Id = 18,
                Name = "Преобразователь CSV ↔ JSON",
                Description = "Конвертирует CSV в массив JSON-объектов и обратно.",
                Endpoint = "csv-json",
                Category = "Данные",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 19,
                Name = "Построитель регулярных выражений",
                Description = "Помогает составить и протестировать регулярное выражение на тестовых строках.",
                Endpoint = "regex-tester",
                Category = "Код",
                Difficulty = 3,
                IsImplemented = false
            },
            new Utility
            {
                Id = 20,
                Name = "Unix Timestamp конвертер",
                Description = "Конвертирует Unix timestamp в читаемую дату и обратно.",
                Endpoint = "unix-time",
                Category = "Дата и время",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 21,
                Name = "JWT Decoder & Debugger",
                Description = "Декодирует JWT-токен, показывает Header/Payload в читаемом виде, подсвечивает истекшие токены, генерирует тестовые JWT.",
                Endpoint = "jwt-debugger",
                Category = "Безопасность",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 22,
                Name = "Инспектор символов (ASCII/Unicode)",
                Description = "Показывает ASCII/Unicode-коды каждого символа текста и собирает строку из кодов обратно. Помогает найти невидимые символы.",
                Endpoint = "char-inspector",
                Category = "Текст",
                Difficulty = 1,
                IsImplemented = false
            },
            new Utility
            {
                Id = 23,
                Name = "URL Encoder / Decoder + Query Parser",
                Description = "Кодирует/декодирует URL-строки, парсит query-параметры в таблицу ключ-значение с авто-декодированием и генерирует JSON-объект.",
                Endpoint = "url-tools",
                Category = "Кодирование",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 24,
                Name = "Конвертер цветов",
                Description = "Конвертирует цвета между HEX, RGB, HSL, CMYK. Определяет контрастные пары, проверяет на доступность (color-blindness).",
                Endpoint = "color-converter",
                Category = "Дизайн",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 25,
                Name = "SQL Formatter / Minifier",
                Description = "Форматирует SQL-запросы с отступами (beautify) или сжимает в одну строку (minify).",
                Endpoint = "sql-formatter",
                Category = "Код",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 26,
                Name = "Текстовый Diff-инструмент",
                Description = "Сравнивает два блока текста и подсвечивает добавленные, удалённые и изменённые строки.",
                Endpoint = "text-diff",
                Category = "Текст",
                Difficulty = 2,
                IsImplemented = false
            },
            new Utility
            {
                Id = 27,
                Name = "Калькулятор пропорций и процентов",
                Description = "Вычисляет: «X это Y% от чего?», «На сколько % изменилось от A до B?», пропорциональное масштабирование.",
                Endpoint = "percent-calc",
                Category = "Числа",
                Difficulty = 1,
                IsImplemented = false
            }
        );
    }
}
