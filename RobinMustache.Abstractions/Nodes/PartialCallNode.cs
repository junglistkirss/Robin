using RobinMustache.Abstractions.Expressions;
using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Nodes;

public sealed class PartialCallNode(string name, IExpressionNode expression) : INode
{
    public string PartialName { get; } = name;
    public IExpressionNode Expression { get; } = expression;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitPartialCall(this, args);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitPartialCall(this, args);
    }
}