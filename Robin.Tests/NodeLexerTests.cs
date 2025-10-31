using Robin.Nodes;

namespace Robin.tests;
public class NodeLexerTests
{
    [Fact]
    public void TestTokenTypes()
    {
        string template = @"Hello {{ name }}!
{{#items}}
  - {{title}}: {{& description}}
{{^items}}Missing item{{/items}}
{{! This is a comment }}
{{> footer}}";
        ReadOnlySpan<char> source = template.AsSpan();
        Token[] tokens = Tokenizer.Tokenize(source);
        Assert.NotEmpty(tokens);

        Token[] txt = [.. tokens.Where(x => x.Type == TokenType.Text)];
        Assert.Equal(8, txt.Length);
        Assert.Equal("Hello ", txt[0].GetValue(source));
        Assert.Equal($"!{Environment.NewLine}", txt[1].GetValue(source));
        Assert.Equal($"{Environment.NewLine}  - ", txt[2].GetValue(source));
        Assert.Equal(": ", txt[3].GetValue(source));
        Assert.Equal(Environment.NewLine, txt[4].GetValue(source));
        Assert.Equal("Missing item", txt[5].GetValue(source));
        Assert.Equal(Environment.NewLine, txt[6].GetValue(source));
        Assert.Equal(Environment.NewLine, txt[7].GetValue(source));

        Token[] vars = [.. tokens.Where(x => x.Type == TokenType.Variable)];
        Assert.Equal(2, vars.Length);
        Assert.Equal("name", vars[0].GetValue(source));
        Assert.Equal("title", vars[1].GetValue(source));

        Token[] uvars = [.. tokens.Where(x => x.Type == TokenType.UnescapedVariable)];
        Token s = Assert.Single(uvars);
        Assert.Equal("description", s.GetValue(source));

        Token secOpen = Assert.Single(tokens, x => x.Type == TokenType.SectionOpen);
        Assert.Equal("items", secOpen.GetValue(source));

        Token invSecOpen = Assert.Single(tokens, x => x.Type == TokenType.InvertedSection);
        Assert.Equal("items", invSecOpen.GetValue(source));

        Token secClose = Assert.Single(tokens, x => x.Type == TokenType.SectionClose);
        Assert.Equal("items", secClose.GetValue(source));

        Token com = Assert.Single(tokens, x => x.Type == TokenType.Comment);
        Assert.Equal("This is a comment", com.GetValue(source));

        Token part = Assert.Single(tokens, x => x.Type == TokenType.Partial);
        Assert.Equal("footer", part.GetValue(source));
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