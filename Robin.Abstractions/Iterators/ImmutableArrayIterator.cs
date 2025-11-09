using Robin.Abstractions.Context;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Abstractions.Iterators;

internal sealed class ImmutableArrayIterator<TItem> : BaseIterator
{
    public readonly static ImmutableArrayIterator<TItem> Instance = new();
    private ImmutableArrayIterator() { }

    public override void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        if (iterable is ImmutableArray<TItem> arr)
            ProcessIterable<T, ImmutableArray<TItem>, TItem>(arr, context, partialTemplate, visitor);
    }

    public override void Iterate(object? iterable, Action<object?> action)
    {
        if (iterable is ImmutableArray<TItem> arr)
            IterableAction<ImmutableArray<TItem>, TItem>(arr, action);
    }
}
