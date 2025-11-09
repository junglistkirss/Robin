using RobinMustache.Abstractions.Facades;
using System.Text.Json.Nodes;

namespace RobinMustache.Evaluator.System.Text.Json;

public static class JsonFacades
{
    //public static IDataFacade FromJsonNode(object? obj)
    //{
    //    return obj is JsonNode ? JsonNodeFacade.Instance : throw new InvalidDataException("Not a json node");
    //}

    public static IDataFacade AsJsonFacade(this object? obj, IDataFacade fallbackFacade)
    {
        return obj switch
        {
            JsonValue => JsonValueFacade.Instance,
            JsonArray => JsonArrayFacade.Instance,
            JsonObject => JsonObjectFacade.Instance,
            _ => fallbackFacade
        };
    }
}

