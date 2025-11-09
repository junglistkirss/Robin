using Robin.Abstractions.Expressions;
using Robin.Abstractions.Nodes;
using Robin.Expressions;
using Robin.Nodes;
using System.Collections.Immutable;

namespace Robin;

public static class Parser
{
    public static ImmutableArray<INode> Parse(this ReadOnlySpan<char> source)
    {
        NodeLexer lexer = new(source);
        ImmutableArray<INode> nodes = lexer.Parse();
        return nodes;
    }

    public static IExpressionNode? ParseExpression(this ReadOnlySpan<char> source)
    {
        ExpressionLexer lexer = new(source);
        IExpressionNode? node = lexer.Parse();
        return node;
    }
}
