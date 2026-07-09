using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

public class LoremIpsumServiceTests
{
    private readonly LoremIpsumService _service = new();

    [Fact]
    public void Execute_GenerateWords_ReturnsCorrectCount()
    {
        string input = "{\"type\": \"words\", \"count\": 5}";
        string result = _service.Execute(input);
        var words = result.Split(' ');
        Assert.Equal(5, words.Length);
    }

    [Fact]
    public void Execute_GenerateChars_ReturnsCorrectLength()
    {
        string input = "{\"type\": \"chars\", \"count\": 10}";
        string result = _service.Execute(input);
        Assert.Equal(10, result.Length);
    }

    [Fact]
    public void Execute_GenerateParagraphs_ReturnsText()
    {
        string input = "{\"type\": \"paragraphs\", \"count\": 1}";
        string result = _service.Execute(input);
        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.Contains(' ', result);
        Assert.Contains(".", result);
    }

    [Fact]
    public void Execute_GenerateParagraphs2_ReturnsTwoParagraphs()
    {
        string input = "{\"type\": \"paragraphs\", \"count\": 2}";
        string result = _service.Execute(input);
        var paragraphs = result.Split("\n\n");
        Assert.Equal(2, paragraphs.Length);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string result = _service.Execute("");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidJson_ReturnsError()
    {
        string result = _service.Execute("not json");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_UnknownType_ReturnsError()
    {
        string input = "{\"type\": \"invalid\", \"count\": 3}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_CountZero_ReturnsError()
    {
        string input = "{\"type\": \"words\", \"count\": 0}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_CountExceedsLimit_ReturnsError()
    {
        string input = "{\"type\": \"words\", \"count\": 1001}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }
}
