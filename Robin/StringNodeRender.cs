using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using System.Collections;
using System.Collections.Immutable;
using System.Net;
using System.Text;

namespace Robin.Abstractions;

public sealed class StringNodeRender : INodeVisitor<NoValue, RenderContext<StringBuilder>>
{
    public readonly static StringNodeRender Instance = new();

    public NoValue VisitText(TextNode node, RenderContext<StringBuilder> context)
    {
        context.Builder.Append(context.Context.GetValue(node.Text));
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

    public NoValue VisitVariable(VariableNode node, RenderContext<StringBuilder> args)
    {
        object? value = args.Evaluator.Resolve(node.Expression, args.Context, out IDataFacade facade);
        if (facade.IsTrue(value))
        {
            if (node.IsUnescaped)
                args.Builder.Append(value?.ToString());
            else
                args.Builder.Append(WebUtility.HtmlEncode(value?.ToString()));
        }
        return NoValue.Instance;
    }

    public NoValue VisitSection(SectionNode node, RenderContext<StringBuilder> args)
    {
        object? value = args.Evaluator.Resolve(node.Expression, args.Context, out IDataFacade facade);
        bool thruly = facade.IsTrue(value);

        if ((!node.Inverted && thruly) || (node.Inverted && !thruly))
        {
            return RenderTree(args, value, facade, node.Children);
        }

        return NoValue.Instance;
    }

    public NoValue VisitPartialCall(PartialCallNode node, RenderContext<StringBuilder> context)
    {
        //object?  value = context.Evaluator.Resolve(node.Expression, context.Data, out IDataFacade facade);

        //if (facade.IsTrue(value) && context.Partials.TryGetValue(node.PartialName, out ImmutableArray<INode> partialTemplate))
        //{
        //    context = context with
        //    {
        //        Partials = partialTemplate.ExtractsPartials(context.Partials)
        //    };
        //    return RenderTree(context, value, facade, partialTemplate);

        //}
        return NoValue.Instance;
    }

    private NoValue RenderTree(RenderContext<StringBuilder> args, object? value, IDataFacade facade, ImmutableArray<INode> partialTemplate)
    {
        if (facade.IsCollection(value, out IEnumerator? collection))
        {
            while (collection.MoveNext())
            {
                object? item = collection.Current;
                RenderContext<StringBuilder> itemCtx = args with
                {
                    Context = args.Context with
                    {
                        Data = args.Context.Data?.Child(item) ?? new DataContext(item, null),
                    }
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
            RenderContext<StringBuilder> innerCtx = args with
            {
                Context = args.Context with
                {
                    Data = args.Context.Data?.Child(value) ?? new DataContext(value, null),
                }
            };
            ImmutableArray<INode>.Enumerator enumerator = partialTemplate.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _ = enumerator.Current.Accept(this, innerCtx);
            }
        }

        return NoValue.Instance;
    }

    public NoValue VisitLineBreak(LineBreakNode node, RenderContext<StringBuilder> context)
    {
        for (int i = 0; i < node.Count; i++)
            context.Builder.AppendLine();
        return NoValue.Instance;
    }
}