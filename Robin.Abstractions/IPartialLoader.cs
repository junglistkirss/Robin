using Robin.Abstractions.Context;
using Robin.Abstractions.Nodes;
using System.Collections.Immutable;

namespace Robin.Abstractions;

public interface IPartialLoader
{
    bool Load(string partialName, RenderContext context, out ImmutableArray<INode> nodes);
}
