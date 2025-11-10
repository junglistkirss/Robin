using RobinMustache.Abstractions.Facades;
using RobinMustache.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RobinMustache.Evaluator.System.Text.Json;

internal sealed class JsonNodeFacade : BaseDataFacade<JsonNode>
{
    public readonly static JsonNodeFacade Instance = new();
    private JsonNodeFacade() { }
    public override bool IsCollection(JsonNode node, [NotNullWhen(true)] out IIterator? collection)
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

    public override bool IsTrue([NotNullWhen(true)] JsonNode node)
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

