using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Хеш-калькулятор (hash-calc)</summary>
public class HashCalcServiceTests
{
    private readonly HashCalcService _service = new();

    [Fact]
    public void Execute_Sha256_HelloWorld_ReturnsCorrectHash()
    {
        string input = "sha256\nHello, World!";
        string result = _service.Execute(input);
        Assert.Equal("dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f", result);
    }

    [Fact]
    public void Execute_Md5_HelloWorld_ReturnsCorrectHash()
    {
        string input = "md5\nHello, World!";
        string result = _service.Execute(input);
        Assert.Equal("65a8e27d8879283831b664bd8b7f0ad4", result);
    }

    [Fact]
    public void Execute_Sha1_HelloWorld_ReturnsCorrectHash()
    {
        string input = "sha1\nHello, World!";
        string result = _service.Execute(input);
        Assert.Equal("0a0a9f2a6772942557ab5355d76af442f8f65e01", result);
    }

    [Fact]
    public void Execute_Sha512_HelloWorld_ReturnsCorrectHash()
    {
        string input = "sha512\nHello, World!";
        string result = _service.Execute(input);
        Assert.Equal("374d794a95cdcfd8b35993185fef9ba368f160d8daf432d08ba9f1ed1e5abe6cc69291e0fa2fe0006a52570ef18c19def4e617c33ce52ef0a6e5fbe318cb0387", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string input = "";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_NoText_ReturnsError()
    {
        string input = "sha256";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_UnknownAlgorithm_ReturnsError()
    {
        string input = "sha999\nHello";
        string result = _service.Execute(input);
        Assert.Contains("Ошибка", result);
    }

    [Fact]
    public void Execute_AlgorithmCaseInsensitive_Works()
    {
        string input = "SHA256\nHello, World!";
        string result = _service.Execute(input);
        Assert.Equal("dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f", result);
    }

    [Fact]
    public void Execute_Sha256_EmptyString_ReturnsCorrectHash()
    {
        string input = "sha256\n";
        string result = _service.Execute(input);
        Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", result);
    }
}
