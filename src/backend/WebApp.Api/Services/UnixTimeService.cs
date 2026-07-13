using System.Globalization;

namespace WebApp.Api.Services;

/// <summary>
/// Unix Timestamp конвертер ★☆☆
/// Конвертирует Unix timestamp в читаемую дату и обратно.
/// Endpoint: unix-time
/// </summary>
public class UnixTimeService : IUtilityService
{
    public string Endpoint => "unix-time";

    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты.";

        var lines = input.Split('\n', StringSplitOptions.None);
        var mode = lines[0].Trim().ToLowerInvariant();

        if (mode != "to-date" && mode != "to-timestamp")
            return "Ошибка: первая строка должна быть «to-date» или «to-timestamp».";

        if (lines.Length < 2 || string.IsNullOrWhiteSpace(lines[1]))
            return "Ошибка: не введено значение для конвертации.";

        var value = lines[1].Trim();

        try
        {
            return mode switch
            {
                "to-date" => ToDate(value),
                "to-timestamp" => ToTimestamp(value),
                _ => "Ошибка: неизвестный режим."
            };
        }
        catch (FormatException)
        {
            return "Ошибка: неверный формат числа.";
        }
        catch (ArgumentOutOfRangeException)
        {
            return "Ошибка: значение выходит за допустимые пределы.";
        }
    }

    private string ToDate(string timestampStr)
    {
        if (!long.TryParse(timestampStr, out var timestamp) || timestamp < 0)
            throw new FormatException();

        var date = UnixEpoch.AddSeconds(timestamp);
        return date.ToString("dd.MM.yyyy HH:mm:ss") + " (UTC)";
    }

    private string ToTimestamp(string dateStr)
    {
        if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var date))
        {
            var timestamp = (long)(date.ToUniversalTime() - UnixEpoch).TotalSeconds;
            return timestamp.ToString();
        }

        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var date2))
        {
            var timestamp = (long)(date2.ToUniversalTime() - UnixEpoch).TotalSeconds;
            return timestamp.ToString();
        }

        throw new FormatException();
    }
}
