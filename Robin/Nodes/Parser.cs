using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Nodes;

public static class Parser
{
    public static ImmutableArray<INode> Parse(this ref Lexer lexer)
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
                    nodes.Add(ParseExpression(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(ParseExpression(lexer.GetValue(token.Value), true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(ParseSection(ref lexer, token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(ParseSection(ref lexer, token.Value, true));
                    break;
                case TokenType.Comment:
                    // Ignore for now
                    break;
                // case TokenType.EOF:
                //     return nodes;
                default:
                    throw new InvalidOperationException($"Unsupported token type {token.Value.Type}");
            }
        }
        return [.. nodes];
    }

    private static INode ParseExpression(string variableExpression, bool isEscaped)
    {
        ExpressionLexer exprLexer = new(variableExpression.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");

        return new VariableNode(node, isEscaped);
    }

    private static INode ParseSection(ref Lexer lexer, Token startToken, bool inverted)
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
                    nodes.Add(ParseExpression(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(ParseExpression(lexer.GetValue(token.Value), true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(ParseSection(ref lexer, token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(ParseSection(ref lexer, token.Value, true));
                    break;
                case TokenType.Comment:
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported token type {token.Value.Type} in section");
            }
        }
        SectionNode section = new(node, [.. nodes], inverted);

        return section;

    }
}


//// Example usage and testing
//public class Program
//{
//    public static void Main()
//    {
//        TestExpression("3 + 4 * 2");
//        TestExpression("(3 + 4) * 2");
//        TestExpression("foo(1, 2, 3)");
//        TestExpression("max(a, b) + min(c, d)");
//        TestExpression("-5 + 3");
//        TestExpression("a > 5 && b < 10");
//        TestExpression("x == 5 || y == 10");
//        TestExpression("2 ^ 3 ^ 2");
//        TestExpression("price * 1.2");
//        TestExpression("obj.property[0]");
//    }

//    static void TestExpression(string expr)
//    {
//        Console.WriteLine($"\nParsing: {expr}");
//        try
//        {
//            var parser = new ExpressionParser(expr.AsSpan());
//            var result = parser.Parse();
//            Console.WriteLine($"Result: {result}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }
//}

