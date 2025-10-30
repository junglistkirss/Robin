namespace Robin.Nodes;

public static class ExpressionParser
{
    /// <summary>
    /// Parse une expression à partir d'un lexer (méthode d'extension)
    /// </summary>
    public static IExpressionNode? Parse(this ref ExpressionLexer lexer)
    {
        if (!lexer.TryGetNextToken(out ExpressionToken? currentToken))
            return null;

        IExpressionNode result = ParseExpression(ref lexer, currentToken.Value);

        // Vérifier qu'il n'y a plus de tokens
        if (lexer.TryGetNextToken(out ExpressionToken? extraToken))
            throw new Exception($"Tokens inattendus après la fin: {extraToken.Value.Type}");

        return result;
    }

    // Niveau 1 : Addition et Soustraction
    private static IExpressionNode ParseExpression(ref ExpressionLexer lexer, ExpressionToken currentToken)
    {
        IExpressionNode left = ParseTerm(ref lexer, currentToken);

        while (lexer.TryGetNextToken(out ExpressionToken? nextToken))
        {
            currentToken = nextToken.Value;

            if (currentToken.Type != ExpressionType.Operator)
                break;

            string op = lexer.GetValue(currentToken);
            if (op != "+" && op != "-")
                break;

            if (!lexer.TryGetNextToken(out ExpressionToken? rightToken))
                throw new Exception("Opérande attendu après opérateur");

            IExpressionNode right = ParseTerm(ref lexer, rightToken.Value);
            left = new BinaryOperationNode(left, op, right);
        }

        return left;
    }

    // Niveau 2 : Multiplication et Division
    private static IExpressionNode ParseTerm(ref ExpressionLexer lexer, ExpressionToken currentToken)
    {
        IExpressionNode left = ParsePower(ref lexer, currentToken);

        while (lexer.TryGetNextToken(out ExpressionToken? nextToken))
        {
            currentToken = nextToken.Value;

            if (currentToken.Type != ExpressionType.Operator)
                break;

            string op = lexer.GetValue(currentToken);
            if (op != "*" && op != "/")
                break;

            if (!lexer.TryGetNextToken(out ExpressionToken? rightToken))
                throw new Exception("Opérande attendu après opérateur");

            IExpressionNode right = ParsePower(ref lexer, rightToken.Value);
            left = new BinaryOperationNode(left, op, right);
        }

        return left;
    }

    // Niveau 3 : Puissance (associativité à droite)
    private static IExpressionNode ParsePower(ref ExpressionLexer lexer, ExpressionToken currentToken)
    {
        IExpressionNode left = ParseUnary(ref lexer, currentToken);

        if (lexer.TryGetNextToken(out ExpressionToken? nextToken))
        {
            currentToken = nextToken.Value;

            if (currentToken.Type == ExpressionType.Operator)
            {
                string op = lexer.GetValue(currentToken);
                if (op == "^")
                {
                    if (!lexer.TryGetNextToken(out ExpressionToken? rightToken))
                        throw new Exception("Opérande attendu après opérateur");

                    IExpressionNode right = ParsePower(ref lexer, rightToken.Value);
                    return new BinaryOperationNode(left, op, right);
                }
            }
        }

        return left;
    }

    // Niveau 4 : Opérateurs unaires
    private static IExpressionNode ParseUnary(ref ExpressionLexer lexer, ExpressionToken currentToken)
    {
        if (currentToken.Type == ExpressionType.Operator)
        {
            string op = lexer.GetValue(currentToken);
            if (op == "+" || op == "-")
            {
                if (!lexer.TryGetNextToken(out ExpressionToken? operandToken))
                    throw new Exception("Opérande attendu après opérateur unaire");

                return new UnaryOperationNode(op, ParseUnary(ref lexer, operandToken.Value));
            }
        }

        return ParsePrimary(ref lexer, currentToken);
    }

    // Niveau 5 : Éléments primaires
    private static IExpressionNode ParsePrimary(ref ExpressionLexer lexer, ExpressionToken currentToken)
    {
        // Parenthèses
        if (currentToken.Type == ExpressionType.LeftParenthesis)
        {
            if (!lexer.TryGetNextToken(out ExpressionToken? innerToken))
                throw new Exception("Expression attendue après '('");

            IExpressionNode node = ParseExpression(ref lexer, innerToken.Value);

            if (!lexer.TryGetNextToken(out ExpressionToken? closingToken) ||
                closingToken.Value.Type != ExpressionType.RightParenthesis)
                throw new Exception("')' attendu");

            return node;
        }

        // Literal
        if (currentToken.Type == ExpressionType.Literal)
        {
            string value = lexer.GetValue(currentToken);
            return new LiteralNode(value);
        }

        // Number
        if (currentToken.Type == ExpressionType.Number)
        {
            string value = lexer.GetValue(currentToken);
            double number = double.Parse(value);
            return new NumberNode(number);
        }

        // Identifier (variable ou fonction)
        if (currentToken.Type == ExpressionType.Identifier)
        {
            string name = lexer.GetValue(currentToken);

            // Vérifier si c'est un appel de fonction
            if (lexer.TryGetNextToken(out ExpressionToken? nextToken) &&
                nextToken.Value.Type == ExpressionType.LeftParenthesis)
            {
                List<IExpressionNode> arguments = new List<IExpressionNode>();

                // Vérifier s'il y a des arguments
                if (lexer.TryGetNextToken(out ExpressionToken? argToken))
                {
                    if (argToken.Value.Type != ExpressionType.RightParenthesis)
                    {
                        // Parser le premier argument
                        arguments.Add(ParseExpression(ref lexer, argToken.Value));

                        // Parser les arguments suivants
                        while (lexer.TryGetNextToken(out ExpressionToken? sepToken))
                        {
                            if (sepToken.Value.Type == ExpressionType.RightParenthesis)
                                break;

                            if (sepToken.Value.Type != ExpressionType.Operator ||
                                lexer.GetValue(sepToken.Value) != ",")
                                throw new Exception("',' ou ')' attendu dans la liste d'arguments");

                            if (!lexer.TryGetNextToken(out ExpressionToken? nextArgToken))
                                throw new Exception("Argument attendu après ','");

                            arguments.Add(ParseExpression(ref lexer, nextArgToken.Value));
                        }
                    }
                }
                else
                {
                    throw new Exception("')' attendu");
                }

                return new FunctionCallNode(name, [..arguments]);
            }

            // Sinon, c'est une variable
            return new IdentifierNode(name);
        }

        throw new Exception($"Token inattendu: {currentToken.Type}");
    }
}
