using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;

namespace RobinMustache.Abstractions;

public interface IPartialLoader
{
    bool Load(string partialName, RenderContext context, out ImmutableArray<INode> nodes);
}
