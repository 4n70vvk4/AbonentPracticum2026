namespace WebApp.Api.Services;

public class StringSortService : IUtilityService
{
    public string Endpoint => "string-sort";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты.";

        var lines = input.Split('\n', StringSplitOptions.None);
        var mode = lines[0].Trim().ToLowerInvariant();
        var items = lines.Skip(1).ToList();

        if (items.Count == 0 || items.All(string.IsNullOrWhiteSpace))
            return "Ошибка: не введены строки для сортировки.";

        var validModes = new[] { "asc", "desc", "length-asc", "length-desc", "unique" };
        if (!validModes.Contains(mode))
            return "Ошибка: неизвестный режим сортировки. Используйте: asc, desc, length-asc, length-desc, unique.";

        var sorted = mode switch
        {
            "asc" => items.OrderBy(x => x, StringComparer.Ordinal).ToList(),
            "desc" => items.OrderByDescending(x => x, StringComparer.Ordinal).ToList(),
            "length-asc" => items.OrderBy(x => x.Length).ThenBy(x => x, StringComparer.Ordinal).ToList(),
            "length-desc" => items.OrderByDescending(x => x.Length).ThenBy(x => x, StringComparer.Ordinal).ToList(),
            "unique" => items.Distinct(StringComparer.Ordinal).ToList(),
            _ => items
        };

        return string.Join("\n", sorted);
    }
}
