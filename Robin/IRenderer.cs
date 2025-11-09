using Robin.Abstractions.Nodes;
using System.Collections.Immutable;

namespace Robin;

public interface IRenderer<TOut>
{
    TOut Render(ImmutableArray<INode> template, object? data);
}
