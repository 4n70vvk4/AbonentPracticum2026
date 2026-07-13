using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Генератор UUID / GUID (uuid-gen)</summary>
public class UuidGenServiceTests
{
    private readonly UuidGenService _service = new();

    [Fact]
    public void Execute_DefaultCount_ReturnsOneUuid()
    {
        string result = _service.Execute("");
        var uuids = result.Split('\n');
        Assert.Single(uuids);
        Assert.Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", uuids[0]);
    }

    [Fact]
    public void Execute_Count3_Returns3Uuids()
    {
        string result = _service.Execute("3");
        var uuids = result.Split('\n');
        Assert.Equal(3, uuids.Length);
        foreach (var uuid in uuids)
        {
            Assert.Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", uuid);
        }
    }

    [Fact]
    public void Execute_AllGeneratedUuids_AreUnique()
    {
        string result = _service.Execute("10");
        var uuids = result.Split('\n');
        Assert.Equal(10, uuids.Length);
        Assert.Equal(10, uuids.Distinct().Count());
    }

    [Fact]
    public void Execute_InvalidCount_ReturnsError()
    {
        string result = _service.Execute("abc");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_NegativeCount_ReturnsError()
    {
        string result = _service.Execute("-5");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_ZeroCount_ReturnsError()
    {
        string result = _service.Execute("0");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_Count1_ReturnsValidUuid()
    {
        string result = _service.Execute("1");
        Assert.Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", result);
    }

    [Fact]
    public void Execute_UuidFormat_HasDashes()
    {
        string result = _service.Execute("1");
        Assert.Contains("-", result);
    }
}
