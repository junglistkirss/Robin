using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Helpers;
using RobinMustache.Abstractions.Nodes;
using RobinMustache.Extensions;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace RobinMustache.Internals;

internal sealed class RendererImpl<T, TOut>(
    Func<T> builderFactory,
    Func<T, TOut> output,
    INodeVisitor<RenderContext<T>> visitor,
    IEvaluator evaluator,
    Action<Helper>? helperConfig = null
    ) : IRenderer<TOut>
    where T : class
{
    public TOut Render(ImmutableArray<INode> template, object? data)
    {
        ReadOnlySpan<INode> templateReadOnly = template.AsSpan();
        ReadOnlyDictionary<string, ImmutableArray<INode>> partials = new(templateReadOnly.ExtractsPartials()); // calculer une seule fois
        using (DataContext.Push(data))
        {
            helperConfig?.Invoke(DataContext.Current.Helper);
            T builderInstance = builderFactory();
            RenderContext<T> ctx = RenderContextPool<T>.Get(evaluator, builderInstance, partials);
            try
            {
                foreach (INode item in templateReadOnly)
                    item.Accept(visitor, ctx);
            }
            finally
            {
                RenderContextPool<T>.Return(ctx); // remet dans le pool
            }
            return output(builderInstance);
        }
    }
}
