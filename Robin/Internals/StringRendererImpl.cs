using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Internals;

internal sealed class StringRendererImpl(
    IRenderer<string> inner
    ) : IStringRenderer
{
    public string Render(ImmutableArray<INode> template, object? data)
    {
        return inner.Render(template, data);
    }
}
