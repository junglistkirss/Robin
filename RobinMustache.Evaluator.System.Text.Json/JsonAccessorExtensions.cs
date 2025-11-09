using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Extensions;
using RobinMustache.Abstractions.Facades;
using System.Text.Json.Nodes;

namespace RobinMustache.Evaluator.System.Text.Json;

public static class JsonAccessorExtensions
{
    public static IServiceCollection AddJsonAccessors(this IServiceCollection services)
    {
        return services
            .AddSingleton<IJsonEvaluator, JsonEvaluator>()
            .AddSingleton<IDataFacadeResolver, JsonDataFacadeResolver>()
            .AddDataFacade<JsonNode>(JsonNodeFacade.Instance)
            .AddDataFacade<JsonValue>(JsonValueFacade.Instance)
            .AddDataFacade<JsonArray>(JsonArrayFacade.Instance)
            .AddDataFacade<JsonObject>(JsonObjectFacade.Instance)
            .AddIndexObjectAccessor<JsonArray>(TryGetIndexValue)
            .AddMemberObjectAccessor<JsonObject>(TryGetMemberValue)
            .AddIndexObjectAccessor<JsonNode>(TryGetIndexValue)
            .AddMemberObjectAccessor<JsonNode>(TryGetMemberValue)
            ;
    }

    internal static bool TryGetMemberValue(this object? obj, string member, out object? value)
    {
        if (obj is JsonObject jObject && jObject.TryGetPropertyValue(member, out JsonNode? node))
        {
            value = node;
            return true;
        }
        value = null;
        return false;
    }
    internal static bool TryGetIndexValue(this object? obj, int index, out object? value)
    {
        if (obj is JsonArray jArray && index < jArray.Count)
        {
            value = jArray[index];
            return true;
        }
        value = null;
        return false;
    }
}

