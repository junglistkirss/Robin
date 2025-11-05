using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Abstractions;

internal sealed class PartialExtractor : INodeVisitor<ImmutableDictionary<string, ImmutableArray<INode>>, ImmutableDictionary<string, ImmutableArray<INode>>>
{
    public readonly static PartialExtractor Instance = new();
    public ImmutableDictionary<string, ImmutableArray<INode>> VisitComment(CommentNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitLineBreak(LineBreakNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitPartialDefine(PartialDefineNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
    {
        if (args.ContainsKey(node.PartialName))
            args = args.Remove(node.PartialName);
        return args.Add(node.PartialName, node.Children);
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitPartialCall(PartialCallNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitSection(SectionNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitText(TextNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitVariable(VariableNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }
}
