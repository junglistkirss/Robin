using Robin.Contracts.Context;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin;

public static class Renderer
{
    public static string Render(this INodeVisitor<RenderResult, RenderContext> visitor, IEvaluator evaluator, ImmutableArray<INode> template, object? data)
    {
        RenderContext ctx = new()
        {
            Data = new DataContext(data, null),
            Evaluator = evaluator,
            Builder = new StringBuilder()
        };
        foreach (INode node in template)
        {
            RenderResult result = node.Accept(visitor, ctx);
            if (!result.IsComplete)
            {
                if (result.Exception is not null)
                {
                    throw result.Exception;
                }
                //break;
            }
        }
        return ctx.Builder.ToString();
    }
}
