using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;
using Robin.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin;

internal sealed class RendererImpl<T, TOut>(
    T defaultBuilder,
    Func<T, TOut> output,
    INodeVisitor<NoValue, RenderContext<T>> visitor,
    IEvaluator evaluator,
    Action<Helper>? helperConfig = null
    ) : IRenderer<TOut>
    where T : class
{
    public TOut Render(ImmutableArray<INode> template, string source, object? data)
    {
        T result = defaultBuilder.Render<T>(source, visitor, evaluator, template, data, helperConfig);
        return output(result);
    }
}

internal sealed class StringRendererImpl(
    IRenderer<string> inner
    ) : IStringRenderer
{
    public string Render(ImmutableArray<INode> template, string source, object? data)
    {
        return inner.Render(template, source, data);
    }
}
