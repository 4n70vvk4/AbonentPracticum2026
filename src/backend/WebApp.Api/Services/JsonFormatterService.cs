using System.Text.Json;

namespace WebApp.Api.Services;

/// <summary>
/// JSON-форматировщик ★★☆
/// Форматирует JSON-строку (pretty-print с отступами) или минифицирует.
/// Endpoint: json-formatter
/// </summary>
public class JsonFormatterService : IUtilityService
{
    public string Endpoint => "json-formatter";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Первая строка — режим (pretty/minify), далее — JSON.";

        var lines = input.Split('\n', StringSplitOptions.None)
                         .Select(l => l.TrimEnd('\r'))
                         .ToList();

        if (lines.Count < 2)
            return "Ошибка: после режима не указан JSON.";

        var mode = lines[0].Trim().ToLowerInvariant();
        if (mode != "pretty" && mode != "minify")
            return "Ошибка: режим должен быть «pretty» или «minify».";

        var json = string.Join("\n", lines.Skip(1));

        if (string.IsNullOrWhiteSpace(json))
            return "Ошибка: JSON не может быть пустым.";

        try
        {
            using var doc = JsonDocument.Parse(json);
            var options = new JsonSerializerOptions
            {
                WriteIndented = mode == "pretty"
            };
            return JsonSerializer.Serialize(doc.RootElement, options);
        }
        catch (JsonException ex)
        {
            return $"Ошибка: неверный JSON — {ex.Message}";
        }
    }
}
