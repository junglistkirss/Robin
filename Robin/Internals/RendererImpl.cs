using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Abstractions.Nodes;
using System.Collections.Immutable;

namespace Robin.Internals;

internal sealed class RendererImpl<T, TOut>(
    T builder,
    Func<T, TOut> output,
    INodeVisitor<RenderContext<T>> visitor,
    IEvaluator evaluator,
    Action<Helper>? helperConfig = null
    ) : IRenderer<TOut>
    where T : class
{
    public TOut Render(ImmutableArray<INode> template, object? data)
    {
        builder.Render(visitor, evaluator, template.AsSpan(), data, helperConfig);
        return output(builder);
    }
}
