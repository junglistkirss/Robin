using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using System.Collections.Immutable;
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
        DataContext ctx = args;
        ImmutableArray<IVariableSegment>.Enumerator enumerator = path.Segments.GetEnumerator();
        if (enumerator.MoveNext())
        {
            EvaluationResult result = enumerator.Current.Accept(visitor, ctx.Data);
            if (!result.IsResolved)
            {
                value = null;
                return false;
            }
            while (result.IsResolved && enumerator.MoveNext())
            {
                ctx = ctx.Child(result.Value);
                IVariableSegment item = enumerator.Current;
                result = item.Accept(visitor, ctx.Data);
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



