using Robin.Abstractions;
using Robin.Abstractions.Accessors;
using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using System.Collections;
using System.Collections.Immutable;
using System.Net;
using System.Text;

namespace Robin.Internals;

internal sealed class StringNodeRender : INodeVisitor<NoValue, RenderContext<StringBuilder>>
{
    public readonly static StringNodeRender Instance = new();

    public NoValue VisitText(TextNode node, RenderContext<StringBuilder> context)
    {
        context.Builder.Append(node.Text);
        return NoValue.Instance;
    }
    public NoValue VisitComment(CommentNode node, RenderContext<StringBuilder> context)
    {
        return NoValue.Instance;
    }

    public NoValue VisitPartialDefine(PartialDefineNode node, RenderContext<StringBuilder> context)
    {
        return NoValue.Instance;
    }

    public NoValue VisitVariable(VariableNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, context.Data, out IDataFacade facade);
        if (facade.IsTrue(value))
        {
            if (node.IsUnescaped)
                context.Builder.Append(value);
            else
                context.Builder.Append(WebUtility.HtmlEncode($"{value}"));
        }
        return NoValue.Instance;
    }

    public NoValue VisitSection(SectionNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, context.Data, out IDataFacade facade);
        bool thruly = facade.IsTrue(value);

        if (!node.Inverted && thruly || node.Inverted && !thruly)
        {
            return RenderTree(context, value, facade, node.Children);
        }

        return NoValue.Instance;
    }

    public NoValue VisitPartialCall(PartialCallNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, context.Data, out IDataFacade facade);

        if (facade.IsTrue(value) && context.Partials.TryGetValue(node.PartialName, out ImmutableArray<INode> partialTemplate))
        {
            context = context with
            {
                Partials = partialTemplate.ExtractsPartials(context.Partials)
            };
            return RenderTree(context, value, facade, partialTemplate);

        }
        return NoValue.Instance;
    }

    private NoValue RenderTree(RenderContext<StringBuilder> context, object? value, IDataFacade facade, ImmutableArray<INode> partialTemplate)
    {
        void action(object? o)
        {
            RenderContext<StringBuilder> itemCtx = context with
            {
                Data = context.Data?.Child(o) ?? new DataContext(o, null),
            };
            foreach (var node in partialTemplate)
            {
                node.Accept(this, itemCtx);
            }
        }

        if (facade.IsCollection(value, out IIterator? collection))
            collection.Iterate(action);
        else
            action(value);

        return NoValue.Instance;
    }

    public NoValue VisitLineBreak(LineBreakNode node, RenderContext<StringBuilder> context)
    {
        for (int i = 0; i < node.Count; i++)
            context.Builder.AppendLine();
        return NoValue.Instance;
    }
}