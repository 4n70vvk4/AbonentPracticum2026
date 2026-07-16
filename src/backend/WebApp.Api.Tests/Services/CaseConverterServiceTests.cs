using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Конвертер регистров (case-converter)</summary>
public class CaseConverterServiceTests
{
    private readonly CaseConverterService _service = new();

    [Fact]
    public void Execute_Upper_ConvertsToUpperCase()
    {
        string result = _service.Execute("upper\nHello World");
        Assert.Equal("HELLO WORLD", result);
    }

    [Fact]
    public void Execute_Lower_ConvertsToLowerCase()
    {
        string result = _service.Execute("lower\nHELLO World");
        Assert.Equal("hello world", result);
    }

    [Fact]
    public void Execute_Title_ConvertsToTitleCase()
    {
        string result = _service.Execute("title\nhello world example");
        Assert.Equal("Hello World Example", result);
    }

    [Fact]
    public void Execute_Title_MixedCaseWord_ConvertsCorrectly()
    {
        string result = _service.Execute("title\nhELLO wORLD");
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void Execute_CamelCase_ConvertsToCamelCase()
    {
        string result = _service.Execute("camel\nHello World Example");
        Assert.Equal("helloWorldExample", result);
    }

    [Fact]
    public void Execute_CamelCase_SingleWord_ReturnsLowercase()
    {
        string result = _service.Execute("camel\nHello");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void Execute_PascalCase_ConvertsToPascalCase()
    {
        string result = _service.Execute("pascal\nhello world example");
        Assert.Equal("HelloWorldExample", result);
    }

    [Fact]
    public void Execute_PascalCase_SingleWord_ReturnsCapitalized()
    {
        string result = _service.Execute("pascal\nhello");
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Execute_SnakeCase_ConvertsToSnakeCase()
    {
        string result = _service.Execute("snake\nHello World Example");
        Assert.Equal("hello_world_example", result);
    }

    [Fact]
    public void Execute_SnakeCase_SingleWord_ReturnsLowercase()
    {
        string result = _service.Execute("snake\nHello");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void Execute_KebabCase_ConvertsToKebabCase()
    {
        string result = _service.Execute("kebab\nHello World Example");
        Assert.Equal("hello-world-example", result);
    }

    [Fact]
    public void Execute_KebabCase_SingleWord_ReturnsLowercase()
    {
        string result = _service.Execute("kebab\nHello");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void Execute_ExampleFromSpec_WorksCorrectly()
    {
        // Пример из описания: snake_case + "Hello World Example" → "hello_world_example"
        string result = _service.Execute("snake_case\nHello World Example");
        Assert.Equal("hello_world_example", result);
    }

    [Fact]
    public void Execute_CyrillicText_WorksCorrectly()
    {
        string result = _service.Execute("upper\nПривет Мир");
        Assert.Equal("ПРИВЕТ МИР", result);
    }

    [Fact]
    public void Execute_CyrillicSnakeCase_ConvertsCorrectly()
    {
        string result = _service.Execute("snake\nПривет Мир Пример");
        Assert.Equal("привет_мир_пример", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string result = _service.Execute("");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_NoTextAfterMode_ReturnsError()
    {
        string result = _service.Execute("upper");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidMode_ReturnsError()
    {
        string result = _service.Execute("invalid\nhello");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_TitleCaseWithPunctuation_PreservesSeparators()
    {
        string result = _service.Execute("title\nhello, world! example...");
        Assert.Equal("Hello, World! Example...", result);
    }

    [Fact]
    public void Execute_AlreadyCamelCase_ConvertsCorrectly()
    {
        string result = _service.Execute("camel\ncamelCase input");
        Assert.Equal("camelcaseInput", result);
    }

    [Fact]
    public void Execute_PascalCaseWithNumbers_ExtractsLettersOnly()
    {
        // Числа не являются буквами \p{L}, поэтому "hello2world" → "hello" + "world" → "HelloWorld"
        string result = _service.Execute("pascal\nhello2world");
        Assert.Equal("HelloWorld", result);
    }
}
