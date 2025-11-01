using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Robin.Contracts.Context;
using Robin.Contracts.Expressions;

namespace Robin.Evaluator.System.Text.Json;

public sealed class JsonEvaluator : IEvaluator
{
    public static readonly JsonEvaluator Instance = new();

    public bool IsCollection(object? value, [NotNullWhen(true)] out IEnumerable? collection)
    {
        if (value is JsonNode node)
        {
            if (node.GetValueKind() == global::System.Text.Json.JsonValueKind.Array)
            {
                collection = node.AsArray()!;
                return true;
            }
        }
        else if (value is IEnumerable objects)
        {
            collection = objects;
            return true;

        }
        collection = null;
        return false;
    }

    public bool IsTrue(object? value)
    {
        if (value is JsonNode node)
        {
            switch (node.GetValueKind())
            {
                case global::System.Text.Json.JsonValueKind.Undefined:
                    return false;
                case global::System.Text.Json.JsonValueKind.Object:
                    return node.AsObject() is not null;
                case global::System.Text.Json.JsonValueKind.Array:
                    return node.AsArray()!.Count > 0;
                case global::System.Text.Json.JsonValueKind.String:
                    return !string.IsNullOrEmpty(node.GetValue<string>());
                case global::System.Text.Json.JsonValueKind.Number:
                    return true;
                case global::System.Text.Json.JsonValueKind.True:
                    return true;
                case global::System.Text.Json.JsonValueKind.False:
                    return false;
                case global::System.Text.Json.JsonValueKind.Null:
                    return false;
                default:
                    break;
            }
        }
        return value is bool b ? b : value is string s ? !string.IsNullOrEmpty(s) : value is not null;
    }

    public bool TryResolve(IExpressionNode expression, object? data, out object? value)
    {
        if (data is JsonNode node)
        {
            value = expression.Accept(JsonObjectExpressionNodeVisitor.Instance, node);
            return true;
        }
        value = null;
        return false;
    }
}

