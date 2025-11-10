using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Iterators;
using RobinMustache.Abstractions.Nodes;
using System.Text.Json.Nodes;

namespace RobinMustache.Evaluator.System.Text.Json;

internal sealed class JsonArrayIteraor : BaseIterator
{
    public readonly static JsonArrayIteraor Instance = new();
    private JsonArrayIteraor() { }

    public override void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
    {
        if (iterable is JsonArray list)
            ProcessEnumerable<T, JsonArray>(list, context, partialTemplate, visitor);
    }

    public override void Iterate(object? iterable, Action<object?> action)
    {
        if (iterable is JsonArray list)
            EnumerableProcess<JsonArray>(list, action);
    }
}

