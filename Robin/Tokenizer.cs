using Robin.Expressions;
using Robin.Nodes;

namespace Robin;

public static class Tokenizer
{
    public static Token[] Tokenize(this ReadOnlySpan<char> source)
    {
        List<Token> tokens = [];
        NodeLexer lexer = new(source);

        while (lexer.TryGetNextToken(out Token? token) && token is not null)
        {
            tokens.Add(token.Value);
        }

        return [.. tokens];
    }

    public static ExpressionToken[] TokenizeExpression(this ReadOnlySpan<char> source)
    {
        List<ExpressionToken> tokens = [];
        ExpressionLexer lexer = new(source);

        while (lexer.TryGetNextToken(out ExpressionToken? token) && token is not null)
        {
            tokens.Add(token.Value);
        }

        return [.. tokens];
    }
}
