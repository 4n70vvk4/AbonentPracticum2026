using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Подсчёт символов и слов (text-stats)</summary>
public class TextStatsServiceTests
{
    private readonly TextStatsService _service = new();

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string input = "";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_WhitespaceInput_ReturnsError()
    {
        string input = "   \n  \t  ";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_SimpleText_ReturnsCorrectStats()
    {
        string input = "Привет, мир!\nКак дела?";
        string result = _service.Execute(input);

        Assert.Contains("Символов (с пробелами): 22", result);
        Assert.Contains("Символов (без пробелов): 19", result);
        Assert.Contains("Слов: 4", result);
        Assert.Contains("Строк: 2", result);
        Assert.Contains("Предложений: 2", result);
    }

    [Fact]
    public void Execute_SingleWord_ReturnsCorrectStats()
    {
        string input = "Hello";
        string result = _service.Execute(input);

        Assert.Contains("Символов (с пробелами): 5", result);
        Assert.Contains("Символов (без пробелов): 5", result);
        Assert.Contains("Слов: 1", result);
        Assert.Contains("Строк: 1", result);
        Assert.Contains("Предложений: 1", result);
    }

    [Fact]
    public void Execute_MultipleSentences_ReturnsCorrectStats()
    {
        string input = "Привет! Как дела? Хорошо.";
        string result = _service.Execute(input);
        Assert.Contains("Предложений: 3", result);
    }

    [Fact]
    public void Execute_TextWithDigits_CountsAsWords()
    {
        string input = "123 456 789";
        string result = _service.Execute(input);
        Assert.Contains("Слов: 3", result);
        Assert.Contains("Символов (без пробелов): 9", result);
    }

    [Fact]
    public void Execute_TextWithPunctuation_CountsCorrectly()
    {
        string input = "Hello, world! This is a test.";
        string result = _service.Execute(input);

        Assert.Contains("Символов (с пробелами): 29", result);
        Assert.Contains("Символов (без пробелов): 24", result);
        Assert.Contains("Слов: 6", result);
        Assert.Contains("Предложений: 2", result);
    }

    [Fact]
    public void Execute_TextWithNewLines_CountsCorrectly()
    {
        string input = "Line1\nLine2\nLine3";
        string result = _service.Execute(input);
        Assert.Contains("Строк: 3", result);
        Assert.Contains("Слов: 3", result);
    }

    [Fact]
    public void Execute_RussianText_CountsCorrectly()
    {
        string input = "Мама мыла раму. Папа мыл пол.";
        string result = _service.Execute(input);

        Assert.Contains("Символов (с пробелами): 29", result);
        Assert.Contains("Символов (без пробелов): 24", result);
        Assert.Contains("Слов: 6", result);
        Assert.Contains("Предложений: 2", result);
    }

    [Fact]
    public void Execute_OnlySymbols_CountsCorrectly()
    {
        string input = "!!! ??? ...";
        string result = _service.Execute(input);

        Assert.Contains("Символов (без пробелов): 9", result);
        Assert.Contains("Слов: 0", result);
        Assert.Contains("Предложений: 3", result);
    }
}