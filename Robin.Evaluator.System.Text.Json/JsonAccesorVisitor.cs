using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonAccesorVisitor : IVariableSegmentVisitor<Type>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public bool VisitIndex(IndexSegment segment, Type args, out Delegate @delegate)
    {
        if (Nullable.GetUnderlyingType(args) == typeof(JsonArray))
        {
            return JsonAccessorExtensions.TryGetIndexValue(segment.Index, out @delegate);
        }
        @delegate = (Func<JsonArray?, object?>)(_ => null);
        return false;
    }

    public bool VisitMember(MemberSegment segment, Type args, out Delegate @delegate)
    {
        if (Nullable.GetUnderlyingType(args) == typeof(JsonObject))
        {
            return JsonAccessorExtensions.TryGetMemberValue(segment.MemberName, out @delegate);
        }
        @delegate = (Func<JsonObject?, object?>)(_ => null);
        return false;
    }

    public bool VisitThis(ThisSegment segment, Type args, out Delegate @delegate)
    {
        @delegate = (Func<object?, object?>)(x => x);
        return true;
    }
}

