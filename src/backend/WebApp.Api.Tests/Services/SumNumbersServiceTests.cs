namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Сумма чисел (sum-numbers)</summary>
public class SumNumbersServiceTests
{
    private readonly SumNumbersService _service = new();

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_EmptyInput_ReturnsZero()
    {
        var result = _service.Execute("");
        Assert.Equal("0", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_WhitespaceInput_ReturnsZero()
    {
        var result = _service.Execute("   \n  \n  ");
        Assert.Equal("0", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_SingleNumber_ReturnsThatNumber()
    {
        var result = _service.Execute("42");
        Assert.Contains("42", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_MultipleNumbers_ReturnsSum()
    {
        var result = _service.Execute("10\n20\n30");
        Assert.Contains("60", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_DecimalNumbers_ReturnsCorrectSum()
    {
        var result = _service.Execute("1.5\n2.5\n3.0");
        Assert.Contains("7", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_NegativeNumbers_ReturnsCorrectSum()
    {
        var result = _service.Execute("10\n-5\n-3");
        Assert.Contains("2", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_InvalidInput_ShowsError()
    {
        var result = _service.Execute("10\nabc\n20");
        Assert.Contains("30", result);         // 10 + 20 = 30 (abc пропущена)
        Assert.Contains("abc", result);         // упоминание некорректной строки
        Assert.Contains("не число", result);    // сообщение об ошибке
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_AllInvalid_ReturnsZero()
    {
        var result = _service.Execute("abc\ndef\nghi");
        Assert.Contains("Сумма: 0", result);
        Assert.Contains("Пропущены строки", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_MixedLineEndings_WorksCorrectly()
    {
        // Разные разделители строк (\n, \r\n)
        var result = _service.Execute("1\n2\r\n3");
        Assert.Contains("6", result);
    }

    [Fact]
    [Trait("Category", "Example")]
    public void Execute_LargeNumbers_HandlesCorrectly()
    {
        var result = _service.Execute("999999999\n1");
        Assert.Contains("1000000000", result);
    }
}
