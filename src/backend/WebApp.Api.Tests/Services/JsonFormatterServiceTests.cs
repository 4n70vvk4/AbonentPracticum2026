using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для JSON-форматировщик (json-formatter)</summary>
public class JsonFormatterServiceTests
{
    private readonly JsonFormatterService _service = new();

    [Fact]
    public void Execute_Pretty_FormatsWithIndentation()
    {
        string input = "pretty\n{\"a\":1,\"b\":[2,3]}";
        string result = _service.Execute(input);

        Assert.Contains("\"a\": 1", result);
        Assert.Contains("\"b\": [", result);
        Assert.Contains("2,", result);
        Assert.Contains("3", result);
    }

    [Fact]
    public void Execute_Pretty_ExampleFromSpec_Works()
    {
        // Пример: pretty + {"a":1,"b":[2,3]} → форматированный
        string input = "pretty\n{\"a\":1,\"b\":[2,3]}";
        string result = _service.Execute(input);

        Assert.Contains("\"a\": 1", result);
        Assert.Contains("\"b\": [", result);
        // Проверяем, что появились отступы (pretty-print)
        Assert.Contains("  ", result);
    }

    [Fact]
    public void Execute_Minify_MinifiesJson()
    {
        string input = "minify\n{\n  \"a\": 1,\n  \"b\": 2\n}";
        string result = _service.Execute(input);

        // Минифицированный JSON — без лишних пробелов
        Assert.Equal("{\"a\":1,\"b\":2}", result);
    }

    [Fact]
    public void Execute_Minify_NestedObject_Works()
    {
        string input = "minify\n{\"a\":{\"b\":[1,2,3]}}";
        string result = _service.Execute(input);

        Assert.Equal("{\"a\":{\"b\":[1,2,3]}}", result);
    }

    [Fact]
    public void Execute_Pretty_DeeplyNested_FormatsCorrectly()
    {
        string input = "pretty\n{\"a\":{\"b\":{\"c\":\"test\"}}}";
        string result = _service.Execute(input);

        Assert.Contains("\"a\": {", result);
        Assert.Contains("\"b\": {", result);
        Assert.Contains("\"c\": \"test\"", result);
    }

    [Fact]
    public void Execute_Pretty_ArrayOfObjects_Works()
    {
        string input = "pretty\n[{\"x\":1},{\"y\":2}]";
        string result = _service.Execute(input);

        Assert.Contains("\"x\": 1", result);
        Assert.Contains("\"y\": 2", result);
    }

    [Fact]
    public void Execute_InvalidJson_ReturnsError()
    {
        string input = "pretty\n{invalid}";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string result = _service.Execute("");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_NoJsonAfterMode_ReturnsError()
    {
        string result = _service.Execute("pretty");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidMode_ReturnsError()
    {
        string result = _service.Execute("invalid\n{}");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_WhitespaceJson_ReturnsError()
    {
        string result = _service.Execute("pretty\n   ");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_Minify_Roundtrip_PrettyThenMinify_ReturnsOriginal()
    {
        string original = "{\"name\":\"test\",\"value\":42}";
        string pretty = _service.Execute($"pretty\n{original}");
        string minified = _service.Execute($"minify\n{pretty}");

        Assert.Equal(original, minified);
    }

    [Fact]
    public void Execute_Pretty_SpecialCharacters_Works()
    {
        string input = "pretty\n{\"text\":\"hello\\nworld\"}";
        string result = _service.Execute(input);

        Assert.Contains("\"text\": \"hello\\nworld\"", result);
    }

    [Fact]
    public void Execute_Pretty_EmptyObject_Works()
    {
        string input = "pretty\n{}";
        string result = _service.Execute(input);

        Assert.Equal("{}", result);
    }
}
