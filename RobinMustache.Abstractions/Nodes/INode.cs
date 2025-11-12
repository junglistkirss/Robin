namespace RobinMustache.Abstractions.Nodes;

public interface INode
{
    TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args);
    void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args);
}
