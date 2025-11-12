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

    public static ImmutableArray<INode> Parse(this ref NodeLexer lexer)
    {
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token token))
        {
            nodes.Add(lexer.ParseNode(token, out bool isNodeStandalone));
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

    private static PartialCallNode ParsePartialCall(string variableExpression)
    {
        int firstSpace = variableExpression.IndexOf(' ');
        if (firstSpace == -1)
        {
            return new PartialCallNode(variableExpression, That);
        }
        else
        {
            string name = variableExpression.Substring(0, firstSpace);
            ExpressionLexer exprLexer = new(variableExpression.Substring(firstSpace + 1).AsSpan());
            IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
            return new PartialCallNode(name, node);
        }
    }

    private static PartialDefineNode ParsePartialDefine(ref NodeLexer lexer, Token startToken, out bool isStandalone)
    {
        isStandalone = false;
        string name = lexer.GetValue(startToken);
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type == TokenType.SectionClose && lexer.GetValue(token).Equals(name))
                break;

            nodes.Add(lexer.ParseNode(token, out bool isNodeStandalone));
        }
        PartialDefineNode partial = new(name, [.. nodes]);

        return partial;

    }

    private static SectionNode ParseSection(ref NodeLexer lexer, Token startToken, bool inverted, out bool isStandalone)
    {
        isStandalone = false;
        string name = lexer.GetValue(startToken);
        ExpressionLexer exprLexer = new(name.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
        List<INode> nodes = [];
        if (startToken.IsAtlineStart)
            lexer.SkipNextLineBreak(startToken.Type);
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type == TokenType.SectionClose && lexer.GetValue(token).Equals(name))
            {
                if (token.IsAtlineStart)
                    RemoveTrailingLineBreak(nodes);
                isStandalone = token.IsAtLineEnd && startToken.IsAtlineStart;
                //if (token.IsAtLineEnd)
                //    lexer.SkipNextLineBreak(token.Type);
                break;
            }

            nodes.Add(lexer.ParseNode(token, out bool isNodeStandalone));
        }
        SectionNode section = new(node, [.. nodes], inverted);

        return section;
    }

    private static INode ParseNode(this ref NodeLexer lexer, Token token,out bool isNodeStandalone)
    {
        isNodeStandalone = false;
        switch (token.Type)
        {
            case TokenType.Text:
                return new TextNode(lexer.GetValue(token));
            case TokenType.Variable:
                return ParseVariable(lexer.GetValue(token), false);
            case TokenType.UnescapedVariable:
                return ParseVariable(lexer.GetValue(token), true);
            case TokenType.SectionOpen:
                var section = ParseSection(ref lexer, token, false, out bool isStandaloneSection);
                isNodeStandalone = isStandaloneSection;
                return section;
            case TokenType.InvertedSection:
                var invertedSection = ParseSection(ref lexer, token, true, out bool isStandaloneSectionInverted);
                isNodeStandalone = isStandaloneSectionInverted;
                return invertedSection;
            case TokenType.Comment:
                return new CommentNode(lexer.GetValue(token));
            case TokenType.PartialCall:
                return ParsePartialCall(lexer.GetValue(token));
            case TokenType.PartialDefine:
                return ParsePartialDefine(ref lexer, token, out bool isStandalonePartialDefine);
            case TokenType.LineBreak:
                return ParseLineBreak(lexer.GetValue(token));
            case TokenType.Whitepsaces:
                //if(token.IsAtlineStart && lexer.TryPeekNextToken(out Token next, out _) && next.Type is not TokenType.SectionOpen or TokenType.SectionClose or TokenType.InvertedSection or TokenType.PartialDefine or TokenType.PartialCall)
                //    return new WhitespaceNode(string.Empty);
                //else
                return new WhitespaceNode(lexer.GetValue(token));
            default:
                throw new InvalidTokenException($"Unsupported token {token} in section");
        }
    }
    private static void RemoveTrailingLineBreak(List<INode> nodes)
    {
        while (nodes.Count > 0 && nodes[nodes.Count - 1] is LineBreakNode or WhitespaceNode)
            nodes.RemoveAt(nodes.Count - 1);
    }
    private static void SkipNextLineBreak(this ref NodeLexer lexer, TokenType currentType)
    {
        if (currentType is TokenType.SectionOpen or TokenType.SectionClose or TokenType.InvertedSection or TokenType.PartialDefine or TokenType.PartialCall)
            while (lexer.TryPeekNextToken(out Token nextToken, out int next) && nextToken.Type is TokenType.LineBreak or TokenType.Whitepsaces)
            {
                lexer.AdvanceTo(next);
            }
    }
}