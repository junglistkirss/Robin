using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Internals;

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

