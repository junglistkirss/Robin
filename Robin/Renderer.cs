using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Abstractions.Nodes;
using Robin.Extensions;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Robin;

public static class Renderer
{
    public static void Render<T>(
        this T defaultBuilder,
        INodeVisitor<RenderContext<T>> visitor,
        IEvaluator evaluator,
        ReadOnlySpan<INode> template,
        object? data,
        Action<Helper>? helperConfig = null)
        where T : class
    {
        ReadOnlyDictionary<string, ImmutableArray<INode>> partials = new(template.ExtractsPartials()); // calculer une seule fois
        using (DataContext.Push(data))
        {
            helperConfig?.Invoke(DataContext.Current.Helper);
            RenderContext<T> ctx = RenderContextPool<T>.Get(evaluator, defaultBuilder, partials);
            try
            {
                foreach (INode item in template)
                    item.Accept(visitor, ctx);
            }
            finally
            {
                RenderContextPool<T>.Return(ctx); // remet dans le pool
            }
        }
    }

    //public static string RenderString(this IEvaluator evaluator, ImmutableArray<INode> template, object? data, Action<Helper>? helperConfig = null)
    //{
    //    StringBuilder sb = new();
    //    Render(sb, StringNodeRender.Instance, evaluator, template.AsSpan(), data, helperConfig);
    //    return sb.ToString();
    //}
}
