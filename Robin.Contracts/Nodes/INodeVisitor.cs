namespace Robin.Contracts.Nodes;

public interface INodeVisitor<TOut, TArgs>
{
    TOut VisitComment(CommentNode node, TArgs args);
    TOut VisitPartialDefine(PartialDefineNode node, TArgs args);
    TOut VisitPartialCall(PartialCallNode node, TArgs args);
    TOut VisitText(TextNode node, TArgs args);
    TOut VisitLineBreak(LineBreakNode node, TArgs args);
    TOut VisitVariable(VariableNode node, TArgs args);
    TOut VisitSection(SectionNode node, TArgs args);
}
