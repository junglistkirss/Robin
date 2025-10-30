namespace Robin.Nodes;

// Reuse previous TokenType and Token

// AST nodes
public interface INode
{
    TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args);
}
