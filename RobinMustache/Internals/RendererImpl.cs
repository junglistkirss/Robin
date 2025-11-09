using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Helpers;
using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;

namespace RobinMustache.Internals;

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
