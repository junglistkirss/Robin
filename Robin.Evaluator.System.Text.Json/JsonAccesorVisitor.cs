using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonAccesorVisitor : IVariableSegmentVisitor<Type>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public bool VisitIndex(IndexSegment segment, Type args, out Delegate @delegate)
    {
        if (args == typeof(JsonArray))
        {
            @delegate = (Func<JsonArray?, JsonNode?>)((x) => x is not null && segment.Index < x.Count ? x[segment.Index] : null);
            return true;
        }
        @delegate = (Func<object?, object?>)((object? _) => null);
        return false;
    }

    public bool VisitMember(MemberSegment segment, Type args, out Delegate @delegate)
    {
        if (args == typeof(JsonObject))
        {
            @delegate = (Func<JsonObject, JsonNode?>)(x => x is not null && x.TryGetPropertyValue(segment.MemberName, out JsonNode? node) ? node : null);
            return true;
        }
        @delegate = (Func<object?, object?>)((object? _) => null);
        return false;
    }

    public bool VisitThis(ThisSegment segment, Type args, out Delegate @delegate)
    {
        @delegate = (Func<object?, object?>)(x => x);
        return true;
    }
}

