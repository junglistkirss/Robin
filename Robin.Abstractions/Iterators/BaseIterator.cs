using Robin.Abstractions.Context;
using Robin.Abstractions.Nodes;
using System.Collections;

namespace Robin.Abstractions.Iterators;

public abstract class BaseIterator : IIterator
{
    public abstract void Iterate(object? iterable, Action<object?> action);

    protected static void IterableAction<TIterable, TItem>(TIterable iterable, Action<object?> action)
        where TIterable : IEnumerable<TItem>
    {
        if (iterable is TIterable arr)
        {
            foreach (TItem item in arr)
                action(item);
        }
    }
    protected static void EnumerableProcess<TIterable>(TIterable iterable, Action<object?> action)
        where TIterable : IEnumerable
    {
        if (iterable is TIterable arr)
        {
            foreach (object? item in arr)
                action(item);
        }
    }
    public abstract void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class;

    protected static void ProcessIterable<T, TIterable, TItem>(TIterable? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor)
        where T : class
        where TIterable : IEnumerable<TItem>
    {
        if (iterable is TIterable arr)
        {
            foreach (TItem item in arr)
            {
                if (item is not null)
                    ProcessItem(item, context, partialTemplate, visitor);
            }
        }
    }

    protected static void ProcessEnumerable<T, TIterable>(TIterable? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor)
       where T : class
       where TIterable : IEnumerable
    {
        if (iterable is TIterable arr)
        {
            foreach (object? item in arr)
            {
                if (item is not null)
                    ProcessItem(item, context, partialTemplate, visitor);
            }
        }
    }
    protected static void ProcessItem<T, TObject>(TObject item, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        using (DataContext.Push(item))
        {
            foreach (INode node in partialTemplate)
            {
                node.Accept(visitor, context);
            }
        }
    }
}
