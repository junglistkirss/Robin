using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using Robin.Contracts.Context;
using Robin.Contracts.Expressions;

namespace Robin.Evaluator.System.Text.Json;

public sealed class JsonEvaluator : IEvaluator
{
    public static readonly JsonEvaluator Instance = new();
    private static readonly ExpressionNodeVisitor NodeInstance = new(JsonAccesorVisitor.Instance);

    public bool IsCollection(object? value, [NotNullWhen(true)] out IEnumerable? collection)
    {
        if (value is JsonNode node)
        {
            if (node.GetValueKind() == JsonValueKind.Array)
            {
                JsonArray jArray = node.AsArray()!;
                collection = jArray;
                return jArray.Count > 0;
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
                case JsonValueKind.Undefined:
                    return false;
                case JsonValueKind.Object:
                    var obj = node.AsObject();
                    return obj is not null;
                case JsonValueKind.Array:
                    JsonArray? jArray = node.AsArray();
                    return jArray is not null && jArray.Count > 0;
                case JsonValueKind.String:
                    return !string.IsNullOrEmpty(node.GetValue<string>());
                case JsonValueKind.Number:
                    return true;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    return false;
                default:
                    break;
            }
        }
        return value is bool b ? b : value is string s ? !string.IsNullOrEmpty(s) : value is not null;
    }

    public bool TryResolve(IExpressionNode expression, DataContext? data, out object? value)
    {
        if (data is null)
        {
            value = null;
            return false;
        }

        EvaluationResult result = expression.Accept(NodeInstance, data);

        if (result.Status == ResoltionState.NotFound && data.Previous is not null)
            result = expression.Accept(NodeInstance, data.Previous);

        value = result.Value;
        return result.Status == ResoltionState.Found;
    }
}

