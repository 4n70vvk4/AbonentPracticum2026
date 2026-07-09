using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

public class UnixTimeServiceTests
{
    private readonly UnixTimeService _service = new();

    [Fact]
    public void Execute_ToDate_ValidTimestamp_ReturnsDate()
    {
        string input = "to-date\n1785963600";
        string result = _service.Execute(input);
        Assert.Equal("05.08.2026 21:00:00 (UTC)", result);
    }

    [Fact]
    public void Execute_ToDate_ZeroTimestamp_ReturnsEpoch()
    {
        string input = "to-date\n0";
        string result = _service.Execute(input);
        Assert.Equal("01.01.1970 00:00:00 (UTC)", result);
    }

    [Fact]
    public void Execute_ToDate_EpochTimestamp_ReturnsCorrectDate()
    {
        string input = "to-date\n1718064000";
        string result = _service.Execute(input);
        Assert.Equal("11.06.2024 00:00:00 (UTC)", result);
    }

    [Fact]
    public void Execute_ToTimestamp_ValidDate_ReturnsTimestamp()
    {
        string input = "to-timestamp\n05.08.2026 21:00:00";
        string result = _service.Execute(input);
        Assert.Equal("1785963600", result);
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
        string result = _service.Execute("invalid\n12345");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_NoValue_ReturnsError()
    {
        string result = _service.Execute("to-date");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidTimestamp_ReturnsError()
    {
        string result = _service.Execute("to-date\nabc");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidDateString_ReturnsError()
    {
        string result = _service.Execute("to-timestamp\nnot-a-date");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_Roundtrip_ReturnsOriginal()
    {
        long original = 1718064000;
        var dateResult = _service.Execute($"to-date\n{original}");
        var timestampStr = dateResult.Split(" (")[0];
        var backResult = _service.Execute($"to-timestamp\n{timestampStr}");
        Assert.Equal(original.ToString(), backResult);
    }
}
