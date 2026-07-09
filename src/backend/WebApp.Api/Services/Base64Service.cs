using System.Text;

namespace WebApp.Api.Services;

public class Base64Service : IUtilityService
{
    public string Endpoint => "base64";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Введите режим (encode/decode) и данные.";

        var lines = input.Split('\n', StringSplitOptions.None);
        var mode = lines[0].Trim().ToLowerInvariant();

        if (mode != "encode" && mode != "decode")
            return "Ошибка: первая строка должна быть «encode» или «decode».";

        var data = string.Join("\n", lines.Skip(1));

        if (string.IsNullOrEmpty(data))
            return "Ошибка: не введены данные для обработки.";

        try
        {
            return mode switch
            {
                "encode" => Encode(data),
                "decode" => Decode(data),
                _ => "Ошибка: неизвестный режим."
            };
        }
        catch (FormatException)
        {
            return "Ошибка: строка не является корректным Base64.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

    private string Encode(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(bytes);
    }

    private string Decode(string data)
    {
        var bytes = Convert.FromBase64String(data);
        return Encoding.UTF8.GetString(bytes);
    }
}
