using Robin.Abstractions.Facades;
using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonObjectFacade : BaseDataFacade<JsonObject>
{
    public readonly static JsonObjectFacade Instance = new();
    private JsonObjectFacade() { }
    public override bool IsCollection(JsonObject obj, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }




    public override bool IsTrue([NotNullWhen(true)] JsonObject obj)
    {
        return obj is not null;
    }
}

