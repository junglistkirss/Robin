

namespace Robin.tests;

public class LexerTests
{
    [Fact]
    public void TestName()
    {
        string template = @"Hello {{name}}!
{{#items}}
  - {{title}}: {{&description}}
{{/items}}
{{! This is a comment }}
{{> footer}}";
        ReadOnlySpan<char> source = template.AsSpan();
        List<Token> tokens = Lexer.Tokenize(source);
        Assert.NotEmpty(tokens);

        Token[] txt = [.. tokens.Where(x => x.Type == TokenType.Text)];
        Assert.Equal(7, txt.Length);
        Assert.Equal("Hello ", txt[0].GetValue(source));
        Assert.Equal("!\n", txt[1].GetValue(source));
        Assert.Equal("\n  - ", txt[2].GetValue(source));
        Assert.Equal(": ", txt[3].GetValue(source));
        Assert.Equal("\n", txt[4].GetValue(source));
        Assert.Equal("\n", txt[5].GetValue(source));
        Assert.Equal("\n", txt[6].GetValue(source));

        Token[] vars = [.. tokens.Where(x => x.Type == TokenType.Variable)];
        Assert.Equal(2, vars.Length);
        Assert.Equal("name", vars[0].GetValue(source));
        Assert.Equal("title", vars[1].GetValue(source));

        Token[] uvars = [.. tokens.Where(x => x.Type == TokenType.UnescapedVariable)];
        Token s = Assert.Single(uvars);
        Assert.Equal("description", s.GetValue(source));

        Token secOpen = Assert.Single(tokens, x => x.Type == TokenType.SectionOpen);
        Assert.Equal("items", secOpen.GetValue(source));

        Token secClose = Assert.Single(tokens, x => x.Type == TokenType.SectionClose);
        Assert.Equal("items", secClose.GetValue(source));

        Token com = Assert.Single(tokens, x => x.Type == TokenType.Comment);
        Assert.Equal("This is a comment", com.GetValue(source));

        Token part = Assert.Single(tokens, x => x.Type == TokenType.Partial);
        Assert.Equal("footer", part.GetValue(source));
    }
}