using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Nodes;
using System.Collections;

namespace RobinMustache.Abstractions.Iterators;

internal sealed class IListIterator : BaseIterator
{
    public readonly static IListIterator Instance = new();
    private IListIterator() { }
    public override void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        if (iterable is IList arr)
            ProcessEnumerable(arr, context, partialTemplate, visitor);
    }

    public override void Iterate(object? iterable, Action<object?> action)
    {
        if (iterable is IList arr)
            EnumerableProcess(arr, action);
    }
}

