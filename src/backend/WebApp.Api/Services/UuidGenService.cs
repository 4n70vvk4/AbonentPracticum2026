namespace WebApp.Api.Services;

/// <summary>
/// Генератор UUID / GUID ★☆☆
/// Генерирует заданное количество UUID.
/// Endpoint: uuid-gen
/// </summary>
public class UuidGenService : IUtilityService
{
    public string Endpoint => "uuid-gen";

    public string Execute(string input)
    {
        var count = 1;
        if (!string.IsNullOrWhiteSpace(input))
        {
            input = input.Trim();
            if (!int.TryParse(input, out count) || count < 1)
                return "Ошибка: введите положительное целое число.";

            if (count > 1000)
                count = 1000;
        }

        var uuids = new string[count];
        for (int i = 0; i < count; i++)
            uuids[i] = Guid.NewGuid().ToString("D");

        return string.Join("\n", uuids);
    }
}
