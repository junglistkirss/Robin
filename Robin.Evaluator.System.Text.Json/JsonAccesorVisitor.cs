using Robin.Abstractions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;


internal sealed class JsonAccesorVisitor : IAccessorVisitor<EvaluationResult, DataContext>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public EvaluationResult VisitIndex(IndexAccessor accessor, DataContext context)
    {
        if (context.Data is JsonArray json)
        {
            return new(ResoltionState.Found, json[accessor.Index].AsJsonFacade());
        }
        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitKey(KeyAccessor accessor, DataContext context)
    {
        EvaluationResult resolvedKey = accessor.Key.Evaluate(this, context);

        if (resolvedKey.Status == ResoltionState.NotFound && context.Parent is not null)
            resolvedKey = accessor.Key.Evaluate(this, context.Parent);

        if (resolvedKey.Status == ResoltionState.Found)
        {
            string key = resolvedKey.Value.RawValue?.ToString() ?? string.Empty;

            if (context.Data is JsonObject json && json.TryGetPropertyValue(key, out JsonNode? keyNode))
            {
                return new(ResoltionState.Found, keyNode.AsJsonFacade());
            }
            else if (context.Parent?.Data is JsonObject prevJson && prevJson.TryGetPropertyValue(key, out JsonNode? prevKeyNode))
            {
                return new(ResoltionState.Found, prevKeyNode.AsJsonFacade());
            }
        }
        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitMember(MemberAccessor accessor, DataContext context)
    {
        if (context.Data is JsonObject json && json.TryGetPropertyValue(accessor.MemberName, out JsonNode? node))
            return new(ResoltionState.Found, node.AsJsonFacade());

        if (context.Parent?.Data is JsonObject jsonPrev && jsonPrev.TryGetPropertyValue(accessor.MemberName, out JsonNode? nodePrev))
            return new(ResoltionState.Found, nodePrev.AsJsonFacade());

        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitParent(ParentAccessor accessor, DataContext context)
    {
        DataContext parent = context;
        while (parent.Parent is not null)
        {
            parent = parent.Parent;
        }
        return new(ResoltionState.Found, parent.Data.AsJsonFacade());
    }

    public EvaluationResult VisitThis(ThisAccessor accessor, DataContext context)
    {
        return new(ResoltionState.Found, context.Data.AsJsonFacade());
    }
}

