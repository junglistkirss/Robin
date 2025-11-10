using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;

namespace RobinMustache.Internals;

internal sealed class DefinedPartialLoader : IPartialLoader
{
    public bool Load(string partialName, RenderContext context, out ImmutableArray<INode> nodes)
    {
        if (context.Partials is not null)
            return context.Partials.TryGetValue(partialName, out nodes);
        nodes = [];
        return false;
    }
}

