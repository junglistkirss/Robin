using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Nodes;

namespace RobinMustache.Abstractions.Iterators;

internal sealed class ListIterator<TItem> : BaseIterator
{
    public readonly static ListIterator<TItem> Instance = new();
    private ListIterator() { }

    public override void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        if (iterable is List<TItem> list)
            ProcessIterable<T, List<TItem>, TItem>(list, context, partialTemplate, visitor);
    }

    public override void Iterate(object? iterable, Action<object?> action)
    {
        if (iterable is List<TItem> list)
            IterableAction<List<TItem>, TItem>(list, action);
    }
}
