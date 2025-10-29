using System.Diagnostics.CodeAnalysis;

namespace Robin;

public ref struct ExpressionLexer
{
    private ReadOnlySpan<char> _source;
    private int _position;

    public ExpressionLexer(ReadOnlySpan<char> source)
    {
        _source = source;
        _position = 0;
    }
    private void SkipWhitespace(ReadOnlySpan<char> span)
    {
        while (_position < span.Length && char.IsWhiteSpace(span[_position]))
        {
            _position++;
        }
    }
    public bool TryGetNextToken([NotNullWhen(true)] out ExpressionToken? token)
    {
        SkipWhitespace(_source);
        if (_position >= _source.Length)
        {
            token = null;
            return false;
        }

        char current = _source[_position];

        if (current == '(')
        {
            token = new ExpressionToken(ExpressionType.LeftParenthesis, _position, 1);
            _position++;
            return true;
        }
        else if (current == ')')
        {
            token = new ExpressionToken(ExpressionType.RightParenthesis, _position, 1);
            _position++;
            return true;
        }
        else if (current == '+' || current == '-' || current == '/' || current == '*' || current == '%' || current == '^')
        {
            token = new ExpressionToken(ExpressionType.Operator, _position, 1);
            _position++;
            return true;
        }
        else if (current == '>' || current == '<')
        {
            int operatorStart = _position;
            _position++;
            if (_source[_position] == '=')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                _position++;
            }
            else
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 1);
            return true;
        }
        else if (current == '=')
        {
            int operatorStart = _position;
            _position++;
            if (_source[_position] == '=')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                _position++;
            }
            else
                throw new InvalidOperationException($"Invalid assign '=', assignation are not supported");
            return true;
        }
        else if (current == '&')
        {
            int operatorStart = _position;
            _position++;
            if (_source[_position] == '&')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                _position++;
            }
            else
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 1);
            return true;
        }
        else if (current == '|')
        {
            int operatorStart = _position;
            _position++;
            if (_source[_position] == '|')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                _position++;
            }
            else
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 1);
            return true;
        }
        else
        {
            int start = _position;
            if (_source[_position] == '"')
            {
                    _position++;
                start++;
                while (_position < _source.Length && _source[_position] != '"')
                {
                    _position++;
                }
                token = new ExpressionToken(ExpressionType.Constant, start, _position - start);
                _position++;
            }
            else if (_source[_position] == '\'')
            {
                    _position++;
                start++;
                while (_position < _source.Length && _source[_position] != '\'')
                {
                    _position++;
                }
                token = new ExpressionToken(ExpressionType.Constant, start, _position - start);
                _position++;
            }
            else
            {
                bool isOnlyDigits = char.IsDigit(_source[_position]) || _source[_position] == '.';
                while (_position < _source.Length &&
                       (char.IsLetterOrDigit(_source[_position]) || _source[_position] == '_' || _source[_position] == '.' || _source[_position] == '[' || _source[_position] == ']'))
                {
                    isOnlyDigits = isOnlyDigits && char.IsDigit(_source[_position]) || _source[_position] == '.';
                    _position++;
                }
                if (isOnlyDigits)
                {
                    token = new ExpressionToken(ExpressionType.Constant, start, _position - start);
                    return true;
                }
                token = new ExpressionToken(ExpressionType.Identifier, start, _position - start);
            }
            return true;
        }
    }



    public readonly string GetValue(ExpressionToken token)
    {
        ReadOnlySpan<char> x = _source.Slice(token.Start, token.Length);
        return x.ToString();
    }
    // Convenience method to tokenize entire input

}
