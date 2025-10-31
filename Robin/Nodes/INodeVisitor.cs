namespace Robin.Nodes;

public interface INodeVisitor<TOut, TArgs>
{
    TOut VisitComment(CommentNode node, TArgs args);
    TOut VisitPartial(PartialNode node, TArgs args);
    TOut VisitText(TextNode node, TArgs args);
    TOut VisitVariable(VariableNode node, TArgs args);
    TOut VisitSection(SectionNode node, TArgs args);
}
