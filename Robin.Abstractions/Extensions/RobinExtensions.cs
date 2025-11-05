using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace Robin.Abstractions.Extensions;

public static class RobinExtensions
{
    public static ImmutableDictionary<string, ImmutableArray<INode>> ExtractsPartials(this IEnumerable<INode> nodes, ImmutableDictionary<string, ImmutableArray<INode>>? baseCollection = null)
    {
        return nodes.Aggregate(baseCollection ?? ImmutableDictionary<string, ImmutableArray<INode>>.Empty, (current, node) => node.Accept(PartialExtractor.Instance, current));
    }

}



