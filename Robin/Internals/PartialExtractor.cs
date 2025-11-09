using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Internals;

internal sealed class PartialExtractor : INodeVisitor<Dictionary<string, ImmutableArray<INode>>>
{
    public readonly static PartialExtractor Instance = new();
    public void VisitComment(CommentNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
    }

    public void VisitLineBreak(LineBreakNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
    }

    public void VisitPartialDefine(PartialDefineNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        if (args.ContainsKey(node.PartialName))
            args.Remove(node.PartialName);
        args.Add(node.PartialName, node.Children);
    }

    public void VisitPartialCall(PartialCallNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
    }

    public void VisitSection(SectionNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
    }

    public void VisitText(TextNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
    }

    public void VisitVariable(VariableNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
    }
}
