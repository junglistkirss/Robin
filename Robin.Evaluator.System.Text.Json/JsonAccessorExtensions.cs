using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

public static class JsonAccessors
{

    internal static bool TryGetMemberValue(this JsonObject? jObject, string member, out object? value)
    {
        if (jObject is not null && jObject.TryGetPropertyValue(member, out JsonNode? node))
        {
            value = node;
            return true;
        }
        value = null;
        return false;
    }
    internal static bool TryGetIndexValue(this JsonArray? jArray, int index, out object? value)
    {
        if (jArray is not null && index < jArray.Count)
        {
            value = jArray[index];
            return true;
        }
        value = null;
        return false;
    }

    public static IMemberAccessor<JsonObject> CreateJsonObjectMemberAccessor() => AccessorBuilder.CreateMemberObjectAccessor<JsonObject>(TryGetMemberValue);
    public static IIndexAccessor<JsonArray> CreateJsonarrayIndexAccessor() => AccessorBuilder.CreateIndexObjectAccessor<JsonArray>(TryGetIndexValue);
}
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
            .AddMemberObjectAccessor<JsonObject>(JsonAccessors.TryGetMemberValue)
            .AddIndexObjectAccessor<JsonArray>(JsonAccessors.TryGetIndexValue)
            //.AddIndexObjectAccessor<JsonNode>(JsonAccessor.TryGetIndexValue)
            //.AddMemberObjectAccessor<JsonNode>(JsonAccessor.TryGetMemberValue)
            ;
    }

}

