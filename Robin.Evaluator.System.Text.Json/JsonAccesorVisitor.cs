using Robin.Abstractions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;


internal static class JsonAccessorExtensions
{
    public static bool TryGetMemberValue(JsonArray? source, string member, [MaybeNullWhen(false)] out object? value)
    {
        if (source is not null)
        {
            value = source[member];
            return true;
        }
        value = null;
        return false;
    }

}


internal sealed class JsonAccesorVisitor : IAccessorVisitor<EvaluationResult, object?>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public EvaluationResult VisitIndex(IndexAccessor accessor, object? args)
    {
        if (args is JsonArray json)
        {
            return new(ResoltionState.Found, json[accessor.Index].AsJsonFacade());
        }
        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitMember(MemberAccessor accessor, object? args)
    {
        if (args is JsonObject json && json.TryGetPropertyValue(accessor.MemberName, out JsonNode? node))
            return new(ResoltionState.Found, node.AsJsonFacade());

        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitThis(ThisAccessor accessor, object? args)
    {
        return new(ResoltionState.Found, args.AsJsonFacade());
    }
}

