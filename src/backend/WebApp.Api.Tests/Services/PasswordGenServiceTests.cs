using System.Text.Json;
using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>
/// Тесты для утилиты «Генератор паролей»
/// </summary>
public class PasswordGenServiceTests
{
    private readonly PasswordGenService _service = new();

    [Fact]
    public void Execute_ValidInput_ReturnsPassword()
    {
        // Arrange
        string input = "{\"length\": 16, \"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Equal(16, result.Length);
    }

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
    public void Execute_InvalidJson_ReturnsError()
    {
        // Arrange
        string input = "not a json";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_LengthTooShort_ReturnsError()
    {
        // Arrange
        string input = "{\"length\": 3, \"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_LengthTooLong_ReturnsError()
    {
        // Arrange
        string input = "{\"length\": 300, \"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_NoCharacterSets_ReturnsError()
    {
        // Arrange
        string input = "{\"length\": 16, \"useUpper\": false, \"useLower\": false, \"useDigits\": false, \"useSymbols\": false}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_OnlyLowercase_ReturnsLowercaseOnly()
    {
        // Arrange
        string input = "{\"length\": 20, \"useUpper\": false, \"useLower\": true, \"useDigits\": false, \"useSymbols\": false}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Equal(20, result.Length);
        Assert.All(result, c => Assert.True(char.IsLower(c)));
    }

    [Fact]
    public void Execute_OnlyUppercase_ReturnsUppercaseOnly()
    {
        // Arrange
        string input = "{\"length\": 20, \"useUpper\": true, \"useLower\": false, \"useDigits\": false, \"useSymbols\": false}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Equal(20, result.Length);
        Assert.All(result, c => Assert.True(char.IsUpper(c)));
    }

    [Fact]
    public void Execute_OnlyDigits_ReturnsDigitsOnly()
    {
        // Arrange
        string input = "{\"length\": 20, \"useUpper\": false, \"useLower\": false, \"useDigits\": true, \"useSymbols\": false}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Equal(20, result.Length);
        Assert.All(result, c => Assert.True(char.IsDigit(c)));
    }

    [Fact]
    public void Execute_OnlySymbols_ReturnsSymbolsOnly()
    {
        // Arrange
        string input = "{\"length\": 20, \"useUpper\": false, \"useLower\": false, \"useDigits\": false, \"useSymbols\": true}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Equal(20, result.Length);
    }

    [Fact]
    public void Execute_DefaultLength_Returns16Chars()
    {
        // Arrange
        string input = "{\"useUpper\": true, \"useLower\": true, \"useDigits\": true, \"useSymbols\": true}";

        // Act
        string result = _service.Execute(input);

        // Assert
        Assert.Equal(16, result.Length);
    }
}