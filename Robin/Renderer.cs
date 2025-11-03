using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin;

public static class Renderer
{
    public static string Render(this INodeVisitor<NoValue, RenderContext> visitor, IEvaluator evaluator, ImmutableArray<INode> template, object? data)
    {
        RenderContext ctx = new()
        {
            Partials = template.ExtractsPartials(),
            Data = new DataContext(data, null),
            Evaluator = evaluator,
            Builder = new StringBuilder()
        };
        ImmutableArray<INode>.Enumerator enumerator = template.GetEnumerator();
        while (enumerator.MoveNext())
        {
            _ = enumerator.Current.Accept(visitor, ctx);
        }
        return ctx.Builder.ToString();
    }
}
