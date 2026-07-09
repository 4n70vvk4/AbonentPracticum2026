using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

public class StringSortServiceTests
{
    private readonly StringSortService _service = new();

    [Fact]
    public void Execute_Asc_SortsAlphabetically()
    {
        string input = "asc\nарбуз\nабрикос\nбанан";
        string result = _service.Execute(input);
        Assert.Equal("абрикос\nарбуз\nбанан", result);
    }

    [Fact]
    public void Execute_Desc_SortsDescending()
    {
        string input = "desc\nарбуз\nабрикос\nбанан";
        string result = _service.Execute(input);
        Assert.Equal("банан\nарбуз\nабрикос", result);
    }

    [Fact]
    public void Execute_LengthAsc_SortsByLength()
    {
        string input = "length-asc\nяблоко\nвишня\nай";
        string result = _service.Execute(input);
        Assert.Equal("ай\nвишня\nяблоко", result);
    }

    [Fact]
    public void Execute_LengthDesc_SortsByLengthDescending()
    {
        string input = "length-desc\nай\nвишня\nяблоко";
        string result = _service.Execute(input);
        Assert.Equal("яблоко\nвишня\nай", result);
    }

    [Fact]
    public void Execute_Unique_RemovesDuplicates()
    {
        string input = "unique\nяблоко\nбанан\nяблоко\nвишня\nбанан";
        string result = _service.Execute(input);
        Assert.Equal("яблоко\nбанан\nвишня", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string result = _service.Execute("");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_NoItems_ReturnsError()
    {
        string result = _service.Execute("asc");
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_UnknownMode_ReturnsError()
    {
        string input = "invalid\nabc\nxyz";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_Asc_EnglishStrings_SortsCorrectly()
    {
        string input = "asc\nbanana\napple\ncherry";
        string result = _service.Execute(input);
        Assert.Equal("apple\nbanana\ncherry", result);
    }

    [Fact]
    public void Execute_UniqueWithNoDuplicates_ReturnsSameOrder()
    {
        string input = "unique\nтретий\nпервый\nвторой";
        string result = _service.Execute(input);
        Assert.Equal("третий\nпервый\nвторой", result);
    }
}
