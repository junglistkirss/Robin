using Robin.Contracts.Context;
using System.Collections.Immutable;
using System.Text;

namespace Robin.Contracts.Variables;

public readonly struct AccesorPath(ImmutableArray<IAccessor> segments)
{
    public static implicit operator string(AccesorPath value) => value.ToString();
    public ImmutableArray<IAccessor> Segments { get; } = segments;
    public override string ToString()
    {
        return Segments.Aggregate(
            new StringBuilder(),
            (sb, segment) =>
            {
                return segment.Accept(InlineAccessorPrinter.Instance, sb);
            },
            sb => sb.ToString()
        );
    }

    public EvaluationResult Accept(IAccessorVisitor<EvaluationResult, DataContext> visitor, DataContext args, bool usePreviousFallback = true)
    {
        EvaluationResult result = new(ResoltionState.NotFound, null);
        DataContext ctx = args;
        ImmutableArray<IAccessor>.Enumerator enumerator = Segments.GetEnumerator();
        if (enumerator.MoveNext())
        {
            result = enumerator.Current.Accept(visitor, ctx);
            while (result.Status == ResoltionState.Found && enumerator.MoveNext())
            {
                ctx = ctx.Child(result.Value);
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
        if (usePreviousFallback && result.Status == ResoltionState.NotFound && args.Previous is not null)
        {
            return Accept(visitor, args.Previous, usePreviousFallback);
        }
        return result;
    }

}
