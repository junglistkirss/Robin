using System.Text.Json.Nodes;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonObjectExpressionNodeVisitor : IExpressionNodeVisitor<object?, JsonNode>
{
    internal static readonly JsonObjectExpressionNodeVisitor Instance = new();
    public object? VisitBinaryOperation(BinaryOperationExpressionNode node, JsonNode args)
    {
        throw new NotImplementedException();
    }

    public object? VisitFunctionCall(FunctionCallNode node, JsonNode args)
    {
        throw new NotImplementedException();
    }

    public object? VisitIdenitifer(IdentifierExpressionNode node, JsonNode args)
    {
        JsonEvaluationResult result = new(true, null);
        int i = 0;
        object? ctx = args;
        while (result.Found && i < node.Path.Segments.Length)
        {
            IAccessor item = node.Path.Segments[i];
            if (ctx is JsonNode n)
            {
                JsonEvaluationResult res = item.Accept(JsonObjectAccesorVisitor.Instance, n);
                result = res;
                if (res.Found)
                    ctx = res.Value;
            }
            else
            {                
                result = new JsonEvaluationResult(false, result.Value);
            }
            i++;
        }
        return result.Value;
    }

    public object? VisitLiteral(LiteralExpressionNode node, JsonNode _)
    {
        return node.Constant;
    }

    public object? VisitNumber(NumberExpressionNode node, JsonNode _)
    {
        return node.Constant;
    }

    public object? VisitUnaryOperation(UnaryOperationExpressionNode node, JsonNode args)
    {
        throw new NotImplementedException();
    }
}

