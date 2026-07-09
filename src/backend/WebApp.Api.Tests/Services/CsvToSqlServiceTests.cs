using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

public class CsvToSqlServiceTests
{
    private readonly CsvToSqlService _service = new();

    [Fact]
    public void Execute_BasicExample_ReturnsCorrectSql()
    {
        string input = "employees\nname,age,city\nИван,25,Москва\nАнна,30,Питер";
        string result = _service.Execute(input);

        Assert.Contains("CREATE TEMP TABLE \"employees\"", result);
        Assert.Contains("\"name\" TEXT", result);
        Assert.Contains("\"age\" INT", result);
        Assert.Contains("\"city\" TEXT", result);
        Assert.Contains("INSERT INTO \"employees\" VALUES ('Иван', 25, 'Москва')", result);
        Assert.Contains("INSERT INTO \"employees\" VALUES ('Анна', 30, 'Питер')", result);
    }

    [Fact]
    public void Execute_DoubleType_DetectsDoublePrecision()
    {
        string input = "products\nname,price\nWidget,19.99\nGadget,5.50";
        string result = _service.Execute(input);

        Assert.Contains("\"price\" DOUBLE PRECISION", result);
        Assert.Contains("INSERT INTO \"products\" VALUES ('Widget', 19.99)", result);
        Assert.Contains("INSERT INTO \"products\" VALUES ('Gadget', 5.50)", result);
    }

    [Fact]
    public void Execute_MixedIntAndDouble_UsesDoublePrecision()
    {
        string input = "items\nid,val\n1,10.5\n2,20\n3,30.0";
        string result = _service.Execute(input);

        Assert.Contains("\"id\" INT", result);
        Assert.Contains("\"val\" DOUBLE PRECISION", result);
    }

    [Fact]
    public void Execute_AllText_AllColumnsAreText()
    {
        string input = "users\nname,email\nИван,ivan@test.com\nАнна,anna@test.com";
        string result = _service.Execute(input);

        Assert.Contains("\"name\" TEXT", result);
        Assert.Contains("\"email\" TEXT", result);
    }

    [Fact]
    public void Execute_WithQuotedCsv_HandlesQuotes()
    {
        string input = "data\nname,desc\nItem,\"Description, with comma\"\nThing,\"Quote \"\"inside\"\"\"";
        string result = _service.Execute(input);

        Assert.Contains("INSERT INTO \"data\" VALUES ('Item', 'Description, with comma')", result);
        Assert.Contains("INSERT INTO \"data\" VALUES ('Thing', 'Quote \"inside\"')", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string input = "";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_SingleLine_ReturnsError()
    {
        string input = "employees";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidTableName_ReturnsError()
    {
        string input = "123table\ncol1\nval1";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_NullValues_ProducesNull()
    {
        string input = "t\ncol1,col2\nval1,\n,val2";
        string result = _service.Execute(input);

        Assert.Contains("INSERT INTO \"t\" VALUES ('val1', NULL)", result);
        Assert.Contains("INSERT INTO \"t\" VALUES (NULL, 'val2')", result);
    }

    [Fact]
    public void Execute_TableNameWithUnderscore_Works()
    {
        string input = "my_table\ncol1\n42";
        string result = _service.Execute(input);

        Assert.Contains("CREATE TEMP TABLE \"my_table\"", result);
        Assert.Contains("INSERT INTO \"my_table\" VALUES (42)", result);
    }

    [Fact]
    public void Execute_TrailingNewlines_StillWorks()
    {
        string input = "employees\nname,age\nИван,25\nАнна,30\n\n\n";
        string result = _service.Execute(input);

        // Пустые строки в конце не должны мешать
        Assert.Contains("INSERT INTO \"employees\" VALUES ('Иван', 25)", result);
        Assert.Contains("INSERT INTO \"employees\" VALUES ('Анна', 30)", result);
    }

    [Fact]
    public void Execute_NegativeNumbers_IntType()
    {
        string input = "t\nval\n-10\n20\n-30";
        string result = _service.Execute(input);

        Assert.Contains("\"val\" INT", result);
        Assert.Contains("INSERT INTO \"t\" VALUES (-10)", result);
        Assert.Contains("INSERT INTO \"t\" VALUES (20)", result);
        Assert.Contains("INSERT INTO \"t\" VALUES (-30)", result);
    }

    [Fact]
    public void Execute_FloatingPointValues_DetectedAsDouble()
    {
        string input = "t\nval\n10.5\n20.0";
        string result = _service.Execute(input);

        Assert.Contains("\"val\" DOUBLE PRECISION", result);
        Assert.Contains("INSERT INTO \"t\" VALUES (10.5)", result);
        Assert.Contains("INSERT INTO \"t\" VALUES (20.0)", result);
    }
}
