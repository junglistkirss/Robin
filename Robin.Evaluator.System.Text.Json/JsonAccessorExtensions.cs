using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using System.Diagnostics.CodeAnalysis;
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
            .AddMemberAccessor<JsonObject>(TryGetMemberValue);
    }

    internal static bool TryGetMemberValue(string member, [NotNull] out Delegate @delegate)
    {
        @delegate = (Func<JsonObject?, JsonNode?>)(x => x is not null && x.TryGetPropertyValue(member, out JsonNode? node) ? node : null);
        return true;
    }
    internal static bool TryGetIndexValue(int index, [NotNull] out Delegate @delegate)
    {
        @delegate = (Func<JsonArray?, JsonNode?>)((x) => x is not null && index < x.Count ? x[index] : null);
        return false;
    }
}

