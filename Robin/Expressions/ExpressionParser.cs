using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using Robin.Variables;
using System.Globalization;

namespace Robin.Expressions;

public static class ExpressionParser
{
    public static bool TryParse(this ref ExpressionLexer lexer, out IExpressionNode? expressionNode)
    {
        try
        {
            expressionNode = Parse(ref lexer);
            return true;
        }
        catch (Exception)
        {
            expressionNode = null;
            return false;
        }
    }

    public static IExpressionNode? Parse(this ref ExpressionLexer lexer)
    {
        if (!lexer.TryGetNextToken(out ExpressionToken? currentToken))
            return null;

        IExpressionNode result = ParseExpression(ref lexer, currentToken.Value);

        // Vérifier qu'il n'y a plus de tokens
        if (lexer.TryPeekNextToken(out ExpressionToken? extraToken, out _))
            throw new Exception($"Tokens inattendus après la fin: {extraToken.Value.Type}");

        return result;
    }

    private static IExpressionNode ParseExpression(ref ExpressionLexer lexer, ExpressionToken currentToken)
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
            //string value = lexer.GetValue(currentToken);
            return new LiteralExpressionNode(currentToken);
        }

        // Number
        if (currentToken.Type == ExpressionType.Number)
        {
            //string value = lexer.GetValue(currentToken);
            //int number = int.Parse(value, CultureInfo.InvariantCulture);
            return new NumberExpressionNode(currentToken);
        }

        // Identifier (variable ou fonction)
        if (currentToken.Type == ExpressionType.Identifier)
        {
            //Range range = currentToken;
            
            // Vérifier si c'est un appel de fonction (PEEK sans consommer)
            if (lexer.TryPeekNextToken(out ExpressionToken? nextToken, out int endPosition) &&
                nextToken.Value.Type == ExpressionType.LeftParenthesis)
            {
                // C'est une fonction, on consomme la parenthèse ouvrante
                lexer.AdvanceTo(endPosition);

                List<IExpressionNode> arguments = [];

                // Vérifier s'il y a des arguments
                if (lexer.TryPeekNextToken(out ExpressionToken? peekToken, out int peekEndPosition))
                {
                    if (peekToken.Value.Type == ExpressionType.RightParenthesis)
                    {
                        // Fonction sans arguments: func()
                        lexer.AdvanceTo(peekEndPosition);
                    }
                    else
                    {
                        // Il y a au moins un argument
                        // Consommer le premier token pour commencer l'expression
                        if (!lexer.TryGetNextToken(out ExpressionToken? firstArgToken))
                            throw new Exception("Argument attendu après '('");

                        // Parser le premier argument (expression complète)
                        arguments.Add(ParseExpression(ref lexer, firstArgToken.Value));

                        // Parser les arguments suivants
                        while (true)
                        {
                            if (!lexer.TryPeekNextToken(out ExpressionToken? sepToken, out int sepEndPosition))
                                throw new Exception("')' attendu à la fin de la liste d'arguments");

                            if (sepToken.Value.Type == ExpressionType.RightParenthesis)
                            {
                                // Fin de la liste d'arguments
                                lexer.AdvanceTo(sepEndPosition);
                                break;
                            }

                            // if (sepToken.Value.Type != ExpressionType.Operator ||
                            //     lexer.GetValue(sepToken.Value) != ",")
                            //     throw new Exception("',' ou ')' attendu dans la liste d'arguments");

                            // // Consommer la virgule
                            // lexer.AdvanceTo(sepEndPosition);

                            // Consommer le token suivant pour l'argument
                            if (!lexer.TryGetNextToken(out ExpressionToken? nextArgToken))
                                throw new Exception("Argument attendu après ','");

                            // Parser l'argument suivant (expression complète)
                            arguments.Add(ParseExpression(ref lexer, nextArgToken.Value));
                        }
                    }
                }
                else
                {
                    throw new Exception("')' attendu");
                }

                return new FunctionCallNode(currentToken, [.. arguments]);
            }

            // Sinon, c'est une variable
            VariablePath chainPath = lexer.ParseVariable(currentToken);
            return new IdentifierExpressionNode(chainPath);
        }

        throw new Exception($"Token inattendu: {currentToken.Type}");
    }
}