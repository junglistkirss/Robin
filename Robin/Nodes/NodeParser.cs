using Robin.Contracts.Expressions;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using Robin.Expressions;
using Robin.Nodes;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Nodes;

public static class NodeParser
{
    private readonly static IdentifierExpressionNode That = new(new VariablePath([ThisAccessor.Instance]));

    public static bool TryParse(this ref NodeLexer lexer, [NotNullWhen(true)] out ImmutableArray<INode>? nodes)
    {
        try
        {
            nodes = Parse(ref lexer);
            return true;
        }
        catch (Exception)
        {
            nodes = null;
            return false;
        }
    }

    public static ImmutableArray<INode> Parse(this ref NodeLexer lexer)
    {
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token))
        {
            switch (token.Value.Type)
            {
                case TokenType.Text:
                    nodes.Add(new TextNode(lexer.GetValue(token.Value)));
                    break;
                case TokenType.Variable:
                    nodes.Add(ParseVariable(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(ParseVariable(lexer.GetValue(token.Value), true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(ParseSection(ref lexer, token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(ParseSection(ref lexer, token.Value, true));
                    break;
                case TokenType.Comment:
                    nodes.Add(new CommentNode(lexer.GetValue(token.Value)));
                    break;
                case TokenType.PartialDefine:
                    nodes.Add(ParsePartial(ref lexer, token.Value));
                    break;
                case TokenType.PartialCall:
                    nodes.Add(ParsePartialCall(lexer.GetValue(token.Value)));
                    break;
                case TokenType.LineBreak:
                    nodes.Add(AggregateLineBreaks(ref lexer));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported token type {token.Value.Type}");
            }
        }
        return [.. nodes];
    }

    private static LineBreakNode AggregateLineBreaks(ref NodeLexer lexer)
    {
        int c = 1;
        while (lexer.TryPeekNextToken(out Token? nextToken, out int position) && nextToken.Value.Type == TokenType.LineBreak)
        {
            c++;
            lexer.AdvanceTo(position);
        }
        return new LineBreakNode(c);
    }

    private static VariableNode ParseVariable(string variableExpression, bool isEscaped)
    {
        ExpressionLexer exprLexer = new(variableExpression.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");

        return new VariableNode(node, isEscaped);
    }

    private static PartialCallNode ParsePartialCall(string variableExpression)
    {
        int firstSpace = variableExpression.IndexOf(' ');
        if (firstSpace == -1)
        {
            return new PartialCallNode(variableExpression, That);
        }
        else
        {
            string name = variableExpression[..firstSpace];
            ExpressionLexer exprLexer = new(variableExpression[(firstSpace + 1)..].AsSpan());
            IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
            return new PartialCallNode(name, node);
        }
    }

    private static PartialDefineNode ParsePartial(ref NodeLexer lexer, Token startToken)
    {
        string name = lexer.GetValue(startToken);
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token))
        {
            if (token.Value.Type == TokenType.SectionClose && lexer.GetValue(token.Value).Equals(name))
                break;

            switch (token.Value.Type)
            {
                case TokenType.Text:
                    nodes.Add(new TextNode(lexer.GetValue(token.Value)));
                    break;
                case TokenType.Variable:
                    nodes.Add(ParseVariable(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(ParseVariable(lexer.GetValue(token.Value), true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(ParseSection(ref lexer, token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(ParseSection(ref lexer, token.Value, true));
                    break;
                case TokenType.Comment:
                    nodes.Add(new CommentNode(lexer.GetValue(token.Value)));
                    break;
                case TokenType.PartialCall:
                    nodes.Add(ParsePartialCall(lexer.GetValue(token.Value)));
                    break;
                case TokenType.PartialDefine:
                    nodes.Add(ParsePartial(ref lexer, token.Value));
                    break;
                case TokenType.LineBreak:
                    nodes.Add(AggregateLineBreaks(ref lexer));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported token type {token.Value.Type} in section");
            }
        }
        PartialDefineNode partial = new(name, [.. nodes]);

        return partial;

    }

    private static SectionNode ParseSection(ref NodeLexer lexer, Token startToken, bool inverted)
    {
        string name = lexer.GetValue(startToken);
        ExpressionLexer exprLexer = new(name.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token))
        {
            if (token.Value.Type == TokenType.SectionClose && lexer.GetValue(token.Value).Equals(name))
                break;

            switch (token.Value.Type)
            {
                case TokenType.Text:
                    nodes.Add(new TextNode(lexer.GetValue(token.Value)));
                    break;
                case TokenType.Variable:
                    nodes.Add(ParseVariable(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(ParseVariable(lexer.GetValue(token.Value), true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(ParseSection(ref lexer, token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(ParseSection(ref lexer, token.Value, true));
                    break;
                case TokenType.Comment:
                    nodes.Add(new CommentNode(lexer.GetValue(token.Value)));
                    break;
                case TokenType.LineBreak:
                    nodes.Add(AggregateLineBreaks(ref lexer));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported token type {token.Value.Type} in section");
            }
        }
        SectionNode section = new(node, [.. nodes], inverted);

        return section;
    }
}