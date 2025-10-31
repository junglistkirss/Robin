using System.Text.Json.Nodes;
using Robin.Contracts.Context;
using Robin.Contracts.Variables;

namespace Robin.Evaluator.System.Text.Json;

public record JsonEvaluationResult(bool Found, object? Value);

internal sealed class JsonObjectAccesorVisitor : IAccessorVisitor<JsonEvaluationResult, JsonNode>
{
    internal static readonly JsonObjectAccesorVisitor Instance = new();
    public JsonEvaluationResult VisitIndex(IndexAccessor accessor, JsonNode args)
    {
        if (args is JsonArray json)
        {
            return new(true, json[accessor.Index]);
        }
        return new(false, null);
    }

    public JsonEvaluationResult VisitKey(KeyAccessor accessor, JsonNode args)
    {
        if (args is JsonObject json)
        {
            object? resolvedKey = accessor.Key.Evaluate(args);
            if (resolvedKey is not null && json.TryGetPropertyValue(resolvedKey.ToString()!, out JsonNode? keyNode))
                return new(true, keyNode);
        }
        return new(false, null);
    }

    public JsonEvaluationResult VisitMember(MemberAccessor accessor, JsonNode args)
    {
        if (args is JsonObject json && json.TryGetPropertyValue(accessor.MemberName, out JsonNode? node))
            return new(true, node);
        return new(false, null);
    }

    public JsonEvaluationResult VisitParent(ParentAccessor accessor, JsonNode args)
    {
        if (args.Parent is not null)
            return new(true, args.Parent);
        return new(false, null);
    }

    public JsonEvaluationResult VisitThis(ThisAccessor accessor, JsonNode args)
    {
        return new(true, args);
    }
}

