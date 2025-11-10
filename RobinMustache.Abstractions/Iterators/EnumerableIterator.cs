using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Nodes;
using System.Collections;

namespace RobinMustache.Abstractions.Iterators;

internal sealed class EnumerableIterator : BaseIterator
{
    public readonly static EnumerableIterator Instance = new();
    private EnumerableIterator() { }
    public override void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        if (iterable is IEnumerable arr)
            ProcessEnumerable(arr, context, partialTemplate, visitor);
    }

    public override void Iterate(object? iterable, Action<object?> action)
    {
        if (iterable is IEnumerable arr)
            EnumerableProcess(arr, action);
    }
}
