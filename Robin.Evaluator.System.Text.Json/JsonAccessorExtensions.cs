using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

public static class JsonAccessorExtensions
{
    public const string JsonEvaluatorKey = "json";
    public static IServiceCollection AddJsonAccessors(this IServiceCollection services)
    {
        return services
            .AddKeyedSingleton<IJsonEvaluator, JsonEvaluator>(JsonEvaluatorKey)
            .AddSingleton<IJsonEvaluator, JsonEvaluator>()
            .AddIndexAccessor<JsonArray>(TryGetIndexValue)
            .AddMemberAccessor<JsonObject>(TryGetMemberValue)
            .AddIndexAccessor<JsonNode>(TryGetIndexValue)
            .AddMemberAccessor<JsonNode>(TryGetMemberValue);
    }

    internal static bool TryGetMemberValue(string member, [NotNull] out Delegate @delegate)
    {
        @delegate = (Func<JsonNode?, JsonNode?>)(x => x is JsonObject obj && obj.TryGetPropertyValue(member, out JsonNode? node) ? node : throw new Exception("Bad type or missing proppoerty"));
        return true;
    }
    internal static bool TryGetIndexValue(int index, [NotNull] out Delegate @delegate)
    {
        @delegate = (Func<JsonNode?, JsonNode?>)((x) => x is JsonArray arr && index < arr.Count ? arr[index] : throw new Exception("Bad type or index out of range"));
        return false;
    }
}

