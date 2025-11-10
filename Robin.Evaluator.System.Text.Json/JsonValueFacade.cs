using Robin.Abstractions.Facades;
using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonValueFacade : BaseDataFacade<JsonValue>
{
    public readonly static JsonValueFacade Instance = new();
    private JsonValueFacade() { }
    public override bool IsCollection(JsonValue node, [NotNullWhen(true)] out IIterator? collection)
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

    public override bool IsTrue([NotNullWhen(true)] JsonValue node)
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

