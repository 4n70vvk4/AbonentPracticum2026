using WebApp.Api.Services;
using Xunit;

namespace WebApp.Api.Tests.Services;

public class MultiReplaceServiceTests
{
    private readonly MultiReplaceService _service = new();

    [Fact]
    public void Execute_BasicReplacement_ReplacesAllOccurrences()
    {
        string input = @"{ ""text"": ""Я люблю кофе и чай"", ""replacements"": { ""кофе"": ""программирование"", ""чай"": ""отладку"" } }";
        string result = _service.Execute(input);
        Assert.Equal("Я люблю программирование и отладку", result);
    }

    [Fact]
    public void Execute_NoMatches_ReturnsOriginalText()
    {
        string input = @"{ ""text"": ""Hello World"", ""replacements"": { ""foo"": ""bar"" } }";
        string result = _service.Execute(input);
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void Execute_AtomicReplacement_NoCascadingEffect()
    {
        // Ключевой тест: если заменить "a" на "ab", а потом "ab" на "c",
        // то строка "ab" должна стать "c", а не "cb" или "cc".
        // Замена "a" → "ab" создала бы "ab" там, где была "a",
        // и затем замена "ab" → "c" заменила бы УЖЕ ВСТАВЛЕННЫЙ "ab" —
        // это каскадный эффект. В атомарной замене такого НЕТ.
        string input = @"{ ""text"": ""ab"", ""replacements"": { ""a"": ""ab"", ""ab"": ""c"" } }";
        string result = _service.Execute(input);
        Assert.Equal("c", result);
    }

    [Fact]
    public void Execute_OverlappingKeys_LongestMatchWins()
    {
        // "aaa" содержит "aa" (длиннее) и "a" (короче).
        // При атомарной замене более длинный ключ имеет приоритет.
        // Но! "aaa" — это 3 символа. "aa" может совпасть на позициях 0-1 и 1-2.
        // Regex находит непересекающиеся слева направо: позиция 0-1 → "aa", позиция 2 → "a".
        string input = @"{ ""text"": ""aaa"", ""replacements"": { ""aa"": ""X"", ""a"": ""Y"" } }";
        string result = _service.Execute(input);
        Assert.Equal("XY", result);
    }

    [Fact]
    public void Execute_EmptyText_ReturnsError()
    {
        string input = @"{ ""text"": """", ""replacements"": { ""a"": ""b"" } }";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_EmptyReplacements_ReturnsError()
    {
        string input = @"{ ""text"": ""hello"", ""replacements"": {} }";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_InvalidJson_ReturnsError()
    {
        string input = "not-json";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_EmptyInput_ReturnsError()
    {
        string input = "";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_WhitespaceInput_ReturnsError()
    {
        string input = "   ";
        string result = _service.Execute(input);
        Assert.StartsWith("Ошибка", result);
    }

    [Fact]
    public void Execute_SpecialRegexCharacters_WorksCorrectly()
    {
        // Символы . + * ( ) [ ] { } \ | ? ^ $ не должны ломать паттерн
        string input = @"{ ""text"": ""price is 5.99 (discount 10%)"", ""replacements"": { ""5.99"": ""4.99"", ""10%"": ""20%"" } }";
        string result = _service.Execute(input);
        Assert.Equal("price is 4.99 (discount 20%)", result);
    }

    [Fact]
    public void Execute_CaseSensitive_RespectsCase()
    {
        // Замены по умолчанию чувствительны к регистру
        string input = @"{ ""text"": ""Foo foo FOO"", ""replacements"": { ""Foo"": ""Bar"" } }";
        string result = _service.Execute(input);
        Assert.Equal("Bar foo FOO", result);
    }

    [Fact]
    public void Execute_MultipleOccurrencesOfSameKey_AllReplaced()
    {
        string input = @"{ ""text"": ""a a a a"", ""replacements"": { ""a"": ""x"" } }";
        string result = _service.Execute(input);
        Assert.Equal("x x x x", result);
    }

    [Fact]
    public void Execute_ReplacementValueContainsKey_NoCascade()
    {
        // Ключ "a" → "ab", но в тексте нет "ab" изначально.
        // Результат: "b" → "ab". Вставленный "ab" НЕ заменяется повторно.
        string input = @"{ ""text"": ""b"", ""replacements"": { ""b"": ""ab"", ""ab"": ""c"" } }";
        string result = _service.Execute(input);
        Assert.Equal("ab", result);
    }

    [Fact]
    public void Execute_CamelCaseJson_Works()
    {
        // Тест с camelCase (text, replacements)
        string input = @"{ ""text"": ""abc"", ""replacements"": { ""b"": ""X"" } }";
        string result = _service.Execute(input);
        Assert.Equal("aXc", result);
    }
}
