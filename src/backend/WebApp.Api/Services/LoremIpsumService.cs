using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

/// <summary>
/// Генератор Lorem Ipsum ★☆☆
/// Генерирует заданное количество абзацев, слов или символов текста-рыбы.
/// Endpoint: lorem-ipsum
/// </summary>
public class LoremIpsumService : IUtilityService
{
    public string Endpoint => "lorem-ipsum";

    private static readonly string[] LoremWords =
    [
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit",
        "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
        "magna", "aliqua", "ut", "enim", "ad", "minim", "veniam", "quis", "nostrud",
        "exercitation", "ullamco", "laboris", "nisi", "ut", "aliquip", "ex", "ea",
        "commodo", "consequat", "duis", "aute", "irure", "dolor", "in", "reprehenderit",
        "in", "voluptate", "velit", "esse", "cillum", "dolore", "eu", "fugiat", "nulla",
        "pariatur", "excepteur", "sint", "occaecat", "cupidatat", "non", "proident",
        "sunt", "in", "culpa", "qui", "officia", "deserunt", "mollit", "anim", "id",
        "est", "laborum"
    ];

    private static readonly string[] SentenceStarters =
    [
        "Lorem ipsum dolor sit amet,", "Consectetur adipiscing elit,",
        "Sed do eiusmod tempor incididunt", "Ut labore et dolore magna aliqua.",
        "Ut enim ad minim veniam,", "Quis nostrud exercitation ullamco",
    ];

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты.";

        try
        {
            var request = JsonSerializer.Deserialize<LoremRequest>(input);
            if (request == null)
                return "Ошибка: неверный формат JSON.";

            if (request.Count < 1)
                return "Ошибка: количество должно быть положительным числом.";

            if (request.Count > 1000)
                return "Ошибка: количество не должно превышать 1000.";

            return request.Type switch
            {
                "paragraphs" => GenerateParagraphs(request.Count),
                "words" => GenerateWords(request.Count),
                "chars" => GenerateChars(request.Count),
                _ => $"Ошибка: неизвестный тип «{request.Type}». Используйте: paragraphs, words, chars."
            };
        }
        catch (JsonException)
        {
            return "Ошибка: неверный формат JSON.";
        }
    }

    private string GenerateParagraphs(int count)
    {
        var random = new Random();
        var paragraphs = new string[count];
        for (int p = 0; p < count; p++)
        {
            int sentenceCount = random.Next(3, 7);
            var sentences = new string[sentenceCount];
            for (int s = 0; s < sentenceCount; s++)
            {
                int wordCount = random.Next(5, 15);
                var words = new List<string>();
                for (int w = 0; w < wordCount; w++)
                    words.Add(LoremWords[random.Next(LoremWords.Length)]);

                var sentence = char.ToUpper(words[0][0]) + words[0].Substring(1)
                    + " " + string.Join(" ", words.Skip(1)) + ".";
                sentences[s] = sentence;
            }
            paragraphs[p] = string.Join(" ", sentences);
        }
        return string.Join("\n\n", paragraphs);
    }

    private string GenerateWords(int count)
    {
        var random = new Random();
        var words = new string[count];
        for (int i = 0; i < count; i++)
            words[i] = LoremWords[random.Next(LoremWords.Length)];
        return string.Join(" ", words);
    }

    private string GenerateChars(int count)
    {
        var random = new Random();
        var chars = new char[count];
        var loremText = string.Join(" ", LoremWords);
        for (int i = 0; i < count; i++)
            chars[i] = loremText[random.Next(loremText.Length)];
        return new string(chars);
    }

    private class LoremRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "paragraphs";

        [JsonPropertyName("count")]
        public int Count { get; set; } = 1;
    }
}
