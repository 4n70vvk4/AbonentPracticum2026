using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

public class PercentCalcService : IUtilityService
{
    public string Endpoint => "percent-calc";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты.";

        try
        {
            var request = JsonSerializer.Deserialize<PercentRequest>(input);
            if (request == null)
                return "Ошибка: неверный формат JSON.";

            return request.Operation switch
            {
                "percent-of" => PercentOf(request.Value1, request.Value2),
                "change" => Change(request.Value1, request.Value2),
                "proportion" => Proportion(request.Value1, request.Value2),
                _ => $"Ошибка: неизвестная операция «{request.Operation}». Используйте: percent-of, change, proportion."
            };
        }
        catch (JsonException)
        {
            return "Ошибка: неверный формат JSON.";
        }
    }

    private string PercentOf(double value1, double value2)
    {
        if (value2 == 0)
            return "Ошибка: второе значение не может быть нулём.";

        var result = (value1 / value2) * 100;
        var formattedResult = FormatNumber(result);
        var formattedValue1 = FormatNumber(value1);
        var formattedValue2 = FormatNumber(value2);
        return $"{formattedValue1} составляет {formattedResult}% от {formattedValue2}";
    }

    private string Change(double value1, double value2)
    {
        if (value1 == 0)
            return "Ошибка: первое значение не может быть нулём.";

        var change = ((value2 - value1) / Math.Abs(value1)) * 100;
        var difference = value2 - value1;
        var formattedChange = FormatNumber(Math.Abs(change));
        var formattedDiff = FormatNumber(Math.Abs(difference));
        var sign = change >= 0 ? "+" : "-";

        string direction = difference switch
        {
            > 0 => "увеличение",
            < 0 => "уменьшение",
            _ => "изменение"
        };

        return $"Изменение: {sign}{formattedChange}% ({direction} на {formattedDiff})";
    }

    private string Proportion(double value1, double value2)
    {
        if (value2 == 0)
            return "Ошибка: второе значение не может быть нулём.";

        var result = value1 / value2;
        var formattedResult = FormatNumber(result);
        var formattedValue1 = FormatNumber(value1);
        var formattedValue2 = FormatNumber(value2);
        return $"{formattedValue1} / {formattedValue2} = {formattedResult}";
    }

    private static string FormatNumber(double value)
    {
        if (value == Math.Floor(value) && !double.IsInfinity(value))
            return value.ToString("0", CultureInfo.InvariantCulture);

        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private class PercentRequest
    {
        [JsonPropertyName("operation")]
        public string Operation { get; set; } = "percent-of";

        [JsonPropertyName("value1")]
        public double Value1 { get; set; }

        [JsonPropertyName("value2")]
        public double Value2 { get; set; }
    }
}
