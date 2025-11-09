namespace Robin.Abstractions.Nodes;

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
public interface INodeVisitor<TArgs>
{
    void VisitComment(CommentNode node, TArgs args);
    void VisitPartialDefine(PartialDefineNode node, TArgs args);
    void VisitPartialCall(PartialCallNode node, TArgs args);
    void VisitText(TextNode node, TArgs args);
    void VisitLineBreak(LineBreakNode node, TArgs args);
    void VisitVariable(VariableNode node, TArgs args);
    void VisitSection(SectionNode node, TArgs args);
}