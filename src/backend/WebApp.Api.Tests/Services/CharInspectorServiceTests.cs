using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

public class CharInspectorServiceTests
{
    private readonly CharInspectorService _service = new();

    [Fact]
    public void Execute_EncodeLatin_ReturnsUnicodeCodes()
    {
        string input = "encode\nHello";
        string result = _service.Execute(input);
        Assert.Equal("U+0048 U+0065 U+006C U+006C U+006F", result);
    }

    [Fact]
    public void Execute_EncodeRussian_ReturnsUnicodeCodes()
    {
        string input = "encode\nПривет!";
        string result = _service.Execute(input);
        Assert.Equal("U+041F U+0440 U+0438 U+0432 U+0435 U+0442 U+0021", result);
    }

    [Fact]
    public void Execute_DecodeHexCodes_ReturnsString()
    {
        string input = "decode\n0048 0065 006C 006C 006F";
        string result = _service.Execute(input);
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Execute_DecodeWithUPlusPrefix_ReturnsString()
    {
        string input = "decode\nU+0048 U+0065 U+006C U+006C U+006F";
        string result = _service.Execute(input);
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string result = _service.Execute("");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidMode_ReturnsError()
    {
        string result = _service.Execute("invalid\ntest");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_NoData_ReturnsError()
    {
        string result = _service.Execute("encode");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_DecodeInvalidHex_ReturnsError()
    {
        string input = "decode\nZZZZ";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_EncodeEmptyData_ReturnsError()
    {
        string result = _service.Execute("encode\n\n");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_DecodeRussianCodes_ReturnsRussianText()
    {
        string input = "decode\n041F 0440 0438 0432 0435 0442";
        string result = _service.Execute(input);
        Assert.Equal("Привет", result);
    }
}
