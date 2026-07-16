using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WebApp.Api.Services;

/// <summary>
/// Валидатор и конвертер JSON / YAML ★★★
/// Проверяет синтаксис JSON или YAML, форматирует с отступами, конвертирует между форматами.
/// Endpoint: yaml-json
/// </summary>
public class YamlJsonService : IUtilityService
{
    public string Endpoint => "yaml-json";

    private static readonly JsonSerializerOptions PrettyJsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Первая строка — режим (validate/json2yaml/yaml2json), далее — данные.";

        var lines = input.Split('\n', StringSplitOptions.None)
                         .Select(l => l.TrimEnd('\r'))
                         .ToList();

        // Убираем пустые строки в конце
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
            lines.RemoveAt(lines.Count - 1);

        if (lines.Count < 2)
            return "Ошибка: после режима не указаны данные.";

        var mode = lines[0].Trim().ToLowerInvariant();

        // Режимы json2yaml и yaml2json: данные могут быть многострочными,
        // берём ВСЁ после первой строки как есть (включая пустые строки между ними).
        var content = string.Join("\n", lines.Skip(1));

        if (string.IsNullOrWhiteSpace(content))
            return "Ошибка: данные не могут быть пустыми.";

        return mode switch
        {
            "validate" => Validate(content),
            "json2yaml" => JsonToYaml(content),
            "yaml2json" => YamlToJson(content),
            _ => "Ошибка: режим должен быть «validate», «json2yaml» или «yaml2json»."
        };
    }

    /// <summary>Валидация JSON или YAML с указанием строки/позиции ошибки.</summary>
    private static string Validate(string content)
    {
        // Пробуем JSON
        try
        {
            using var doc = JsonDocument.Parse(content);
            var pretty = JsonSerializer.Serialize(doc.RootElement, PrettyJsonOptions);
            return $"JSON корректен\n\n{pretty}";
        }
        catch (JsonException)
        {
            // Если JSON невалидный — пытаемся YAML
        }

        // Пробуем YAML
        try
        {
            var yamlStream = new YamlStream();
            using var reader = new StringReader(content);
            yamlStream.Load(reader);
            return "YAML корректен";
        }
        catch (YamlException ex)
        {
            var line = ex.Start.Line + 1;      // YamlDotNet.Line — 0-based
            var column = ex.Start.Column + 1;  // Column — 0-based
            return $"YAML ошибка в строке {line}, позиция {column}: {ex.Message}";
        }
    }

    /// <summary>Конвертация JSON → YAML.</summary>
    private static string JsonToYaml(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            var obj = ConvertJsonElementToObject(doc.RootElement);

            var yamlSerializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return yamlSerializer.Serialize(obj).TrimEnd();
        }
        catch (JsonException ex)
        {
            return $"Ошибка: неверный JSON — {ex.Message}";
        }
    }

    /// <summary>Конвертация YAML → JSON.</summary>
    private static string YamlToJson(string content)
    {
        try
        {
            var yamlStream = new YamlStream();
            using var reader = new StringReader(content);
            yamlStream.Load(reader);

            if (yamlStream.Documents.Count == 0)
                return "Ошибка: YAML-документ пуст.";

            var rootNode = yamlStream.Documents[0].RootNode;
            var obj = ConvertYamlNodeToObject(rootNode);

            return JsonSerializer.Serialize(obj, PrettyJsonOptions);
        }
        catch (YamlException ex)
        {
            var line = ex.Start.Line + 1;
            var column = ex.Start.Column + 1;
            return $"Ошибка: неверный YAML (строка {line}, позиция {column}): {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Ошибка: неверный YAML — {ex.Message}";
        }
    }

    /// <summary>Рекурсивно конвертирует JsonElement в Dictionary/List/примитив для YamlDotNet.</summary>
    private static object? ConvertJsonElementToObject(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(p => p.Name, p => ConvertJsonElementToObject(p.Value)),
            JsonValueKind.Array => element.EnumerateArray()
                .Select(ConvertJsonElementToObject).ToList(),
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.TryGetInt64(out var l) ? (object)l : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.GetRawText()
        };
    }

    /// <summary>Рекурсивно конвертирует YamlNode в Dictionary/List/примитив для System.Text.Json.</summary>
    private static object? ConvertYamlNodeToObject(YamlNode node)
    {
        return node switch
        {
            YamlMappingNode mapping => mapping.Children
                .ToDictionary(kvp => kvp.Key.ToString(), kvp => ConvertYamlNodeToObject(kvp.Value)),
            YamlSequenceNode sequence => sequence.Children
                .Select(ConvertYamlNodeToObject).ToList(),
            YamlScalarNode scalar => ParseYamlScalar(scalar),
            _ => node.ToString()
        };
    }

    /// <summary>Парсит YAML-скаляр: пытается определить число, bool или null.</summary>
    private static object? ParseYamlScalar(YamlScalarNode scalar)
    {
        var value = scalar.Value;

        if (value is null)
            return null;

        // null / ~ (YamlDotNet представляет null-скаляры как null)
        if (value == "null" || value == "~" || value == "")
            return null;

        // bool
        if (bool.TryParse(value, out var b))
            return b;

        // int
        if (long.TryParse(value, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out var l))
            return l;

        // double
        if (double.TryParse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var d))
            return d;

        return value;
    }
}
