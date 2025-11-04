using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin;

public interface IRenderer<TOut>
{
    TOut Render(ImmutableArray<INode> template, object? data);
}
