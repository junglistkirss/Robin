using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Internals;

internal sealed class RendererImpl<T, TOut>(
    T defaultBuilder,
    Func<T, TOut> output,
    INodeVisitor<NoValue, RenderContext<T>> visitor,
    IEvaluator evaluator,
    Action<Helper>? helperConfig = null
    ) : IRenderer<TOut>
    where T : class
{
    public TOut Render(ImmutableArray<INode> template, object? data)
    {
        T result = defaultBuilder.Render(visitor, evaluator, template, data, helperConfig);
        return output(result);
    }
}
