using System.Text.Json;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

/// <summary>
/// Множественная замена в тексте ★★☆
/// Выполняет одновременную (атомарную) замену нескольких подстрок по словарю через Regex.
/// Endpoint: multi-replace
/// </summary>
public class MultiReplaceService : IUtilityService
{
    public string Endpoint => "multi-replace";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Введите JSON с полями text и replacements.";

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

        // Атомарная замена: сортируем ключи по убыванию длины,
        // чтобы "ab" нашёлся раньше "a", и заменяем все в один проход Regex
        var sortedKeys = request.Replacements.Keys
            .OrderByDescending(k => k.Length)
            .ToArray();

        var pattern = string.Join("|", sortedKeys.Select(Regex.Escape));
        var result = Regex.Replace(request.Text, pattern,
            match => request.Replacements[match.Value]);

        return result;
    }
}

/// <summary>DTO для входящего JSON.</summary>
public class MultiReplaceRequest
{
    public string Text { get; set; } = string.Empty;
    public Dictionary<string, string>? Replacements { get; set; }
}
