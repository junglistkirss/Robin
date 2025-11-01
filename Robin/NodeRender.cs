using Robin.Contracts.Context;
using Robin.Contracts.Nodes;
using System.Collections;
using System.Collections.Immutable;
using System.Net;

namespace Robin;

public class NodeRender : INodeVisitor<RenderResult, RenderContext>
{
    public readonly static NodeRender Instance = new();

    public RenderResult VisitText(TextNode node, RenderContext context)
    {
        context.Builder.Append(node.Text);
        return new RenderResult(true, null);
    }
    public RenderResult VisitComment(CommentNode node, RenderContext context)
    {
        return new RenderResult(true, null);
    }

    public RenderResult VisitPartial(PartialNode node, RenderContext context)
    {
        throw new NotImplementedException();

    }

    public RenderResult VisitVariable(VariableNode node, RenderContext context)
    {
        if (context.Evaluator.TryResolve(node.Expression, context.Data, out object? value))
        {
            if (node.IsUnescaped)
                context.Builder.Append(value?.ToString());
            else
                context.Builder.Append(WebUtility.HtmlEncode(value?.ToString()));
            return new RenderResult(true, null);
        }
        return new RenderResult(false, null);
    }

    public RenderResult VisitSection(SectionNode node, RenderContext context)
    {
        object? subData = context.Evaluator.TryResolve(node.Expression, context.Data, out object? value) ? value : null;
        bool thruly = context.Evaluator.IsTrue(subData);

        if ((!node.Inverted && thruly) || (node.Inverted && !thruly))
        {
            if (context.Evaluator.IsCollection(subData, out IEnumerable? collection))
            {
                foreach (object? item in collection)
                {
                    RenderContext itemCtx = context with
                    {
                        Data = item
                    };
                    ImmutableArray<INode>.Enumerator enumerator = node.Children.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        RenderResult result = enumerator.Current.Accept(this, itemCtx);
                        if (!result.IsComplete)
                            return result;
                    }
                }
            }
            else
            {
                RenderContext innerCtx = context with
                {
                    Data = subData
                };
                ImmutableArray<INode>.Enumerator enumerator = node.Children.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    RenderResult result = enumerator.Current.Accept(this, innerCtx);
                    if (!result.IsComplete)
                        return result;
                }
            }
            
        }
        return new RenderResult(true, null);
    }
}