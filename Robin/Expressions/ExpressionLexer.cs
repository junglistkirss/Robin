using Robin.Contracts;
using Robin.Nodes;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Expressions;

public ref struct ExpressionLexer
{
    private readonly NodeLexer _source;
    private readonly Extract _range;
    private int _position;

    public ExpressionLexer(ref NodeLexer lexer, Extract range)
    {
        _source = lexer;
        _range = range;
        _position = _range.Start;
    }

    private readonly void SkipWhitespace(ref int pos)
    {
        while (pos < _range.End && char.IsWhiteSpace(_source[pos]))
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

    public readonly bool TryPeekNextToken([NotNullWhen(true)] out ExpressionToken? token, out int endPosition)
    {
        int peekPosition = _position;
        bool result = TryGetNextTokenInternal(out token, ref peekPosition);
        endPosition = peekPosition;
        return result;
    }

    private readonly bool TryGetNextTokenInternal([NotNullWhen(true)] out ExpressionToken? token, ref int pos)
    {
        SkipWhitespace(ref pos);
        if (pos >= _range.End)
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
            while (pos < _range.End && _source[pos] != quote)
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
            while (pos < _range.End && (char.IsLetterOrDigit(_source[pos]) || _source[pos] is '_' or '.' or '[' or ']' or '.'))
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

    //public readonly ReadOnlySpan<char> GetValue(Range token)
    //{
    //    return _source[token];
    //}
    public readonly ReadOnlySpan<char> this[Extract extract] => _source[extract];
    public readonly char this[int token] => _source[token];
}
