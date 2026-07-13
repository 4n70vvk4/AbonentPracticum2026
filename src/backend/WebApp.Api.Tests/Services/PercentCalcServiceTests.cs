using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Калькулятор пропорций и процентов (percent-calc)</summary>
public class PercentCalcServiceTests
{
    private readonly PercentCalcService _service = new();

    [Fact]
    public void Execute_PercentOf_ReturnsCorrectPercentage()
    {
        string input = "{\"operation\": \"percent-of\", \"value1\": 30, \"value2\": 200}";
        string result = _service.Execute(input);
        Assert.Equal("30 составляет 15% от 200", result);
    }

    [Fact]
    public void Execute_PercentOf_Half_Returns50Percent()
    {
        string input = "{\"operation\": \"percent-of\", \"value1\": 50, \"value2\": 100}";
        string result = _service.Execute(input);
        Assert.Equal("50 составляет 50% от 100", result);
    }

    [Fact]
    public void Execute_Change_Increase_ReturnsPositiveChange()
    {
        string input = "{\"operation\": \"change\", \"value1\": 200, \"value2\": 250}";
        string result = _service.Execute(input);
        Assert.Equal("Изменение: +25% (увеличение на 50)", result);
    }

    [Fact]
    public void Execute_Change_Decrease_ReturnsNegativeChange()
    {
        string input = "{\"operation\": \"change\", \"value1\": 250, \"value2\": 200}";
        string result = _service.Execute(input);
        Assert.Equal("Изменение: -20% (уменьшение на 50)", result);
    }

    [Fact]
    public void Execute_Change_NoChange_ReturnsZero()
    {
        string input = "{\"operation\": \"change\", \"value1\": 100, \"value2\": 100}";
        string result = _service.Execute(input);
        Assert.Equal("Изменение: +0% (изменение на 0)", result);
    }

    [Fact]
    public void Execute_Proportion_ReturnsRatio()
    {
        string input = "{\"operation\": \"proportion\", \"value1\": 10, \"value2\": 3}";
        string result = _service.Execute(input);
        Assert.Equal("10 / 3 = 3.33", result);
    }

    [Fact]
    public void Execute_Proportion_WholeNumber_ReturnsIntegerRatio()
    {
        string input = "{\"operation\": \"proportion\", \"value1\": 10, \"value2\": 5}";
        string result = _service.Execute(input);
        Assert.Equal("10 / 5 = 2", result);
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
    public void Execute_UnknownOperation_ReturnsError()
    {
        string input = "{\"operation\": \"invalid\", \"value1\": 10, \"value2\": 20}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_PercentOf_ZeroValue2_ReturnsError()
    {
        string input = "{\"operation\": \"percent-of\", \"value1\": 10, \"value2\": 0}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_Change_ZeroValue1_ReturnsError()
    {
        string input = "{\"operation\": \"change\", \"value1\": 0, \"value2\": 10}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_PercentOf_DecimalValues_ReturnsFormattedResult()
    {
        string input = "{\"operation\": \"percent-of\", \"value1\": 1, \"value2\": 3}";
        string result = _service.Execute(input);
        Assert.Equal("1 составляет 33.33% от 3", result);
    }
}
