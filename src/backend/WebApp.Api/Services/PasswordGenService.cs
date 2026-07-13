using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

/// <summary>
/// Генератор паролей ★☆☆
/// Генерирует надёжный пароль заданной длины с выбором наборов символов.
/// Endpoint: password-gen
/// </summary>
public class PasswordGenService : IUtilityService
{
    public string Endpoint => "password-gen";

    public string Execute(string input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Ошибка: входные данные пусты.";

            var request = JsonSerializer.Deserialize<PasswordGenRequest>(input);
            if (request == null)
                return "Ошибка: неверный формат JSON.";

            if (request.Length < 4)
                return "Ошибка: длина пароля должна быть минимум 4 символа.";

            if (request.Length > 256)
                return "Ошибка: длина пароля не должна превышать 256 символов.";

            if (!request.UseUpper && !request.UseLower && !request.UseDigits && !request.UseSymbols)
                return "Ошибка: должен быть включён хотя бы один набор символов.";

            var password = GeneratePassword(request.Length, request.UseUpper, request.UseLower, request.UseDigits, request.UseSymbols);
            return password;
        }
        catch (JsonException)
        {
            return "Ошибка: неверный формат JSON.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

    private string GeneratePassword(int length, bool useUpper, bool useLower, bool useDigits, bool useSymbols)
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var charPool = new StringBuilder();
        if (useUpper) charPool.Append(upper);
        if (useLower) charPool.Append(lower);
        if (useDigits) charPool.Append(digits);
        if (useSymbols) charPool.Append(symbols);

        var availableChars = charPool.ToString();
        var random = new Random();
        var result = new StringBuilder();

        if (useUpper) result.Append(upper[random.Next(upper.Length)]);
        if (useLower) result.Append(lower[random.Next(lower.Length)]);
        if (useDigits) result.Append(digits[random.Next(digits.Length)]);
        if (useSymbols) result.Append(symbols[random.Next(symbols.Length)]);

        for (int i = result.Length; i < length; i++)
        {
            result.Append(availableChars[random.Next(availableChars.Length)]);
        }

        return ShuffleString(result.ToString());
    }

    private string ShuffleString(string input)
    {
        var random = new Random();
        var chars = input.ToCharArray();
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars);
    }

    private class PasswordGenRequest
    {
        [JsonPropertyName("length")]
        public int Length { get; set; } = 16;

        [JsonPropertyName("useUpper")]
        public bool UseUpper { get; set; } = true;

        [JsonPropertyName("useLower")]
        public bool UseLower { get; set; } = true;

        [JsonPropertyName("useDigits")]
        public bool UseDigits { get; set; } = true;

        [JsonPropertyName("useSymbols")]
        public bool UseSymbols { get; set; } = true;
    }
}