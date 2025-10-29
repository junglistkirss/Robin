namespace Robin.tests;


public class ExpressionLexerTests
{
    [Theory]
    [InlineData("test")]
    [InlineData("test ")]
    [InlineData("test  ")]
    [InlineData(" test")]
    [InlineData("  test")]
    [InlineData(" test ")]
    [InlineData("  test   ")]
    [InlineData("  test_test   ")]
    [InlineData("_test_")]
    [InlineData("_test")]
    [InlineData("test_")]
    [InlineData("__test__")]
    [InlineData("__test")]
    [InlineData("test__")]
    [InlineData("test__[0]")]
    [InlineData("test__[test]")]
    [InlineData("test__[test__]")]
    public void SimpleIdentifier(string dat)
    {
        ReadOnlySpan<char> source = dat.AsSpan();
        ExpressionToken[] tokens = Tokenizer.TokenizeExpression(source);
        Assert.NotEmpty(tokens);
        ExpressionToken secOpen = Assert.Single(tokens, x => x.Type == ExpressionType.Identifier);
        Assert.Equal(dat.Trim(), secOpen.GetValue(source));
    }

    [Theory]
    [InlineData("'12'")]
    [InlineData("'12 '")]
    [InlineData("' 12'")]
    [InlineData("' 12 '")]
    public void StringConstantSingleQuote(string dat)
    {
        ReadOnlySpan<char> source = dat.AsSpan();
        ExpressionToken[] tokens = Tokenizer.TokenizeExpression(source);
        Assert.NotEmpty(tokens);
        ExpressionToken secOpen = Assert.Single(tokens, x => x.Type == ExpressionType.Constant);
        Assert.Equal(dat.Trim('\''), secOpen.GetValue(source));
    }
    [Theory]
    [InlineData("\"12\"")]
    [InlineData("\"12 \"")]
    [InlineData("\" 12\"")]
    [InlineData("\" 12 \"")]
    public void StringConstantDoubleQuote(string dat)
    {
        ReadOnlySpan<char> source = dat.AsSpan();
        ExpressionToken[] tokens = Tokenizer.TokenizeExpression(source);
        Assert.NotEmpty(tokens);
        ExpressionToken secOpen = Assert.Single(tokens, x => x.Type == ExpressionType.Constant);
        Assert.Equal(dat.Trim('"'), secOpen.GetValue(source));
    }
    [Theory]
    [InlineData("12")]
    [InlineData("12 ")]
    [InlineData(" 12")]
    [InlineData(" 12 ")]
    public void NumberConstant(string dat)
    {
        ReadOnlySpan<char> source = dat.AsSpan();
        ExpressionToken[] tokens = Tokenizer.TokenizeExpression(source);
        Assert.NotEmpty(tokens);
        ExpressionToken secOpen = Assert.Single(tokens, x => x.Type == ExpressionType.Constant);
        Assert.Equal(dat.Trim(), secOpen.GetValue(source));
    }

    [Theory]
    [InlineData("test_complex")]
    [InlineData("test_complex[0]")]
    [InlineData("test_complex[test_complex]")]
    [InlineData("test[0]")]
    [InlineData("test[test]")]
    [InlineData("test.test")]
    [InlineData("test.test[test]")]
    public void ComplexIdentifier(string dat)
    {
        ReadOnlySpan<char> source = dat.AsSpan();
        ExpressionToken[] tokens = Tokenizer.TokenizeExpression(source);
        Assert.NotEmpty(tokens);
        ExpressionToken secOpen = Assert.Single(tokens, x => x.Type == ExpressionType.Identifier);
        Assert.Equal(dat, secOpen.GetValue(source));
    }

    [Theory]
    [InlineData(" (test) ")]
    [InlineData(" (test ) ")]
    [InlineData(" (test  ) ")]
    [InlineData(" ( test) ")]
    [InlineData(" (  test) ")]
    [InlineData(" ( test ) ")]
    [InlineData(" (  test   ) ")]
    public void IdentifierParenthesis(string dat)
    {
        ReadOnlySpan<char> source = dat.AsSpan();
        ExpressionToken[] tokens = Tokenizer.TokenizeExpression(source);
        Assert.NotEmpty(tokens);
        Assert.Equal(3, tokens.Length);
        Assert.Single(tokens, x => x.Type == ExpressionType.LeftParenthesis);
        Assert.Single(tokens, x => x.Type == ExpressionType.RightParenthesis);
        ExpressionToken secOpen = Assert.Single(tokens, x => x.Type == ExpressionType.Identifier);
        Assert.Equal("test", secOpen.GetValue(source));
    }
    [Theory]
    [InlineData("(left + right)")]
    [InlineData("(left - right)")]
    [InlineData("(left / right)")]
    [InlineData("(left * right)")]
    [InlineData("(left % right)")]
    [InlineData("(left > right)")]
    [InlineData("(left >= right)")]
    [InlineData("(left < right)")]
    [InlineData("(left <= right)")]
    [InlineData("(left && right)")]
    [InlineData("(left & right)")]
    [InlineData("(left || right)")]
    [InlineData("(left | right)")]
    public void IdentifierOperator(string dat)
    {
        ReadOnlySpan<char> source = dat.AsSpan();
        ExpressionToken[] tokens = Tokenizer.TokenizeExpression(source);
        Assert.NotEmpty(tokens);
        Assert.Equal(5, tokens.Length);
        Assert.Single(tokens, x => x.Type == ExpressionType.Operator);
        ExpressionToken[] idenitifers = [.. tokens.Where(x => x.Type == ExpressionType.Identifier)];
        Assert.Equal(2, idenitifers.Length);
        Assert.Equal("left", idenitifers[0].GetValue(source));
        Assert.Equal("right", idenitifers[1].GetValue(source));
    }
}
