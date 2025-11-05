using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonAccesorVisitor : IVariableSegmentVisitor<EvaluationResult, SourceContext>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public EvaluationResult VisitIndex(IndexSegment segment, SourceContext args)
    {
        if (args.Data.Current is JsonArray json && int.TryParse(args.GetValue(segment.Index), out int index) && json.TryGetIndexValue(index, out object? node))
            return new(true, node);

        return new(false, null);
    }

    public EvaluationResult VisitMember(MemberSegment segment, SourceContext args)
    {
        if (args.Data.Current is JsonObject json && json.TryGetMemberValue(args.GetValue(segment.MemberName), out object? node))
            return new(true, node);

        return new(false, null);
    }

    public EvaluationResult VisitThis(ThisSegment segment, SourceContext args)
    {
        return new(true, args.Data.Current);
    }
}

