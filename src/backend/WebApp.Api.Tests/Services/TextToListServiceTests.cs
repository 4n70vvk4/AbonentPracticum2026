namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Текст в список (text-to-list)</summary>
public class TextToListServiceTests
{
    private readonly TextToListService _service = new();

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        // Arrange
        string input = "";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_OnlyLanguage_ReturnsError()
    {
        // Arrange
        string input = "csharp";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_ValidCSharp_ReturnsCorrectList()
    {
        // Arrange
        string input = "csharp\nяблоко\nбанан\nвишня";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("var items = new List<string>", result);
        Assert.Contains("\"яблоко\"", result);
        Assert.Contains("\"банан\"", result);
        Assert.Contains("\"вишня\"", result);
    }

    [Fact]
    public void Execute_ValidJavaScript_ReturnsCorrectArray()
    {
        // Arrange
        string input = "javascript\nяблоко\nбанан\nвишня";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("const items = [", result);
        Assert.Contains("\"яблоко\"", result);
        Assert.Contains("\"банан\"", result);
        Assert.Contains("\"вишня\"", result);
    }

    [Fact]
    public void Execute_ValidPython_ReturnsCorrectList()
    {
        // Arrange
        string input = "python\nяблоко\nбанан\nвишня";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("items = [", result);
        Assert.Contains("\"яблоко\"", result);
        Assert.Contains("\"банан\"", result);
        Assert.Contains("\"вишня\"", result);
    }

    [Fact]
    public void Execute_ValidSql_ReturnsCorrectInClause()
    {
        // Arrange
        string input = "sql\nяблоко\nбанан\nвишня";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("IN ('яблоко', 'банан', 'вишня')", result);
    }

    [Fact]
    public void Execute_UnknownLanguage_ReturnsError()
    {
        // Arrange
        string input = "rust\nяблоко\nбанан\nвишня";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("Ошибка", result);
        Assert.Contains("rust", result);
    }

    [Fact]
    public void Execute_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        string input = "csharp\nслово с пробелом\nслово \"с\" кавычками\nслова,через,запятую";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("слово с пробелом", result);
        Assert.Contains("слово \\\"с\\\" кавычками", result);
        Assert.Contains("слова,через,запятую", result);
    }

    [Fact]
    public void Execute_WithSingleQuoteForSql_HandlesCorrectly()
    {
        // Arrange
        string input = "sql\nJohn's\nJack's";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("IN ('John''s', 'Jack''s')", result);
    }
}