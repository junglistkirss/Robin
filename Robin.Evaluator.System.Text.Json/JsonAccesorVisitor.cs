using System.Text.Json.Nodes;
using Robin.Contracts.Context;
using Robin.Contracts.Variables;

namespace Robin.Evaluator.System.Text.Json;


internal sealed class JsonAccesorVisitor : IAccessorVisitor<EvaluationResult, DataContext>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public EvaluationResult VisitIndex(IndexAccessor accessor, DataContext args)
    {
        if (args.Data is JsonArray json)
        {
            return new(ResoltionState.Found, json[accessor.Index]);
        }
        return new(ResoltionState.NotFound, null);
    }

    public EvaluationResult VisitKey(KeyAccessor accessor, DataContext args)
    {
        EvaluationResult resolvedKey = accessor.Key.Accept(this, args);

        if (resolvedKey.Status == ResoltionState.NotFound && args.Previous is not null)
            resolvedKey = accessor.Key.Accept(this, args.Previous);
        
        if (resolvedKey.Status == ResoltionState.Found)
        {
            string key = resolvedKey.Value?.ToString() ?? string.Empty;

            if (args.Data is JsonObject json && json.TryGetPropertyValue(key, out JsonNode? keyNode))
            {
                return new(ResoltionState.Found, keyNode);
            }
            else if(args.Previous?.Data is JsonObject prevJson && prevJson.TryGetPropertyValue(key, out JsonNode? prevKeyNode))
            {
                return new(ResoltionState.Found, prevKeyNode);
            }
        }
        return new(ResoltionState.NotFound, null);
    }

    public EvaluationResult VisitMember(MemberAccessor accessor, DataContext args)
    {
        if (args.Data is JsonObject json && json.TryGetPropertyValue(accessor.MemberName, out JsonNode? node))
            return new(ResoltionState.Found, node);

        if (args.Previous?.Data is JsonObject jsonPrev && jsonPrev.TryGetPropertyValue(accessor.MemberName, out JsonNode? nodePrev))
            return new(ResoltionState.Found, nodePrev);

        return new(ResoltionState.NotFound, null);
    }

    public EvaluationResult VisitParent(ParentAccessor accessor, DataContext args)
    {
        if (args.Previous?.Data is not null)
            return new(ResoltionState.Found, args.Previous.Data);
        return new(ResoltionState.NotFound, null);
    }

    public EvaluationResult VisitThis(ThisAccessor accessor, DataContext args)
    {
        return new(ResoltionState.Found, args.Data);
    }
}

