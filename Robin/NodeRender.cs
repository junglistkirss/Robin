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
        return RenderResult.Complete;
    }
    public RenderResult VisitComment(CommentNode node, RenderContext context)
    {
        return RenderResult.Complete;
    }

    public RenderResult VisitPartial(PartialDefineNode node, RenderContext context)
    {
        return RenderResult.Complete;
    }

    public RenderResult VisitVariable(VariableNode node, RenderContext context)
    {
        if (context.Evaluator.TryResolve(node.Expression, context.Data, out object? value))
        {
            if (node.IsUnescaped)
                context.Builder.Append(value?.ToString());
            else
                context.Builder.Append(WebUtility.HtmlEncode(value?.ToString()));
            return RenderResult.Complete;
        }
        return RenderResult.Complete;
    }

    public RenderResult VisitSection(SectionNode node, RenderContext context)
    {
        object? subData = context.Evaluator.TryResolve(node.Expression, context.Data, out object? value) ? value : null;
        bool thruly = context.Evaluator.IsTrue(subData);

        if ((!node.Inverted && thruly) || (node.Inverted && !thruly))
        {
            return RenderTree(context, subData, node.Children);
        }
        return RenderResult.Complete;
    }

    public RenderResult VisitPartialCall(PartialCallNode node, RenderContext context)
    {
        object? subData = context.Evaluator.TryResolve(node.Expression, context.Data, out object? value) ? value : null;
        bool thruly = context.Evaluator.IsTrue(subData);
        if (context.Partials.TryGetValue(node.PartialName, out ImmutableArray<INode> partialTemplate) && thruly)
        {
            return  RenderTree(context, subData, partialTemplate);

        }
        return RenderResult.Complete;
    }

    private  RenderResult  RenderTree(RenderContext context, object? subData, ImmutableArray<INode> partialTemplate)
    {
        if (context.Evaluator.IsCollection(subData, out IEnumerable? collection))
        {
            foreach (object? item in collection)
            {
                RenderContext itemCtx = context with
                {
                    Data = context.Data?.Child(item) ?? new DataContext(item, null),
                };
                ImmutableArray<INode>.Enumerator enumerator = partialTemplate.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    RenderResult result = enumerator.Current.Accept(this, itemCtx);
                    if (!result.IsComplete)
                    {
                        return result;
                    }
                }
            }
        }
        else
        {
            RenderContext innerCtx = context with
            {
                Data = context.Data?.Child(subData) ?? new DataContext(subData, null),
            };
            ImmutableArray<INode>.Enumerator enumerator = partialTemplate.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RenderResult result = enumerator.Current.Accept(this, innerCtx);
                if (!result.IsComplete)
                    return result;
            }
        }

        return RenderResult.Complete;
    }
}