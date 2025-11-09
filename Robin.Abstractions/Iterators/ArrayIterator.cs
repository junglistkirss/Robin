using Robin.Abstractions.Context;
using Robin.Abstractions.Nodes;

namespace Robin.Abstractions.Iterators;

internal sealed class ArrayIterator : BaseIterator
{
    public readonly static ArrayIterator Instance = new();
    private ArrayIterator() { }
    public override void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        if (iterable is Array arr)
            ProcessEnumerable(arr, context, partialTemplate, visitor);
    }

    public override void Iterate(object? iterable, Action<object?> action)
    {
        if (iterable is Array arr)
            EnumerableProcess(arr, action);
    }
}
