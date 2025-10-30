namespace Robin.Nodes;

public interface INodeVisitor<TOut, TArgs>
{
    TOut VisitTextNode(TextNode node, TArgs args);
    TOut VisitVariableNode(VariableNode node, TArgs args);
    TOut VisitSectionNode(SectionNode node, TArgs args);
}
