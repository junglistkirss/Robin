using System.Collections.Immutable;

namespace Robin.Nodes;

public static class ExpressionParser
{
    private enum PendingType
    {
        Identifier,
        FunctionCall,
        Group,
    }
    private record struct Pending(string Name, PendingType Type, List<Pending> Arguments);

    private static IExpressionNode Qualify(this Pending pending)
    {
        if (pending.Type == PendingType.Identifier)
        {
            return new IdentifierNode(pending.Name);
        }
        else if (pending.Type == PendingType.FunctionCall)
        {
            return new FunctionCallNode(pending.Name, [.. pending.Arguments.Select(Qualify)]);
        }
        else if (pending.Type == PendingType.Group)
        {
            if(pending.Arguments.Count == 0)
                throw new InvalidOperationException("Empty group");
            else if (pending.Arguments.Count == 2)
                return new BinaryOperatorNode(pending.Name, pending.Arguments[0].Qualify(), pending.Arguments[1].Qualify());
            else
                throw new InvalidOperationException("too long group");

        }
        else
        {
            throw new InvalidOperationException("Unsupported pending type");
        }
    }

    public static ImmutableArray<IExpressionNode> Parse(this ref ExpressionLexer lexer)
    {
        List<IExpressionNode> output = [];
        Stack<Pending> funcStack = new();
        //Stack<int> functionArgCountStack = new();
        //ExpressionToken? previousToken = null;
        ExpressionToken? currentToken = null;
        ExpressionToken? nextToken = null;

        // Pre-fetch first token
        if (lexer.TryGetNextToken(out ExpressionToken? token))
        {
            currentToken = token.Value;
        }

        while (currentToken != null)
        {
            // Peek ahead for next token
            if (lexer.TryGetNextToken(out ExpressionToken? peekedToken))
            {
                nextToken = peekedToken.Value;
            }
            else
            {
                nextToken = null;
            }
            ExpressionToken current = currentToken.Value;
            bool isPending = funcStack.TryPeek(out Pending pending);
            switch (current.Type)
            {
                case ExpressionType.Identifier:
                    string currentValue = lexer.GetValue(current);
                    // Check if next token is '(' for function call
                    if (nextToken.HasValue && nextToken.Value.Type == ExpressionType.LeftParenthesis)
                    {
                        if (isPending)
                        {
                            pending.Arguments.Add(new Pending(currentValue, PendingType.FunctionCall, [])); // Push function name
                        }
                        else
                        {
                            funcStack.Push(new Pending(currentValue, PendingType.FunctionCall, []));
                        }
                        currentToken = null; // Consume '('
                    }
                    else
                    {
                        if (isPending)
                            pending.Arguments.Add(new Pending(currentValue, PendingType.Identifier, []));
                        else
                            output.Add(new IdentifierNode(currentValue));
                        currentToken = nextToken; // next not yet consumed

                        //// Track argument count in function calls
                        //if (functionArgCountStack.Count > 0 &&
                        //    (previousToken == null || previousToken.Value.Type == ExpressionType.LeftParenthesis))
                        //{
                        //    int count = functionArgCountStack.Pop();
                        //    functionArgCountStack.Push(count + 1);
                        //}
                    }
                    break;

                //case ExpressionType.Operator:
                //    // Handle unary operators
                //    bool isUnary = previousToken == null ||
                //                   previousToken.Value.Type == ExpressionType.Operator ||
                //                   previousToken.Value.Type == ExpressionType.LeftParenthesis;
                //    string currentValue = lexer.GetValue(current);
                //    if (isUnary && (currentValue == "+" || currentValue == "-"))
                //    {
                //        // Push unary operator with highest precedence
                //        operatorStack.Push(current);//, "unary" + currentValue));
                //    }
                //    else if (currentValue == ",")
                //    {
                //        // Comma separates function arguments
                //        // Pop operators until we hit '('
                //        while (operatorStack.Count > 0)
                //        {
                //            var top = operatorStack.Peek();
                //            string topValue = lexer.GetValue(top);
                //            if (top.Type == ExpressionType.LeftParenthesis)
                //                break;

                //            operatorStack.Pop();
                //            if (top.Type == ExpressionType.Operator)
                //            {
                //                if (OperatorPrecedence.TryGetValue(topValue, out int prec))
                //                {
                //                    output.Add(new OperatorNode(topValue, prec));
                //                }
                //            }
                //        }

                //        // Increment argument count
                //        if (functionArgCountStack.Count > 0)
                //        {
                //            int count = functionArgCountStack.Pop();
                //            functionArgCountStack.Push(count + 1);
                //        }
                //    }
                //    else
                //    {
                //        // Handle binary operators with precedence
                //        if (!OperatorPrecedence.TryGetValue(currentValue, out int precedence))
                //        {
                //            throw new InvalidOperationException($"Unknown operator: {currentValue}");
                //        }

                //        while (operatorStack.Count > 0)
                //        {
                //            var top = operatorStack.Peek();
                //            if (top.Type == ExpressionType.LeftParenthesis)
                //                break;

                //            if (top.Type == ExpressionType.Operator)
                //            {
                //                string topValue = lexer.GetValue(top);
                //                if (!OperatorPrecedence.TryGetValue(topValue, out int topPrecedence))
                //                    break;

                //                // Right-associative for ^ (power), left-associative for others
                //                bool isRightAssociative = currentValue == "^";
                //                if (isRightAssociative ? precedence < topPrecedence : precedence <= topPrecedence)
                //                {
                //                    operatorStack.Pop();
                //                    output.Add(new OperatorNode(topValue, topPrecedence));
                //                }
                //                else
                //                {
                //                    break;
                //                }
                //            }
                //            else
                //            {
                //                break;
                //            }
                //        }

                //        operatorStack.Push(current);
                //    }
                //    break;

                //case ExpressionType.LeftParenthesis:
                //    operatorStack.Push(current);
                //    break;

                case ExpressionType.RightParenthesis:

                    if (!isPending)
                        throw new Exception("Invalid parenthesis");

                    Pending pop = funcStack.Pop();
                    if (funcStack.TryPeek(out Pending topPending))
                    {
                        topPending.Arguments.Add(pop);
                    }
                    else
                    {
                        output.Add(pop.Qualify());
                    }
                    currentToken = null; // consume
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported token type {current.Type}");
            }
            if (currentToken is null)
            {
                lexer.TryGetNextToken(out currentToken);
            }
        }
        if (funcStack.Count > 0)
            throw new Exception("Malformed expression");

        return [.. output];
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

