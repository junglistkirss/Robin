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
    private readonly void SkipWhitespace(ref int pos)
    {
        while (pos < _source.Length && char.IsWhiteSpace(_source[pos]))
        {
            pos++;
        }
    }
    public void AdvanceTo(int position)
    {
        _position = position;
    }

    public bool TryGetNextToken([NotNullWhen(true)] out ExpressionToken? token)
    {
        return TryGetNextTokenInternal(out token, ref _position);
    }

    public bool TryPeekNextToken([NotNullWhen(true)] out ExpressionToken? token, out int endPosition)
    {
        int peekPosition = _position;
        bool result = TryGetNextTokenInternal(out token, ref peekPosition);
        endPosition = peekPosition;
        return result;
    }

    private bool TryGetNextTokenInternal([NotNullWhen(true)] out ExpressionToken? token, ref int pos)
    {
        SkipWhitespace(ref pos);
        if (pos >= _source.Length)
        {
            token = null;
            return false;
        }

        char current = _source[pos];

        if (current == '(')
        {
            token = new ExpressionToken(ExpressionType.LeftParenthesis, pos, 1);
            pos++;
            return true;
        }
        else if (current == ')')
        {
            token = new ExpressionToken(ExpressionType.RightParenthesis, pos, 1);
            pos++;
            return true;
        }
        else if (current == '+' || current == '-' || current == '/' || current == '*' || current == '%' || current == '^')
        {
            token = new ExpressionToken(ExpressionType.Operator, pos, 1);
            pos++;
            return true;
        }
        else if (current == '>' || current == '<')
        {
            int operatorStart = pos;
            pos++;
            if (_source[pos] == '=')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                pos++;
            }
            else
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 1);
            return true;
        }
        else if (current == '=')
        {
            int operatorStart = pos;
            pos++;
            if (_source[pos] == '=')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                pos++;
            }
            else
                throw new InvalidOperationException($"Invalid assign '=', assignation are not supported");
            return true;
        }
        else if (current == '&')
        {
            int operatorStart = pos;
            pos++;
            if (_source[pos] == '&')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                pos++;
            }
            else
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 1);
            return true;
        }
        else if (current == '|')
        {
            int operatorStart = pos;
            pos++;
            if (_source[pos] == '|')
            {
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 2);
                pos++;
            }
            else
                token = new ExpressionToken(ExpressionType.Operator, operatorStart, 1);
            return true;
        }
        else
        {
            int start = pos;
            if (_source[pos] == '"')
            {
                pos++;
                start++;
                while (pos < _source.Length && _source[pos] != '"')
                {
                    pos++;
                }
                token = new ExpressionToken(ExpressionType.Literal, start, pos - start);
                pos++;
            }
            else if (_source[pos] == '\'')
            {
                pos++;
                start++;
                while (pos < _source.Length && _source[pos] != '\'')
                {
                    pos++;
                }
                token = new ExpressionToken(ExpressionType.Literal, start, pos - start);
                pos++;
            }
            else
            {
                bool isOnlyDigits = char.IsDigit(_source[pos]) || _source[pos] == '.';
                while (pos < _source.Length && (char.IsLetterOrDigit(_source[pos]) || _source[pos] == '_' || _source[pos] == '.' || _source[pos] == '[' || _source[pos] == ']' || _source[pos] == '.'))
                {
                    isOnlyDigits = isOnlyDigits && (char.IsDigit(_source[pos]) || _source[pos] == '.');
                    pos++;
                }
                if (isOnlyDigits)
                {
                    token = new ExpressionToken(ExpressionType.Number, start, pos - start);
                    return true;
                }
                token = new ExpressionToken(ExpressionType.Identifier, start, pos - start);
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
