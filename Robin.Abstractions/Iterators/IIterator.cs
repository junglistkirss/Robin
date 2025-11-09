using Robin.Abstractions.Context;
using Robin.Abstractions.Nodes;

namespace Robin.Abstractions.Iterators;

public interface IIterator
{
    void Iterate(object? iterable, Action<object?> action);
    void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class;
}
