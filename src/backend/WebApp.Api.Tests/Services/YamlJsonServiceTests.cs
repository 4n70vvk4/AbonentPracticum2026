using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

/// <summary>Тесты для Валидатор и конвертер JSON / YAML (yaml-json)</summary>
public class YamlJsonServiceTests
{
    private readonly YamlJsonService _service = new();

    // ============ validate ============

    [Fact]
    public void Execute_Validate_ValidJson_ReturnsSuccess()
    {
        string input = "validate\n{\"name\":\"test\",\"age\":25}";
        string result = _service.Execute(input);

        Assert.Contains("JSON корректен", result);
        // Проверяем, что pretty-print присутствует
        Assert.Contains("\"name\": \"test\"", result);
    }

    [Fact]
    public void Execute_Validate_ValidYaml_ReturnsSuccess()
    {
        string input = "validate\nname: test\nage: 25";
        string result = _service.Execute(input);

        Assert.Contains("YAML корректен", result);
    }

    [Fact]
    public void Execute_Validate_InvalidSyntax_ReturnsError()
    {
        // Незакрытый flow-токен — невалиден и для JSON, и для YAML
        string input = "validate\n[unclosed";
        string result = _service.Execute(input);

        Assert.Contains("ошибка", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Execute_Validate_EmptyInput_ReturnsError()
    {
        string result = _service.Execute("validate");
        Assert.StartsWith("Ошибка", result);
    }

    // ============ json2yaml ============

    [Fact]
    public void Execute_Json2Yaml_SimpleObject_ReturnsYaml()
    {
        string input = "json2yaml\n{\"name\":\"Иван\",\"age\":25}";
        string result = _service.Execute(input);

        Assert.Contains("name: Иван", result);
        Assert.Contains("age: 25", result);
    }

    [Fact]
    public void Execute_Json2Yaml_ExampleFromSpec_Works()
    {
        // Пример: json2yaml + {"name":"Иван","age":25} → name: Иван\nage: 25
        string input = "json2yaml\n{\"name\":\"Иван\",\"age\":25}";
        string result = _service.Execute(input);

        Assert.Contains("name: Иван", result);
        Assert.Contains("age: 25", result);
    }

    [Fact]
    public void Execute_Json2Yaml_NestedObject_Works()
    {
        string input = "json2yaml\n{\"person\":{\"name\":\"John\",\"age\":30}}";
        string result = _service.Execute(input);

        Assert.Contains("person:", result);
        Assert.Contains("name: John", result);
        Assert.Contains("age: 30", result);
    }

    [Fact]
    public void Execute_Json2Yaml_Array_Works()
    {
        string input = "json2yaml\n{\"items\":[1,2,3]}";
        string result = _service.Execute(input);

        Assert.Contains("items:", result);
        Assert.Contains("- 1", result);
        Assert.Contains("- 2", result);
        Assert.Contains("- 3", result);
    }

    [Fact]
    public void Execute_Json2Yaml_InvalidJson_ReturnsError()
    {
        string input = "json2yaml\n{invalid}";
        string result = _service.Execute(input);

        Assert.StartsWith("Ошибка", result);
    }

    // ============ yaml2json ============

    [Fact]
    public void Execute_Yaml2Json_SimpleObject_ReturnsJson()
    {
        string input = "yaml2json\nname: Иван\nage: 25";
        string result = _service.Execute(input);

        Assert.Contains("\"name\": \"Иван\"", result);
        Assert.Contains("\"age\": 25", result);
    }

    [Fact]
    public void Execute_Yaml2Json_NestedObject_Works()
    {
        string input = "yaml2json\nperson:\n  name: John\n  age: 30";
        string result = _service.Execute(input);

        Assert.Contains("\"person\"", result);
        Assert.Contains("\"name\": \"John\"", result);
        Assert.Contains("\"age\": 30", result);
    }

    [Fact]
    public void Execute_Yaml2Json_Array_Works()
    {
        string input = "yaml2json\nitems:\n  - 1\n  - 2\n  - 3";
        string result = _service.Execute(input);

        Assert.Contains("\"items\"", result);
        Assert.Contains("1", result);
        Assert.Contains("2", result);
        Assert.Contains("3", result);
    }

    [Fact]
    public void Execute_Yaml2Json_InvalidYaml_ReturnsError()
    {
        // Незакрытый flow-токен — невалидный YAML
        string input = "yaml2json\n[unclosed";
        string result = _service.Execute(input);

        Assert.StartsWith("Ошибка", result);
    }

    // ============ roundtrip & general ============

    [Fact]
    public void Execute_JsonYamlJsonRoundtrip_PreservesStructure()
    {
        string originalJson = "{\"name\":\"test\",\"value\":42}";
        string yaml = _service.Execute($"json2yaml\n{originalJson}");
        string jsonBack = _service.Execute($"yaml2json\n{yaml}");

        Assert.Contains("\"name\": \"test\"", jsonBack);
        Assert.Contains("\"value\": 42", jsonBack);
    }

    [Fact]
    public void Execute_InvalidMode_ReturnsError()
    {
        string result = _service.Execute("invalid\n{}");
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_EmptyData_ReturnsError()
    {
        string result = _service.Execute("validate");
        Assert.StartsWith("Ошибка", result);
    }
}
