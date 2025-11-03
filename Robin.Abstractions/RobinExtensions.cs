using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using System.Collections.Immutable;

namespace Robin.Abstractions;

public static class RobinExtensions
{
    public static ImmutableDictionary<string, ImmutableArray<INode>> ExtractsPartials(this IEnumerable<INode> nodes, ImmutableDictionary<string, ImmutableArray<INode>>? baseCollection = null)
    {
        return nodes.Aggregate(baseCollection ?? ImmutableDictionary<string, ImmutableArray<INode>>.Empty, (current, node) => node.Accept(PartialExtractor.Instance, current));
    }
    public static EvaluationResult Evaluate(this VariablePath path, IVariableSegmentVisitor<EvaluationResult, object?> visitor, DataContext args, bool useParentFallback = true)
    {
        EvaluationResult result = new(ResoltionState.NotFound, DataFacade.Null);
        DataContext ctx = args;
        ImmutableArray<IVariableSegment>.Enumerator enumerator = path.Segments.GetEnumerator();
        if (enumerator.MoveNext())
        {
            result = enumerator.Current.Accept(visitor, ctx.Data);
            while (result.Status == ResoltionState.Found && enumerator.MoveNext())
            {
                ctx = ctx.Child(result.Value?.RawValue);
                IVariableSegment item = enumerator.Current;
                EvaluationResult res = item.Accept(visitor, ctx.Data);
                if (res.Status == ResoltionState.Found)
                {
                    result = res;
                }
                else
                {
                    result = result with { Status = ResoltionState.Partial };
                }
            }
        }
        if (useParentFallback && result.Status == ResoltionState.NotFound && args.Parent is not null)
        {
            return path.Evaluate(visitor, args.Parent, useParentFallback);
        }
        return result;
    }
}



