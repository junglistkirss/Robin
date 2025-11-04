using Robin.Abstractions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonAccesorVisitor : IVariableSegmentVisitor<EvaluationResult, object?>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public EvaluationResult VisitIndex(IndexSegment segment, object? args)
    {
        if (args is JsonArray json && json.TryGetIndexValue(segment.Index, out object? node))
            return new(true, node);

        return new(false, null);
    }

    public EvaluationResult VisitMember(MemberSegment segment, object? args)
    {
        if (args is JsonObject json && json.TryGetMemberValue(segment.MemberName, out object? node))
            return new(true, node);

        return new(false, null);
    }

    public EvaluationResult VisitThis(ThisSegment segment, object? args)
    {
        return new(true, args);
    }
}

