using Robin.Contracts;
using Robin.Expressions;
using Robin.Nodes;

namespace Robin;

public static class Tokenizer
{
    public static Token[] Tokenize(this ReadOnlySpan<char> source)
    {
        List<Token> tokens = [];
        NodeLexer lexer = new(source);

        while (lexer.TryGetNextToken(out Token? token))
        {
            tokens.Add(token.Value);
        }

        return [.. tokens];
    }

    public static ExpressionToken[] TokenizeExpression(this ReadOnlySpan<char> source)
    {
        List<ExpressionToken> tokens = [];
        NodeLexer baseLexer = new(source);
        ExpressionLexer lexer = new(ref baseLexer, Extract.Full(source.Length));

        while (lexer.TryGetNextToken(out ExpressionToken? token))
        {
            tokens.Add(token.Value);
        }

        return [.. tokens];
    }
}
