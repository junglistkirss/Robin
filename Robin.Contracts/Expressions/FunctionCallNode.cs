using System.Collections.Immutable;

namespace Robin.Contracts.Expressions;

public sealed class FunctionCallNode(string functionName, ImmutableArray<IExpressionNode> arguments) : IExpressionNode
{
    public string FunctionName { get; } = functionName;
    public ImmutableArray<IExpressionNode> Arguments { get; } = arguments;

    public bool Accept<TArgs>(IExpressionNodeVisitor<TArgs> visitor, TArgs args, out object? value)
    {
        return visitor.VisitFunctionCall(this, args, out value);
    }
}
