using Microsoft.Extensions.DependencyInjection.Extensions;
using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin;

public static class Renderer
{
    public static string Render(this INodeVisitor<NoValue, RenderContext> visitor, IEvaluator evaluator, ImmutableArray<INode> template, object? data, Action<Helper>? helperConfig = null)
    {
        DataContext dataContext = new(data, null);
        helperConfig?.Invoke(dataContext.Helper);
        RenderContext ctx = new()
        {
            Partials = template.ExtractsPartials(),
            Data = dataContext,
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
