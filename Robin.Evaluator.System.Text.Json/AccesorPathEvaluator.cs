using System.Text.Json.Nodes;
using Robin.Contracts.Variables;

namespace Robin.Evaluator.System.Text.Json;

internal static class AccesorPathEvaluator
{
    internal static object? Evaluate(this AccesorPath path, JsonNode node)
    {
        JsonEvaluationResult result = new(true, null);
        int i = 0;
        object? ctx = node;
        while (result.Found && i < path.Segments.Length)
        {
            IAccessor item = path.Segments[i];
            if (ctx is JsonNode n)
            {
                JsonEvaluationResult res = item.Accept(JsonObjectAccesorVisitor.Instance, n);
                result = res;
                if (res.Found)
                    ctx = res.Value;
                else
                {
                    JsonNode? parentnode = n.Parent;
                    JsonEvaluationResult parentRes = new(true, null);
                    while (parentnode is not null && !result.Found)
                    {
                        // Try to resolve from parent context
                        parentRes = item.Accept(JsonObjectAccesorVisitor.Instance, parentnode);                        
                        if (parentRes.Found)
                        {
                            result = parentRes;
                            ctx = parentRes.Value;
                        }
                        else
                        {
                            parentnode = parentnode.Parent;
                        }
                    }
                }
            }
            else
            {
                result = result with { Found = false };
            }
            i++;
        }
        return result.Value;
    }
}

