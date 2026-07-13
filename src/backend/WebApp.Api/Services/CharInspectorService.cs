using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

/// <summary>
/// Инспектор символов (ASCII / Unicode) ★☆☆
/// Показывает ASCII-код или Unicode codepoint каждого символа и собирает строку из кодов обратно.
/// Endpoint: char-inspector
/// </summary>
public class CharInspectorService : IUtilityService
{
    public string Endpoint => "char-inspector";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты.";

        var lines = input.Split('\n', StringSplitOptions.None);
        var mode = lines[0].Trim().ToLowerInvariant();

        if (mode != "encode" && mode != "decode")
            return "Ошибка: первая строка должна быть «encode» или «decode».";

        var data = string.Join("\n", lines.Skip(1));

        if (string.IsNullOrWhiteSpace(data))
            return "Ошибка: не введены данные для обработки.";

        return mode switch
        {
            "encode" => Encode(data),
            "decode" => Decode(data),
            _ => "Ошибка: неизвестный режим."
        };
    }

    private string Encode(string text)
    {
        var codes = new List<string>();
        foreach (char c in text)
        {
            codes.Add($"U+{((int)c):X4}");
        }
        return string.Join(" ", codes);
    }

    private string Decode(string hexCodes)
    {
        var cleaned = hexCodes.Trim();
        if (string.IsNullOrEmpty(cleaned))
            return "";

        var parts = Regex.Split(cleaned, @"\s+");
        var sb = new StringBuilder();

        foreach (var part in parts)
        {
            var hex = part.Trim();
            if (hex.StartsWith("U+", StringComparison.OrdinalIgnoreCase)
                || hex.StartsWith("u+", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex[2..];
            }

            if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int codePoint))
            {
                sb.Append((char)codePoint);
            }
            else
            {
                return $"Ошибка: неверный код «{part}».";
            }
        }

        return sb.ToString();
    }
}
