using System.Security.Cryptography;
using System.Text;

namespace WebApp.Api.Services;

public class HashCalcService : IUtilityService
{
    public string Endpoint => "hash-calc";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты.";

        var lines = input.Split('\n', StringSplitOptions.None);
        var algorithm = lines[0].Trim().ToLowerInvariant();

        if (lines.Length < 2)
            return "Ошибка: не введён текст для хеширования.";

        var data = string.Join("\n", lines.Skip(1));

        byte[] hash;
        switch (algorithm)
        {
            case "md5": hash = MD5.HashData(Encoding.UTF8.GetBytes(data)); break;
            case "sha1": hash = SHA1.HashData(Encoding.UTF8.GetBytes(data)); break;
            case "sha256": hash = SHA256.HashData(Encoding.UTF8.GetBytes(data)); break;
            case "sha512": hash = SHA512.HashData(Encoding.UTF8.GetBytes(data)); break;
            default:
                return $"Ошибка: неизвестный алгоритм «{algorithm}». Используйте: md5, sha1, sha256, sha512.";
        }

        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
