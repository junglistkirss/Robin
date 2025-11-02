using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Collections.Immutable;

namespace Robin.Abstractions;

public static class VariablePathExtensions
{
    public static EvaluationResult Evaluate(this VariablePath path, IAccessorVisitor<EvaluationResult, DataContext> visitor, DataContext args, bool useParentFallback = true)
    {
        EvaluationResult result = new(ResoltionState.NotFound, DataFacade.Null);
        DataContext ctx = args;
        ImmutableArray<IAccessor>.Enumerator enumerator = path.Segments.GetEnumerator();
        if (enumerator.MoveNext())
        {
            result = enumerator.Current.Accept(visitor, ctx);
            while (result.Status == ResoltionState.Found && enumerator.MoveNext())
            {
                ctx = ctx.Child(result.Value?.RawValue);
                IAccessor item = enumerator.Current;
                EvaluationResult res = item.Accept(visitor, ctx);
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



