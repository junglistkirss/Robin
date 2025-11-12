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


    public static bool IsStandaloneTag(this Token token) => token.Type is TokenType.Comment or TokenType.SectionOpen or TokenType.InvertedSection or TokenType.PartialCall or TokenType.PartialDefine or TokenType.SectionClose;
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

    private static LineBreakNode ParseLineBreak(string lineBreakType)
    {
        return lineBreakType switch
        {
            "\r" => LineBreakNode.InstanceReturn,
            "\n" => LineBreakNode.InstanceLine,
            _ => LineBreakNode.Instance,
        };
    }

    private static VariableNode ParseVariable(string variableExpression, bool isEscaped)
    {
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
        return new PartialCallNode(name, expressionNode);
    }

    private static PartialDefineNode ParsePartialDefine(ref NodeLexer lexer, Token startToken, out bool openStandalone, out bool closeStandalone)
    {
        string name = lexer.GetValue(startToken);
        List<INode> nodes = [];
        openStandalone = startToken.IsAtLineStart & startToken.IsAtLineEnd;
        closeStandalone = false;
        if (openStandalone)
            lexer.SkipNextLineBreak(out _);
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type is TokenType.SectionClose && lexer.GetValue(token).Equals(name))
            {
                closeStandalone = token.IsAtLineStart & token.IsAtLineEnd;

                if (closeStandalone)
                {
                    RemoveTrailingLineBreak(nodes);
                }

                lexer.SkipNextLineBreak(out _);

                break;
            }
            INode node = lexer.ParseNode(token, out bool isNodeOpenStandalone, out bool isNodeCloseStandalone);
            if (isNodeOpenStandalone && token.IsStandaloneTag())
                RemoveTrailingWhiteSpace(nodes);
            nodes.Add(node);
        }
        PartialDefineNode partial = new(name, [.. nodes]);

        return partial;

    }

    private static SectionNode ParseSection(ref NodeLexer lexer, Token startToken, bool inverted, out bool openStandalone, out bool closeStandalone)
    {
        string name = lexer.GetValue(startToken);
        IExpressionNode expNode = name.AsSpan().ParseExpression() ?? throw new Exception("Variable expression is invalid");
        List<INode> nodes = [];
        openStandalone = startToken.IsAtLineStart & startToken.IsAtLineEnd;
        closeStandalone = false;
        if (openStandalone)
            lexer.SkipNextLineBreak(out _);
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type is TokenType.SectionClose && lexer.GetValue(token).Equals(name))
            {
                closeStandalone = token.IsAtLineStart & token.IsAtLineEnd;

                if (closeStandalone)
                {
                    RemoveTrailingLineBreak(nodes);
                }
                
                lexer.SkipNextLineBreak(out INode[] after);
                    nodes.AddRange(after);
                

                break;
            }
            INode node = lexer.ParseNode(token, out bool isNodeOpenStandalone, out bool isNodeCloseStandalone);
            if (isNodeOpenStandalone && token.IsStandaloneTag())
                RemoveTrailingWhiteSpace(nodes);
            nodes.Add(node);
        }
        SectionNode section = new(expNode, [.. nodes], inverted);

        return section;
    }

    private static CommentNode ParseComment(this ref NodeLexer lexer, Token token, out bool openStandalone, out bool closeStandalone)
    {
        openStandalone = token.IsAtLineStart;
        closeStandalone = token.IsAtLineEnd;
        if (openStandalone && closeStandalone)
            lexer.SkipNextLineBreak(out _);
        return new(lexer.GetValue(token));

    }

    private static INode ParseNode(this ref NodeLexer lexer, Token token, out bool openStandalone, out bool closeStandalone)
    {
        openStandalone = false;
        closeStandalone = false;
        return token.Type switch
        {
            TokenType.Text => new TextNode(lexer.GetValue(token)),
            TokenType.Variable => ParseVariable(lexer.GetValue(token), false),
            TokenType.UnescapedVariable => ParseVariable(lexer.GetValue(token), true),
            TokenType.SectionOpen => ParseSection(ref lexer, token, false, out openStandalone, out closeStandalone),
            TokenType.InvertedSection => ParseSection(ref lexer, token, true, out openStandalone, out closeStandalone),
            TokenType.Comment => ParseComment(ref lexer, token, out openStandalone, out closeStandalone),
            TokenType.PartialCall => lexer.ParsePartialCall(token, out openStandalone, out closeStandalone),
            TokenType.PartialDefine => ParsePartialDefine(ref lexer, token, out openStandalone, out closeStandalone),
            TokenType.LineBreak => ParseLineBreak(lexer.GetValue(token)),
            TokenType.Whitepsaces => new WhitespaceNode(lexer.GetValue(token)),
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

    private static void SkipNextLineBreak(this ref NodeLexer lexer, out INode[] nodes)
    {
        List<INode> peekeds = [];
        while (lexer.TryPeekNextToken(out Token nextToken, out int next) && nextToken.Type is TokenType.LineBreak or TokenType.Whitepsaces)
        {
            peekeds.Add(lexer.ParseNode(nextToken, out _, out _));
            lexer.AdvanceTo(next);
        }
        nodes = [.. peekeds];
    }
}