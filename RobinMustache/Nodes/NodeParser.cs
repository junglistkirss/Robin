using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Nodes;
using RobinMustache.Abstractions.Variables;
using RobinMustache.Expressions;
using RobinMustache.Nodes;
using System.Collections.Immutable;

namespace RobinMustache.Nodes;

public static class NodeParser
{
    private readonly static IdentifierExpressionNode That = new(new VariablePath([ThisSegment.Instance]));

    public static bool TryParse(this ref NodeLexer lexer, out ImmutableArray<INode>? nodes)
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

    public static bool IsStandaloneTag(this Token token)
        => token.Type is TokenType.Comment or TokenType.SectionOpen or TokenType.InvertedSection or TokenType.PartialDefine or TokenType.SectionClose;

    public static ImmutableArray<INode> Parse(this ref NodeLexer lexer)
    {
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token token))
        {
            INode node = lexer.ParseNode(token, out bool openStandalone, out bool closeStandalone);
            bool isStandaloneTag = token.IsStandaloneTag();

            if (isStandaloneTag && openStandalone)
                RemoveTrailingWhiteSpace(nodes);
            nodes.Add(node);
        }
        return [.. nodes];
    }

    private static WhitespaceNode ParseWhitespaces(this ref NodeLexer lexer, Token token)
    {
        return new WhitespaceNode(lexer.GetValue(token));
    }
    private static TextNode ParseText(this ref NodeLexer lexer, Token token)
    {
        return new TextNode(lexer.GetValue(token));
    }
    private static LineBreakNode ParseLineBreak(this ref NodeLexer lexer, Token token)
    {
        string lineBreakValue = lexer.GetValue(token);
        return lineBreakValue switch
        {
            "\r" => LineBreakNode.InstanceReturn,
            "\n" => LineBreakNode.InstanceLine,
            "\r\n" => LineBreakNode.Instance,
            _ => new LineBreakNode(Environment.NewLine),
        };
    }

    private static VariableNode ParseVariable(this ref NodeLexer lexer, Token token, bool isEscaped)
    {
        string variableExpression = lexer.GetValue(token);
        ExpressionLexer exprLexer = new(variableExpression.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");

        return new VariableNode(node, isEscaped);
    }

    private static PartialCallNode ParsePartialCall(this ref NodeLexer lexer, Token token, out bool openStandalone, out bool closeStandalone)
    {
        string variableExpression = lexer.GetValue(token);
        int firstSpace = variableExpression.IndexOf(' ');
        IExpressionNode expressionNode;
        string name;
        if (firstSpace == -1)
        {
            name = variableExpression;
            expressionNode = That;
        }
        else
        {
            name = variableExpression.Substring(0, firstSpace);
            var expression = variableExpression.Substring(firstSpace + 1).AsSpan();
            expressionNode = expression.ParseExpression() ?? throw new Exception("Variable expression is invalid");
        }
        openStandalone = token.IsAtLineStart;
        closeStandalone = token.IsAtLineEnd;
        if(openStandalone && closeStandalone)
        lexer.SkipNextLineBreak(out _, out _);
        return new PartialCallNode(name, expressionNode);
    }

    private static PartialDefineNode ParsePartialDefine(this ref NodeLexer lexer, Token startToken, out bool openStandalone, out bool closeStandalone)
    {
        string name = lexer.GetValue(startToken);
        List<INode> nodes = [];
        openStandalone = startToken.IsAtLineStart & startToken.IsAtLineEnd;
        closeStandalone = false;
        if (openStandalone && lexer.SkipNextLineBreak(out _, out bool isEOF) && isEOF)
            throw new Exception("Unexpected end of partial define");
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type is TokenType.SectionClose && lexer.GetValue(token).Equals(name))
            {
                closeStandalone = token.IsAtLineStart & token.IsAtLineEnd;

                if (closeStandalone)
                {
                    RemoveTrailingLineBreak(nodes);
                    lexer.SkipNextLineBreak(out _, out _);
                }
                break;
            }
            INode node = lexer.ParseNode(token, out bool isNodeOpenStandalone, out bool _);
            if (isNodeOpenStandalone && token.IsStandaloneTag())
                RemoveTrailingWhiteSpace(nodes);
            nodes.Add(node);
        }
        PartialDefineNode partial = new(name, [.. nodes]);

        return partial;

    }

    private static SectionNode ParseSection(this ref NodeLexer lexer, Token startToken, bool inverted, out bool openStandalone, out bool closeStandalone)
    {
        string name = lexer.GetValue(startToken);
        LineBreakNode? trailingBreak = null;
        IExpressionNode expNode = name.AsSpan().ParseExpression() ?? throw new Exception("Variable expression is invalid");
        List<INode> nodes = [];
        openStandalone = startToken.IsAtLineStart & startToken.IsAtLineEnd;
        closeStandalone = false;
        if (openStandalone && lexer.SkipNextLineBreak(out _, out bool isEOF) && isEOF)
            throw new Exception("Unexpected EOF");
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type is TokenType.SectionClose && lexer.GetValue(token).Equals(name))
            {
                closeStandalone = token.IsAtLineStart & token.IsAtLineEnd;

                if (closeStandalone)
                {
                    RemoveTrailingLineBreak(nodes);
                    if (lexer.SkipNextLineBreak(out LineBreakNode? after, out _) && nodes.Count > 0)
                        trailingBreak = after;
                }
                break;
            }
            INode node = lexer.ParseNode(token, out bool isNodeOpenStandalone, out bool _);
            if (isNodeOpenStandalone && token.IsStandaloneTag())
                RemoveTrailingWhiteSpace(nodes);
            nodes.Add(node);
        }
        SectionNode section = new(expNode, [.. nodes], trailingBreak, inverted);

        return section;
    }

    private static CommentNode ParseComment(this ref NodeLexer lexer, Token token, out bool openStandalone, out bool closeStandalone)
    {
        openStandalone = token.IsAtLineStart;
        closeStandalone = token.IsAtLineEnd;
        if (openStandalone && closeStandalone)
            lexer.SkipNextLineBreak(out _, out _);
        return new(lexer.GetValue(token));

    }

    private static INode ParseNode(this ref NodeLexer lexer, Token token, out bool openStandalone, out bool closeStandalone)
    {
        openStandalone = false;
        closeStandalone = false;
        return token.Type switch
        {
            TokenType.Text => lexer.ParseText(token),
            TokenType.Variable => lexer.ParseVariable(token, false),
            TokenType.UnescapedVariable => lexer.ParseVariable(token, true),
            TokenType.SectionOpen => lexer.ParseSection(token, false, out openStandalone, out closeStandalone),
            TokenType.InvertedSection => lexer.ParseSection(token, true, out openStandalone, out closeStandalone),
            TokenType.Comment => lexer.ParseComment(token, out openStandalone, out closeStandalone),
            TokenType.PartialCall => lexer.ParsePartialCall(token, out openStandalone, out closeStandalone),
            TokenType.PartialDefine => lexer.ParsePartialDefine(token, out openStandalone, out closeStandalone),
            TokenType.LineBreak => lexer.ParseLineBreak(token),
            TokenType.Whitepsaces => lexer.ParseWhitespaces(token),
            _ => throw new InvalidTokenException($"Unsupported token {token} in section"),
        };
    }
    private static void RemoveTrailingWhiteSpace(this List<INode> nodes)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i] is WhitespaceNode)
                nodes.RemoveAt(i);
            else break;
        }
    }
    private static void RemoveTrailingLineBreak(this List<INode> nodes)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i] is WhitespaceNode)
                nodes.RemoveAt(i);
            else if (nodes[i] is LineBreakNode)
            {
                nodes.RemoveAt(i);
                break; // stop après avoir retiré un seul saut de ligne
            }
            else break;
        }
    }

    private static bool SkipNextLineBreak(this ref NodeLexer lexer, out LineBreakNode? lineBreak, out bool isEOF)
    {
        while (true)
        {
            lexer.TryPeekNextToken(out Token nextToken, out int next);
            if (nextToken.Type is TokenType.LineBreak or TokenType.Whitepsaces)
            {
                lexer.AdvanceTo(next);
                if (nextToken.Type is TokenType.LineBreak)
                {
                    lineBreak = lexer.ParseLineBreak(nextToken);
                    isEOF = false;
                    return true;
                }
            }
            else if (nextToken.Type is TokenType.EOF)
            {
                lineBreak = lexer.ParseLineBreak(nextToken);
                isEOF = true;
                return true;
            }
            else break;
        }
        lineBreak = null;
        isEOF = false;
        return false;
    }
}