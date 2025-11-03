using Robin.Abstractions.Facades;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonNodeFacade(JsonNode node) : IDataFacade
{
    public object? RawValue => node;

    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Array:
                JsonArray jArray = node.AsArray()!;
                collection = jArray.GetEnumerator();
                return jArray.Count > 0;
            default:
                break;
        }
        collection = null;
        return false;
    }

    public bool IsTrue()
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Undefined:
                return false;
            case JsonValueKind.Object:
                JsonObject? jObject = node.AsObject();
                return jObject is not null;
            case JsonValueKind.Array:
                JsonArray? jArray = node.AsArray();
                return jArray is not null && jArray.Count > 0;
            case JsonValueKind.String:
                return !string.IsNullOrEmpty(node.GetValue<string>());
            case JsonValueKind.Number:
                return true;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
            case JsonValueKind.Null:
            default:
                return false;
        }
    }
}

