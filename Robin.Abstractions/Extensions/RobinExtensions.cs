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

    public static bool Evaluate(this VariablePath path, IVariableSegmentVisitor<EvaluationResult, object?> visitor, DataContext args, out object? value, bool useParentFallback = true)
    {
        bool shouldFallbackOnParentContext = useParentFallback;
        DataContext ctx = args;
        int limit = path.Segments.Length;
        if (limit > 0)
        {
            int i = 0;
            IVariableSegment current = path.Segments[i];
            EvaluationResult result = current.Accept(visitor, ctx.Data);
            i++;
            while (result.IsResolved && i < limit)
            {
                ctx = ctx.Child(result.Value);
                current = path.Segments[i]; ;
                result = current.Accept(visitor, ctx.Data);
                if (!result.IsResolved)
                {
                    // avoid precedence
                    value = null;
                    return true;
                }
                i++;
            }
            if (result.IsResolved)
            {
                value = result.Value;
                return true;
            }
        }
        if (useParentFallback && args.Parent is not null)
            return path.Evaluate(visitor, args.Parent, out value, useParentFallback);
        value = null;
        return false;
    }
}



