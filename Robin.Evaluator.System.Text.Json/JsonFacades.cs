using Robin.Abstractions.Facades;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

public static class JsonFacades
{
    public static IDataFacade FromJsonNode(object? obj) => obj is JsonNode jNode ? new JsonNodeFacade(jNode) : throw new InvalidDataException("Not a json node");
    public static IDataFacade AsJsonFacade(this object? obj) => obj is JsonNode node ? new JsonNodeFacade(node) : obj.AsFacade();

}

