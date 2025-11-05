using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using Robin.Nodes;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace Robin.Abstractions.Extensions;

public static class RobinExtensions
{
    //public static ImmutableDictionary<string, ImmutableArray<INode>> ExtractsPartials(this IEnumerable<INode> nodes, ReadOnlySpan<char> source, ImmutableDictionary<string, ImmutableArray<INode>>? baseCollection = null)
    //{
    //    //PartialExtractor extractor = new(valueProvider);
    //    return nodes.Aggregate(baseCollection ?? ImmutableDictionary<string, ImmutableArray<INode>>.Empty, (current, node) => node.Accept(extractor, current));
    //}

    public static bool Evaluate(this VariablePath path, IVariableSegmentVisitor<EvaluationResult, SourceContext> visitor, SourceContext args, out object? value, bool useParentFallback = true)
    {
        bool shouldFallbackOnParentContext = useParentFallback;
        SourceContext ctx = args;
        ImmutableArray<IVariableSegment>.Enumerator enumerator = path.Segments.GetEnumerator();
        if (enumerator.MoveNext())
        {
            EvaluationResult result = enumerator.Current.Accept(visitor, ctx);
            while (result.IsResolved && enumerator.MoveNext())
            {
                ctx = ctx with
                {
                    Data = ctx.Data.Child(result.Value)
                };
                IVariableSegment item = enumerator.Current;
                result = item.Accept(visitor, ctx);
                if (!result.IsResolved)
                {
                    // avoid precedence
                    value = null;
                    return true;
                }
            }
            if (result.IsResolved)
            {
                value = result.Value;
                return true;
            }
        }
        if (useParentFallback && args.Data.Parent is not null)
            return path.Evaluate(visitor, args with { Data = args.Data.Parent }, out value, useParentFallback);
        value = null;
        return false;
    }
}



