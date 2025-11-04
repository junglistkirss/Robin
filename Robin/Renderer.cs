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
    public static IEnumerable<TOut> Accept<TIn, TOut>(this ImmutableArray<INode> template, INodeVisitor<TOut, TIn> visitor, TIn args)
    {
        ImmutableArray<INode>.Enumerator enumerator = template.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current.Accept(visitor, args);
        }
    }

    public static T Render<T>(this T defaultBuilder, INodeVisitor<NoValue, RenderContext<T>> visitor, IEvaluator evaluator, ImmutableArray<INode> template, object? data, Action<Helper>? helperConfig = null)
        where T : class
    {
        DataContext dataContext = new(data, null);
        helperConfig?.Invoke(dataContext.Helper);
        RenderContext<T> ctx = new()
        {
            Partials = template.ExtractsPartials(),
            Data = dataContext,
            Evaluator = evaluator,
            Builder = defaultBuilder
        };
        ImmutableArray<INode>.Enumerator enumerator = template.GetEnumerator();
        while (enumerator.MoveNext())
        {
            _ = enumerator.Current.Accept(visitor, ctx);
        }
        return defaultBuilder;
    }

    public static string RenderString(this IEvaluator evaluator, ImmutableArray<INode> template, object? data, Action<Helper>? helperConfig = null)
    {
        StringBuilder sb = new();
        Render(sb, StringNodeRender.Instance, evaluator, template, data, helperConfig);
        return sb.ToString();
    }
}
