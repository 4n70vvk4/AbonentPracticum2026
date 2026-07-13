using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Генератор паролей (password-gen)</summary>
public class PasswordGenServiceTests
{
    private readonly PasswordGenService _service = new();

    [Fact]
    public void Execute_ValidInput_ReturnsPassword()
    {
        string input = "{\"length\": 16, \"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";
        string result = _service.Execute(input);
        Assert.Equal(16, result.Length);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string input = "";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidJson_ReturnsError()
    {
        string input = "not a json";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_LengthTooShort_ReturnsError()
    {
        string input = "{\"length\": 3, \"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_LengthTooLong_ReturnsError()
    {
        string input = "{\"length\": 300, \"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_NoCharacterSets_ReturnsError()
    {
        string input = "{\"length\": 16, \"useUpper\": false, \"useLower\": false, \"useDigits\": false, \"useSymbols\": false}";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_OnlyLowercase_ReturnsLowercaseOnly()
    {
        string input = "{\"length\": 20, \"useUpper\": false, \"useLower\": true, \"useDigits\": false, \"useSymbols\": false}";
        string result = _service.Execute(input);
        Assert.Equal(20, result.Length);
        Assert.All(result, c => Assert.True(char.IsLower(c)));
    }

    [Fact]
    public void Execute_OnlyUppercase_ReturnsUppercaseOnly()
    {
        string input = "{\"length\": 20, \"useUpper\": true, \"useLower\": false, \"useDigits\": false, \"useSymbols\": false}";
        string result = _service.Execute(input);
        Assert.Equal(20, result.Length);
        Assert.All(result, c => Assert.True(char.IsUpper(c)));
    }

    [Fact]
    public void Execute_OnlyDigits_ReturnsDigitsOnly()
    {
        string input = "{\"length\": 20, \"useUpper\": false, \"useLower\": false, \"useDigits\": true, \"useSymbols\": false}";
        string result = _service.Execute(input);
        Assert.Equal(20, result.Length);
        Assert.All(result, c => Assert.True(char.IsDigit(c)));
    }

    [Fact]
    public void Execute_OnlySymbols_ReturnsSymbolsOnly()
    {
        string input = "{\"length\": 20, \"useUpper\": false, \"useLower\": false, \"useDigits\": false, \"useSymbols\": true}";
        string result = _service.Execute(input);
        Assert.Equal(20, result.Length);
    }

    [Fact]
    public void Execute_DefaultLength_Returns16Chars()
    {
        string input = "{\"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";
        string result = _service.Execute(input);
        Assert.Equal(16, result.Length);
    }
}