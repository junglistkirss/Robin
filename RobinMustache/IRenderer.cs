using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;

namespace RobinMustache;

public interface IRenderer<TOut>
{
    TOut Render(ImmutableArray<INode> template, object? data);
}
