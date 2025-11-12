using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Nodes;

public sealed class LineBreakNode : INode
{

    public readonly static LineBreakNode InstanceReturn = new("\r");
    public readonly static LineBreakNode InstanceLine= new("\n");
    public readonly static LineBreakNode Instance = new("\r\n");
    private LineBreakNode(string content)
    {
        Content = content;
    }

    public string Content { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitLineBreak(this, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitLineBreak(this, args);
    }
}
