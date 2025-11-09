using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Nodes;

namespace RobinMustache.Abstractions.Iterators;

public interface IIterator
{
    void Iterate(object? iterable, Action<object?> action);
    void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class;
}
