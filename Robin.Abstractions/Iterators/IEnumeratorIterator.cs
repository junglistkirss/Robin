using Robin.Abstractions.Context;
using Robin.Contracts.Nodes;
using System.Collections;

namespace Robin.Abstractions.Iterators;

internal sealed class IEnumeratorIterator : BaseIterator
{
    public readonly static IEnumeratorIterator Instance = new();
    private IEnumeratorIterator() { }
    public override void Iterate(object? iterable, Action<object?> action)
    {
        if (iterable is IEnumerator enumerator)
            while (enumerator.MoveNext())
                action(enumerator.Current);
    }

    public override void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        if (iterable is IEnumerator enumerator)
            while (enumerator.MoveNext())
                ProcessItem(enumerator.Current, context, partialTemplate, visitor);
    }
}
