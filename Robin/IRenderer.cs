using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin;

public interface IRenderer
{
    string Render(ImmutableArray<INode> template, object? data);
}
