namespace Robin.Contracts.Nodes;

public interface INode
{
    TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args);
}
