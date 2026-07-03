using System.Text;

namespace WebApp.Api.Services;

/// <summary>
/// Утилита "Текст в список C# / JavaScript / Python / SQL"
/// </summary>
public class TextToListService : IUtilityService
{
    public string Endpoint => "text-to-list";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Укажите язык и строки.";

        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                         .Select(l => l.Trim())
                         .Where(l => !string.IsNullOrEmpty(l))
                         .ToList();

        if (lines.Count == 0)
            return "Ошибка: нет данных для преобразования.";

        // Первая строка — язык
        string language = lines[0].ToLower().Trim();
        var items = lines.Skip(1).ToList();

        if (items.Count == 0)
            return "Ошибка: после указания языка нет строк для преобразования.";

        return language switch
        {
            "csharp" or "c#" => ToCSharp(items),
            "javascript" or "js" => ToJavaScript(items),
            "python" or "py" => ToPython(items),
            "sql" or "in" => ToSql(items),
            _ => $"Ошибка: неизвестный язык '{language}'. Доступные: csharp, javascript, python, sql"
        };
    }

    private string ToCSharp(List<string> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("var items = new List<string>");
        sb.AppendLine("{");
        for (int i = 0; i < items.Count; i++)
        {
            string comma = i < items.Count - 1 ? "," : "";
            sb.AppendLine($"    \"{EscapeQuotes(items[i])}\"{comma}");
        }
        sb.AppendLine("};");
        return sb.ToString();
    }

    private string ToJavaScript(List<string> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("const items = [");
        for (int i = 0; i < items.Count; i++)
        {
            string comma = i < items.Count - 1 ? "," : "";
            sb.AppendLine($"    \"{EscapeQuotes(items[i])}\"{comma}");
        }
        sb.AppendLine("];");
        return sb.ToString();
    }

    private string ToPython(List<string> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("items = [");
        for (int i = 0; i < items.Count; i++)
        {
            string comma = i < items.Count - 1 ? "," : "";
            sb.AppendLine($"    \"{EscapeQuotes(items[i])}\"{comma}");
        }
        sb.AppendLine("]");
        return sb.ToString();
    }

    private string ToSql(List<string> items)
    {
        var quoted = items.Select(item => $"'{EscapeSingleQuote(item)}'");
        return $"IN ({string.Join(", ", quoted)})";
    }

    private string EscapeQuotes(string value)
    {
        return value.Replace("\"", "\\\"");
    }

    private string EscapeSingleQuote(string value)
    {
        return value.Replace("'", "''");
    }
}