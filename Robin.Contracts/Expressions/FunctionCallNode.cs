using System.Collections.Immutable;

namespace Robin.Contracts.Expressions;

public readonly struct FunctionCallNode(Extract functionName, ImmutableArray<IExpressionNode> arguments) : IExpressionNode
{
    public Extract FunctionName { get; } = functionName;
    public ImmutableArray<IExpressionNode> Arguments { get; } = arguments;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitFunctionCall(this, args);
    }
}
