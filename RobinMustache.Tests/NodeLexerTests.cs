using RobinMustache.Nodes;

namespace RobinMustache.tests;

public class NodeLexerTests
{
    [Fact]
    public void TestTokenTypes()
    {
        string template = @"Hello {{ name }}!
{{#block}}
  - {{title}}: {{& description}}
{{/block}}
{{^revert}}Missing item{{/revert}}
{{! This is a comment }}";
        ReadOnlySpan<char> source = template.AsSpan();
        Token[] tokens = Tokenizer.Tokenize(source);
        Assert.NotEmpty(tokens);

        Token[] br = [.. tokens.Where(x => x.Type == TokenType.LineBreak)];
        Assert.Equal(5, br.Length);

        Token[] txt = [.. tokens.Where(x => x.Type == TokenType.Text)];
        Assert.Equal(5, txt.Length);
        Assert.Equal("Hello ", txt[0].GetValue(source));
        Assert.Equal($"!", txt[1].GetValue(source));
        Assert.Equal($"  - ", txt[2].GetValue(source));
        Assert.Equal(": ", txt[3].GetValue(source));
        Assert.Equal("Missing item", txt[4].GetValue(source));

        Token[] vars = [.. tokens.Where(x => x.Type == TokenType.Variable)];
        Assert.Equal(2, vars.Length);
        Assert.Equal("name", vars[0].GetValue(source));
        Assert.Equal("title", vars[1].GetValue(source));

        Token[] uvars = [.. tokens.Where(x => x.Type == TokenType.UnescapedVariable)];
        Token s = Assert.Single(uvars);
        Assert.Equal("description", s.GetValue(source));

        Token secOpen = Assert.Single(tokens, x => x.Type == TokenType.SectionOpen);
        Assert.Equal("block", secOpen.GetValue(source));

        Token invSecOpen = Assert.Single(tokens, x => x.Type == TokenType.InvertedSection);
        Assert.Equal("revert", invSecOpen.GetValue(source));

        Token[] closes = [.. tokens.Where(x => x.Type == TokenType.SectionClose)];
        Assert.Equal(2, closes.Length);
        Assert.Equal("block", closes[0].GetValue(source));
        Assert.Equal("revert", closes[1].GetValue(source));

        Token com = Assert.Single(tokens, x => x.Type == TokenType.Comment);
        Assert.Equal("This is a comment", com.GetValue(source));
    }

    [Theory]
    [InlineData("test1 | test2")]
    [InlineData("test1 || test2")]
    [InlineData("test1 & test2")]
    [InlineData("test1 && test2")]
    public void VariableTest(string dat)
    {
        string template = "{{" + dat + "}}";
        ReadOnlySpan<char> source = template.AsSpan();
        Token[] tokens = Tokenizer.Tokenize(source);
        Assert.NotEmpty(tokens);
        Token secOpen = Assert.Single(tokens, x => x.Type == TokenType.Variable);
        Assert.Equal(dat, secOpen.GetValue(source));
    }

    [Theory]
    [InlineData("test1 | test2")]
    [InlineData("test1 || test2")]
    [InlineData("test1 & test2")]
    [InlineData("test1 && test2")]
    public void UnescapedVariableTest(string dat)
    {
        string template = "{{{" + dat + "}}}";
        ReadOnlySpan<char> source = template.AsSpan();
        Token[] tokens = Tokenizer.Tokenize(source);
        Assert.NotEmpty(tokens);
        Token secOpen = Assert.Single(tokens, x => x.Type == TokenType.UnescapedVariable);
        Assert.Equal(dat, secOpen.GetValue(source));
    }
    [Fact]
    public void TestSectionHelperTokenTypes()
    {
        string template = "{{#if test}}plouf{{/if}}";
        ReadOnlySpan<char> source = template.AsSpan();
        Token[] tokens = Tokenizer.Tokenize(source);
        Assert.NotEmpty(tokens);
        Token secOpen = Assert.Single(tokens, x => x.Type == TokenType.SectionOpen);
        Assert.Equal("if test", secOpen.GetValue(source));
    }

    [Fact]
    public void TestSectionHelperTokenTypes2()
    {
        string template = "{{#if test}}OK{{^else}}{{/if}}";
        ReadOnlySpan<char> source = template.AsSpan();
        Token[] tokens = Tokenizer.Tokenize(source);
        Assert.NotEmpty(tokens);
        Token secIf = Assert.Single(tokens, x => x.Type == TokenType.SectionOpen);
        Assert.Equal("if test", secIf.GetValue(source));
        Token secElse = Assert.Single(tokens, x => x.Type == TokenType.InvertedSection);
        Assert.Equal("else", secElse.GetValue(source));
    }
}