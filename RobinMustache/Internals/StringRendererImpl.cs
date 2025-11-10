using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;

namespace RobinMustache.Internals;

internal sealed class StringRendererImpl(
    IRenderer<string> inner
    ) : IStringRenderer
{
    public string Render(ImmutableArray<INode> template, object? data)
    {
        if (data is not null)
            return inner.Render(template, data);
        return string.Empty;
    }
}
