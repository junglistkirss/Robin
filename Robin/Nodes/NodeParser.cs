using Robin.Contracts;
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
    private readonly static IdentifierExpressionNode That = new(new VariablePath([ThisSegment.Instance]));

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
                    nodes.Add(new TextNode(token.Value));
                    break;
                case TokenType.Variable:
                    nodes.Add(lexer.ParseVariable(token.Value, false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(lexer.ParseVariable(token.Value, true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(ParseSection(ref lexer, token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(ParseSection(ref lexer, token.Value, true));
                    break;
                case TokenType.Comment:
                    nodes.Add(new CommentNode(token.Value));
                    break;
                case TokenType.PartialDefine:
                    nodes.Add(lexer.ParsePartial(token.Value));
                    break;
                case TokenType.PartialCall:
                    nodes.Add(lexer.ParsePartialCall(token.Value));
                    break;
                case TokenType.LineBreak:
                    nodes.Add(lexer.AggregateLineBreaks());
                    break;
                default:
                    throw new InvalidTokenException($"Unsupported token type {token.Value.Type}");
            }
        }
        return [.. nodes];
    }

    private static LineBreakNode AggregateLineBreaks(this ref NodeLexer lexer)
    {
        int c = 1;
        while (lexer.TryPeekNextToken(out Token? nextToken, out int position) && nextToken.Value.Type == TokenType.LineBreak)
        {
            c++;
            lexer.AdvanceTo(position);
        }
        return new LineBreakNode(c);
    }

    private static VariableNode ParseVariable(this ref NodeLexer lexer, Extract range, bool isEscaped)
    {
        ExpressionLexer exprLexer = new(ref lexer, range);
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");

        return new VariableNode(node, isEscaped);
    }

    private static PartialCallNode ParsePartialCall(this ref NodeLexer lexer, Extract variableExpression)
    {
        int firstSpace = 0;
        int len = variableExpression.Length;
        while (firstSpace < len && !char.IsWhiteSpace(lexer[variableExpression.Start + firstSpace]))
            firstSpace++;
        if (firstSpace < len)
        {
            ExpressionLexer exprLexer = new(ref lexer, variableExpression.Offset(firstSpace+1));
            IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
            return new PartialCallNode(variableExpression.Limit(firstSpace), node);
        }
        else
        {
            return new PartialCallNode(variableExpression, That);
        }
    }

    private static PartialDefineNode ParsePartial(this ref NodeLexer lexer, Token startToken)
    {
        ReadOnlySpan<char> name = lexer[startToken];
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token))
        {
            if (token.Value.Type == TokenType.SectionClose && lexer[token.Value].SequenceEqual(name))
                break;

            switch (token.Value.Type)
            {
                case TokenType.Text:
                    nodes.Add(new TextNode(token.Value));
                    break;
                case TokenType.Variable:
                    nodes.Add(lexer.ParseVariable(token.Value, false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(lexer.ParseVariable(token.Value, true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(lexer.ParseSection(token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(lexer.ParseSection(token.Value, true));
                    break;
                case TokenType.Comment:
                    nodes.Add(new CommentNode(token.Value));
                    break;
                case TokenType.PartialCall:
                    nodes.Add(lexer.ParsePartialCall(token.Value));
                    break;
                case TokenType.PartialDefine:
                    nodes.Add(lexer.ParsePartial(token.Value));
                    break;
                case TokenType.LineBreak:
                    nodes.Add(lexer.AggregateLineBreaks());
                    break;
                default:
                    throw new InvalidTokenException($"Unsupported token type {token.Value.Type} in section");
            }
        }
        PartialDefineNode partial = new(startToken, [.. nodes]);

        return partial;

    }

    private static SectionNode ParseSection(this ref NodeLexer lexer, Token startToken, bool inverted)
    {
        ReadOnlySpan<char> name = lexer[startToken];
        ExpressionLexer exprLexer = new(ref lexer, startToken);
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token))
        {
            if (token.Value.Type == TokenType.SectionClose && lexer[token.Value].SequenceEqual(name))
                break;

            switch (token.Value.Type)
            {
                case TokenType.Text:
                    nodes.Add(new TextNode(token.Value));
                    break;
                case TokenType.Variable:
                    nodes.Add(lexer.ParseVariable(token.Value, false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(lexer.ParseVariable(token.Value, true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(lexer.ParseSection(token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(lexer.ParseSection(token.Value, true));
                    break;
                case TokenType.Comment:
                    nodes.Add(new CommentNode(token.Value));
                    break;
                case TokenType.LineBreak:
                    nodes.Add(lexer.AggregateLineBreaks());
                    break;
                default:
                    throw new InvalidTokenException($"Unsupported token type {token.Value.Type} in section");
            }
        }
        SectionNode section = new(node, [.. nodes], inverted);

        return section;
    }
}