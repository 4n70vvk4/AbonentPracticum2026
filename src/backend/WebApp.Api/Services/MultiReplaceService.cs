using System.Text.Json;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

/// <summary>
/// Утилита "Множественная замена в тексте" ★★☆
/// Выполняет ОДНОВРЕМЕННУЮ (атомарную) замену нескольких подстрок по словарю.
///
/// Вход — JSON-строка с двумя полями:
///   text         — исходный текст
///   replacements — объект { "что": "на что" }
///
/// Ключевой момент: замены атомарны.
/// Обычный последовательный вызов String.Replace даёт каскадный эффект:
/// если заменить "a" → "ab", а потом искать "ab", то вторая замена
/// применится к ТОЛЬКО ЧТО вставленному тексту — это неправильно.
///
/// Решение: Regex.Replace с одним паттерном из всех ключей.
/// За один проход по тексту находятся ВСЕ совпадения и заменяются
/// независимо друг от друга.
/// </summary>
public class MultiReplaceService : IUtilityService
{
    public string Endpoint => "multi-replace";

    public string Execute(string input)
    {
        // --- Валидация ---
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Введите JSON с полями text и replacements.";

        // --- Парсинг JSON ---
        MultiReplaceRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<MultiReplaceRequest>(input,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            return $"Ошибка: неверный формат JSON — {ex.Message}";
        }

        if (request is null)
            return "Ошибка: не удалось разобрать JSON.";

        if (string.IsNullOrEmpty(request.Text))
            return "Ошибка: поле text не может быть пустым.";

        if (request.Replacements is null || request.Replacements.Count == 0)
            return "Ошибка: поле replacements не может быть пустым. Укажите хотя бы одну пару для замены.";

        // --- Атомарная замена ---

        // ШАГ 1: Сортируем ключи по убыванию длины.
        //   Почему? Если ключи "ab" и "a" оба присутствуют, то в тексте "ab"
        //   мы должны заменить "ab" целиком, а не "a" по отдельности.
        //   Regex.Alternation («a|ab») пробует варианты слева направо,
        //   поэтому более длинные ключи ставим первыми.
        var sortedKeys = request.Replacements.Keys
            .OrderByDescending(k => k.Length)
            .ToArray();

        // ШАГ 2: Строим единый паттерн из ВСЕХ ключей.
        //   Regex.Escape экранирует спецсимволы (., *, +, \, (, ) и т.д.)
        var pattern = string.Join("|", sortedKeys.Select(Regex.Escape));
        var regex = new Regex(pattern);

        // ШАГ 3: Один проход — каждая замена независима.
        //   MatchEvaluator вызывается для каждого найденного совпадения.
        //   Результат одной замены НЕ участвует в поиске других замен.
        var result = regex.Replace(request.Text,
            match => request.Replacements[match.Value]);

        return result;
    }
}

/// <summary>
/// DTO для десериализации входящего JSON.
/// Поля могут приходить в любом регистре (camelCase, PascalCase — без разницы).
/// </summary>
public class MultiReplaceRequest
{
    public string Text { get; set; } = string.Empty;
    public Dictionary<string, string>? Replacements { get; set; }
}
