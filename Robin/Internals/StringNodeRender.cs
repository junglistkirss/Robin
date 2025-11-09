using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Iterators;
using Robin.Contracts.Nodes;
using Robin.Extensions;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

namespace Robin.Internals;

internal sealed class StringNodeRender(IEnumerable<IPartialLoader> loaders) : INodeVisitor<RenderContext<StringBuilder>>
{

    public void VisitText(TextNode node, RenderContext<StringBuilder> context)
    {
        context.Builder.Append(node.Text);
    }
    public void VisitComment(CommentNode node, RenderContext<StringBuilder> context)
    {
    }

    public void VisitPartialDefine(PartialDefineNode node, RenderContext<StringBuilder> context)
    {
    }

    public void VisitVariable(VariableNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, DataContext.Current, out IDataFacade facade);
        if (facade.IsTrue(value))
        {
            string? str;
            if (value is string s)
                str = s;
            else
                str = value.ToString();
            if (str is not null)
            {
                if (node.IsUnescaped)
                    context.Builder.Append(str);
                else
                    context.Builder.Append(str.Escape());
            }

        }
    }

    public void VisitSection(SectionNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, DataContext.Current, out IDataFacade facade);
        bool thruly = facade.IsTrue(value);

        if ((!node.Inverted && thruly) || (node.Inverted && !thruly))
        {
            RenderTree(context, value, facade, node.Children.AsSpan());
        }

    }

    public void VisitPartialCall(PartialCallNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, DataContext.Current, out IDataFacade facade);

        if (facade.IsTrue(value))
        {
            foreach (IPartialLoader loader in loaders)
            {
                if (loader.Load(node.PartialName, context, out ImmutableArray<INode> partialTemplate))
                {
                    ReadOnlySpan<INode> span = partialTemplate.AsSpan();

                    ReadOnlyDictionary<string, ImmutableArray<INode>> tempPartials = span.ExtractsPartials(context.Partials).AsReadOnly();
                    using (new PartialsScope<StringBuilder>(context, tempPartials))
                    {
                        RenderTree(context, value, facade, span);
                    }
                    return;
                }
            }
        }
    }

    private void RenderTree(RenderContext<StringBuilder> context, object? value, IDataFacade facade, ReadOnlySpan<INode> partialTemplate)
    {
        if (facade.IsCollection(value, out IIterator? iterator))
            iterator.Iterate(value, context, partialTemplate, this);
        else
            using (DataContext.Push(value))
            {
                foreach (INode node in partialTemplate)
                {
                    node.Accept(this, context);
                }
            }
    }

    public void VisitLineBreak(LineBreakNode node, RenderContext<StringBuilder> context)
    {
        for (int i = 0; i < node.Count; i++)
            context.Builder.AppendLine();
    }
}