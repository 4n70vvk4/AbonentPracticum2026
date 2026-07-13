using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Base64 кодер / декодер (base64)</summary>
public class Base64ServiceTests
{
    private readonly Base64Service _service = new();

    [Fact]
    public void Execute_EncodeSimpleText_ReturnsBase64()
    {
        string input = "encode\nHello, World!";
        string result = _service.Execute(input);
        Assert.Equal("SGVsbG8sIFdvcmxkIQ==", result);
    }

    [Fact]
    public void Execute_EncodeRussianText_ReturnsBase64()
    {
        string input = "encode\nПривет, мир!";
        string result = _service.Execute(input);
        Assert.Equal("0J/RgNC40LLQtdGCLCDQvNC40YAh", result);
    }

    [Fact]
    public void Execute_DecodeBase64_ReturnsOriginalText()
    {
        string input = "decode\nSGVsbG8sIFdvcmxkIQ==";
        string result = _service.Execute(input);
        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public void Execute_DecodeRussianBase64_ReturnsOriginalText()
    {
        string input = "decode\n0J/RgNC40LLQtdGCLCDQvNC40YAh";
        string result = _service.Execute(input);
        Assert.Equal("Привет, мир!", result);
    }

    [Fact]
    public void Execute_EncodeEmptyData_ReturnsError()
    {
        string input = "encode";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_DecodeEmptyData_ReturnsError()
    {
        string input = "decode";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string input = "";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidMode_ReturnsError()
    {
        string input = "invalid\nsome data";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_DecodeInvalidBase64_ReturnsError()
    {
        string input = "decode\nnot-a-base64-string!!!";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_EncodeMultilineText_ReturnsBase64()
    {
        string input = "encode\nLine1\nLine2\nLine3";
        string result = _service.Execute(input);
        Assert.Equal("TGluZTEKTGluZTIKTGluZTM=", result);
    }

    [Fact]
    public void Execute_DecodeMultilineBase64_ReturnsOriginalText()
    {
        string input = "decode\nTGluZTEKTGluZTIKTGluZTM=";
        string result = _service.Execute(input);
        Assert.Equal("Line1\nLine2\nLine3", result);
    }

    [Fact]
    public void Execute_EncodeWithWhitespaceMode_Works()
    {
        string input = "  encode  \nHello";
        string result = _service.Execute(input);
        Assert.Equal("SGVsbG8=", result);
    }

    [Fact]
    public void Execute_DecodeRoundtrip_ReturnsSameText()
    {
        string original = "The quick brown fox jumps over the lazy dog.";
        string encoded = _service.Execute($"encode\n{original}");
        string decoded = _service.Execute($"decode\n{encoded}");
        Assert.Equal(original, decoded);
    }
}
