using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Nodes;
using RobinMustache.Expressions;
using RobinMustache.Nodes;
using System.Collections.Immutable;

namespace RobinMustache;

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
