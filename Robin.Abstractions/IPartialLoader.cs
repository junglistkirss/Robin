using Robin.Abstractions.Context;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Abstractions;

public interface IPartialLoader
{
    bool Load(string partialName, RenderContext context, out ImmutableArray<INode> nodes);
}
