using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

public class TextStatsService : IUtilityService
{
    public string Endpoint => "text-stats";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Ошибка: входные данные пусты. Введите текст для анализа.";

        var stats = GetStats(input);
        return FormatReport(stats);
    }

    private (int totalChars, int charsWithoutSpaces, int words, int lines, int sentences) GetStats(string text)
    {
        int totalChars = text.Length;
        int charsWithoutSpaces = text.Count(c => !char.IsWhiteSpace(c));
        int lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        var wordMatches = Regex.Matches(text, @"[\p{L}\p{N}]+");
        int words = wordMatches.Count;
        var sentenceMatches = Regex.Matches(text, @"[^.!?]*[.!?]+");
        int sentences = sentenceMatches.Count;

        if (sentences == 0 && !string.IsNullOrWhiteSpace(text) && Regex.IsMatch(text, @"[\p{L}\p{N}]"))
            sentences = 1;

        return (totalChars, charsWithoutSpaces, words, lines, sentences);
    }

    private string FormatReport((int totalChars, int charsWithoutSpaces, int words, int lines, int sentences) stats)
    {
        return $@"  Символов (с пробелами): {stats.totalChars}
  Символов (без пробелов): {stats.charsWithoutSpaces}
  Слов: {stats.words}
  Строк: {stats.lines}
  Предложений: {stats.sentences}";
    }
}