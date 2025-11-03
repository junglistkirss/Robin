using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

public static class JsonAccessorExtensions
{

    public static IServiceCollection AddJsonAccessors(this IServiceCollection services)
    {
        return services
            .AddIndexAccessor<JsonArray>(TryGetIndexValue)
            .AddMemberAccessor<JsonObject>(TryGetMemberValue);
    }

    internal static bool TryGetMemberValue(this JsonObject? source, string member, [MaybeNullWhen(false)] out object? value)
    {
        if (source is not null && source.TryGetPropertyValue(member, out JsonNode? node))
        {
            value = node;
            return true;
        }
        value = null;
        return false;
    }
    internal static bool TryGetIndexValue(this JsonArray? source, int index, [MaybeNullWhen(false)] out object? value)
    {
        if (source is not null && index >= 0 && index < source.Count)
        {
            value = source[index];
            return true;
        }
        value = null;
        return false;
    }
}

