//using Robin.Abstractions.Context;
//using Robin.Contracts.Nodes;
//using Robin.Nodes;
//using System.Collections.Immutable;

//namespace Robin.Abstractions;

//internal sealed class PartialExtractor : INodeVisitor<RenderContext<T>, RenderContext<T>>
//{
//    public ImmutableDictionary<string, ImmutableArray<INode>> VisitComment(CommentNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
//    {
//        return args;
//    }

//    public ImmutableDictionary<string, ImmutableArray<INode>> VisitLineBreak(LineBreakNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
//    {
//        return args;
//    }

//    public ImmutableDictionary<string, ImmutableArray<INode>> VisitPartialDefine(PartialDefineNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
//    {
//        string partialName = valueProvider(node.PartialName);
//        if (args.ContainsKey(partialName))
//            args = args.Remove(partialName);
//        return args.Add(partialName, node.Children);
//    }

//    public ImmutableDictionary<string, ImmutableArray<INode>> VisitPartialCall(PartialCallNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
//    {
//        return args;
//    }

//    public ImmutableDictionary<string, ImmutableArray<INode>> VisitSection(SectionNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
//    {
//        return args;
//    }

//    public ImmutableDictionary<string, ImmutableArray<INode>> VisitText(TextNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
//    {
//        return args;
//    }

//    public ImmutableDictionary<string, ImmutableArray<INode>> VisitVariable(VariableNode node, ImmutableDictionary<string, ImmutableArray<INode>> args)
//    {
//        return args;
//    }
//}
