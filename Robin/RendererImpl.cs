using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin;

internal sealed class RendererImpl(
    INodeVisitor<NoValue, RenderContext> visitor,
    IEvaluator evaluator,
    Action<Helper>? helperConfig = null) : IRenderer
{
    public string Render(ImmutableArray<INode> template, object? data)
    {
        return visitor.Render(evaluator, template, data, helperConfig);
    }
}
