using System.Collections.Immutable;

namespace Robin.Expressions;

public readonly struct FunctionCallNode(string functionName, ImmutableArray<IExpressionNode> arguments) : IExpressionNode
{
    public string FunctionName { get; } = functionName;
    public ImmutableArray<IExpressionNode> Arguments { get; } = arguments;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitFunctionCall(this, args);
    }
}
