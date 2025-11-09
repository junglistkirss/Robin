using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Iterators;
using Robin.Contracts.Nodes;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

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

internal sealed class JsonNodeFacade : IDataFacade<JsonNode>
{
    public readonly static JsonNodeFacade Instance = new();
    private JsonNodeFacade() { }
    public bool IsCollection(JsonNode node, [NotNullWhen(true)] out IIterator? collection)
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Array:
                JsonArray jArray = node.AsArray()!;
                return JsonArrayFacade.Instance.IsCollection(jArray, out collection);
            default:
                break;
        }
        collection = null;
        return false;
    }

    public bool IsTrue([NotNullWhen(true)] JsonNode node)
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Undefined:
                return false;
            case JsonValueKind.Object:
                JsonObject? jObject = node.AsObject();
                return JsonObjectFacade.Instance.IsTrue(jObject);
            case JsonValueKind.Array:
                JsonArray? jArray = node.AsArray();
                return JsonArrayFacade.Instance.IsTrue(jArray);
            case JsonValueKind.String:
                return !string.IsNullOrEmpty(node.GetValue<string>());
            case JsonValueKind.Number:
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
            case JsonValueKind.Null:
                return false;
        }
        return false;
    }
}

internal sealed class JsonValueFacade : IDataFacade<JsonValue>
{
    public readonly static JsonValueFacade Instance = new();
    private JsonValueFacade() { }
    public bool IsCollection(JsonValue node, [NotNullWhen(true)] out IIterator? collection)
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Array:
                JsonArray jArray = node.AsArray()!;
                return JsonArrayFacade.Instance.IsCollection(jArray, out collection);
            default:
                break;
        }
        collection = null;
        return false;
    }

    public bool IsTrue([NotNullWhen(true)] JsonValue node)
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Undefined:
                return false;
            case JsonValueKind.Object:
                JsonObject? jObject = node.AsObject();
                return JsonObjectFacade.Instance.IsTrue(jObject);
            case JsonValueKind.Array:
                JsonArray? jArray = node.AsArray();
                return JsonArrayFacade.Instance.IsTrue(jArray);
            case JsonValueKind.String:
                return !string.IsNullOrEmpty(node.GetValue<string>());
            case JsonValueKind.Number:
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
            case JsonValueKind.Null:
                return false;
        }
        return false;
    }
}
internal sealed class JsonArrayFacade : IDataFacade<JsonArray>
{
    public readonly static JsonArrayFacade Instance = new();
    private JsonArrayFacade() { }
    public bool IsCollection(JsonArray obj, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = JsonArrayIteraor.Instance;
        return obj.Count > 0;
    }
    public bool IsTrue([NotNullWhen(true)] JsonArray obj)
    {
        return obj is not null && obj.Count > 0;
    }
}
internal sealed class JsonObjectFacade : IDataFacade<JsonObject>
{
    public readonly static JsonObjectFacade Instance = new();
    private JsonObjectFacade() { }
    public bool IsCollection(JsonObject obj, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }




    public bool IsTrue([NotNullWhen(true)] JsonObject obj)
    {
        return obj is not null;
    }
}

