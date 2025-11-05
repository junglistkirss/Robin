using Robin.Contracts.Nodes;
using Robin.Nodes;
using System.Collections.Immutable;

namespace Robin;

public interface IRenderer<TOut>
{
    TOut Render(ImmutableArray<INode> template, string source, object? data);
}
