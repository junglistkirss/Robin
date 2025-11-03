using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using System.Collections;
using System.Collections.Immutable;
using System.Net;

namespace Robin.Abstractions;

public class NodeRender : INodeVisitor<NoValue, RenderContext>
{
    public readonly static NodeRender Instance = new();

    public NoValue VisitText(TextNode node, RenderContext context)
    {
        context.Builder.Append(node.Text);
        return NoValue.Instance;
    }
    public NoValue VisitComment(CommentNode node, RenderContext context)
    {
        return NoValue.Instance;
    }

    public NoValue VisitPartialDefine(PartialDefineNode node, RenderContext context)
    {
        return NoValue.Instance;
    }

    public NoValue VisitVariable(VariableNode node, RenderContext context)
    {
        IDataFacade value = context.Evaluator.Resolve(node.Expression, context.Data);
        if (value.IsTrue())
        {
            if (node.IsUnescaped)
                context.Builder.Append(value.RawValue?.ToString());
            else
                context.Builder.Append(WebUtility.HtmlEncode(value.RawValue?.ToString()));
        }
        return NoValue.Instance;
    }

    public NoValue VisitSection(SectionNode node, RenderContext context)
    {
        IDataFacade value = context.Evaluator.Resolve(node.Expression, context.Data);
        bool thruly = value.IsTrue();

        if ((!node.Inverted && thruly) || (node.Inverted && !thruly))
        {
            return RenderTree(context, value, node.Children);
        }

        return NoValue.Instance;
    }

    public NoValue VisitPartialCall(PartialCallNode node, RenderContext context)
    {
        IDataFacade value = context.Evaluator.Resolve(node.Expression, context.Data);

        if (value.IsTrue() && context.Partials.TryGetValue(node.PartialName, out ImmutableArray<INode> partialTemplate))
        {
            context = context with
            {
                Partials = partialTemplate.ExtractsPartials(context.Partials)
            };
            return RenderTree(context, value, partialTemplate);

        }
        return NoValue.Instance;
    }

    private NoValue RenderTree(RenderContext context, IDataFacade facade, ImmutableArray<INode> partialTemplate)
    {
        if (facade.IsCollection(out IEnumerator? collection))
        {
            while (collection.MoveNext())
            {
                object? item = collection.Current;
                RenderContext itemCtx = context with
                {
                    Data = context.Data?.Child(item) ?? new DataContext(item, null),
                };
                ImmutableArray<INode>.Enumerator enumerator = partialTemplate.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    _ = enumerator.Current.Accept(this, itemCtx);
                }
            }
        }
        else
        {
            RenderContext innerCtx = context with
            {
                Data = context.Data?.Child(facade.RawValue) ?? new DataContext(facade.RawValue, null),
            };
            ImmutableArray<INode>.Enumerator enumerator = partialTemplate.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _ = enumerator.Current.Accept(this, innerCtx);
            }
        }

        return NoValue.Instance;
    }

    public NoValue VisitLineBreak(LineBreakNode node, RenderContext context)
    {
        for (int i = 0; i < node.Count; i++)
            context.Builder.AppendLine();
        return NoValue.Instance;
    }
}