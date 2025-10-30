using Robin.Nodes.Expressions;

namespace Robin;

public static class Tokenizer
{
    public static Token[] Tokenize(this ReadOnlySpan<char> source)
    {
        List<Token> tokens = [];
        Lexer lexer = new(source);

        while (lexer.TryGetNextToken(out Token? token))
        {
            tokens.Add(token.Value);
        }

        return [.. tokens];
    }

    public static ExpressionToken[] TokenizeExpression(this ReadOnlySpan<char> source)
    {
        List<ExpressionToken> tokens = [];
        ExpressionLexer lexer = new(source);

        while (lexer.TryGetNextToken(out ExpressionToken? token))
        {
            tokens.Add(token.Value);
        }

        return [.. tokens];
    }
}