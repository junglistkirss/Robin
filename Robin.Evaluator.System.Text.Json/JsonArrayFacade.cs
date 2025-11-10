using Robin.Abstractions.Facades;
using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonArrayFacade : BaseDataFacade<JsonArray>
{
    public readonly static JsonArrayFacade Instance = new();
    private JsonArrayFacade() { }
    public override bool IsCollection(JsonArray obj, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = JsonArrayIteraor.Instance;
        return obj.Count > 0;
    }
    public override bool IsTrue([NotNullWhen(true)] JsonArray obj)
    {
        return obj is not null && obj.Count > 0;
    }
}

