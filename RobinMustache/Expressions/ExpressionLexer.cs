namespace RobinMustache.Expressions;

public ref struct ExpressionLexer
{
    private readonly ReadOnlySpan<char> _source;
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

    public bool TryGetNextToken(out ExpressionToken? token)
    {
        return TryGetNextTokenInternal(out token, ref _position);
    }

    public readonly bool TryPeekNextToken(out ExpressionToken? token, out int endPosition)
    {
        int peekPosition = _position;
        bool result = TryGetNextTokenInternal(out token, ref peekPosition);
        endPosition = peekPosition;
        return result;
    }

    private readonly bool TryGetNextTokenInternal(out ExpressionToken? token, ref int pos)
    {
        SkipWhitespace(ref pos);
        if (pos >= _source.Length)
        {
            token = null;
            return false;
        }

        char current = _source[pos];

        if (current is '(')
        {
            token = new ExpressionToken(ExpressionType.LeftParenthesis, pos, 1);
            pos++;
            return true;
        }
        else if (current is ')')
        {
            token = new ExpressionToken(ExpressionType.RightParenthesis, pos, 1);
            pos++;
            return true;
        }

        int start = pos;
        if (_source[pos] is '"' or '\'' or '`')
        {
            char quote = _source[pos];
            pos++;
            start++;
            while (pos < _source.Length && _source[pos] != quote)
            {
                pos++;
            }
            token = new ExpressionToken(ExpressionType.Literal, start, pos - start);
            pos++;
            return true;
        }
        else if (char.IsLetterOrDigit(_source[pos]) || _source[pos] is '_' or '.' or '[' or ']' or '.')
        {
            bool isOnlyDigits = char.IsDigit(_source[pos]);
            while (pos < _source.Length && (char.IsLetterOrDigit(_source[pos]) || _source[pos] is '_' or '.' or '[' or ']' or '.'))
            {
                isOnlyDigits = isOnlyDigits && (char.IsDigit(_source[pos]));
                pos++;
            }
            if (isOnlyDigits)
            {
                token = new ExpressionToken(ExpressionType.Number, start, pos - start);
                return true;
            }
            token = new ExpressionToken(ExpressionType.Identifier, start, pos - start);
            return true;
        }
        throw new InvalidTokenException($"Invalid expression token found : \"{_source[pos]}\"");
    }

    public readonly string GetValue(ExpressionToken token)
    {
        ReadOnlySpan<char> x = _source.Slice(token.Start, token.Length);
        return x.ToString();
    }
}
