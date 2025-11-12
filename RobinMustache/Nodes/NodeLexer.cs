namespace RobinMustache.Nodes;

public ref struct NodeLexer
{
    private readonly ReadOnlySpan<char> _source;
    private int _position;
    private const string OpenDelimiter = "{{";
    private const string CloseDelimiter = "}}";

    public NodeLexer(ReadOnlySpan<char> source)
    {
        _source = source;
        _position = 0;
    }
    public void AdvanceTo(int position)
    {
        _position = position;
    }
    public readonly bool TryPeekNextToken(out Token token, out int endPosition)
    {
        int savedPosition = _position;
        bool result = TryGetNextTokenInternal(out token, ref savedPosition);
        endPosition = savedPosition;
        return result;
    }
    public bool TryGetNextToken(out Token token)
    {
        return TryGetNextTokenInternal(out token, ref _position);

    }
    private readonly bool TryGetNextTokenInternal(out Token token, ref int pos)
    {
        if (pos >= _source.Length)
        {
            token = Token.EOF;
            return false;
        }
        bool isAtLineStart = pos == 0 || _source[pos - 1] is '\n' or '\r';
        // Détection du retour à la ligne avant tout
        if (_source[pos] is '\r' or '\n')
        {
            int start = pos;

            // Gérer \r\n comme un seul token
            if (_source[pos] is '\r' && pos + 1 < _source.Length && _source[pos + 1] is '\n')
            {
                pos += 2;
                token = new Token(TokenType.LineBreak, start, pos - start, isAtLineStart, false);
                return true;
            }
            else
            {
                pos++;
                token = new Token(TokenType.LineBreak, start, pos - start, isAtLineStart, false);
                return true;
            }
        }

        // Look for opening delimiter
        int delimiterPos = IndexOf(_source.Slice(pos), OpenDelimiter);

        // If no delimiter found, rest is text
        if (delimiterPos == -1)
        {
            int start = pos;
            int length = _source.Length - pos;
            pos = _source.Length;
            token = new Token(TokenType.Text, start, length, isAtLineStart, true);
            return true;
        }

        // If there's text before the delimiter, return it first
        if (delimiterPos > 0)
        {
            int start = pos;
            ReadOnlySpan<char> slice = _source.Slice(start, delimiterPos);
            int carriagePos = slice.IndexOf('\r');
            int linePos = slice.IndexOf('\n');
            if (carriagePos > 0 || linePos > 0)
            {
                if (carriagePos > 0 && carriagePos < linePos)
                    delimiterPos = carriagePos;
                else
                    delimiterPos = linePos;
            }

            pos += delimiterPos;
            bool isAtLineEnd = _source[pos + 1] is '\n' or '\r';
            if (isAtLineStart && slice.IsWhiteSpace())
            {
                token = new Token(TokenType.Whitepsaces, start, delimiterPos, isAtLineStart, isAtLineEnd);
                return true;
            }
            // Pas de line break : tout le bloc est du texte
            token = new Token(TokenType.Text, start, delimiterPos, isAtLineStart, isAtLineEnd);
            return true;
        }

        // Parse the mustache tag
        return TryParseMustacheTag(out token, ref pos, isAtLineStart);
    }

    private readonly bool TryParseMustacheTag(out Token token, ref int pos, bool isAtLineStart)
    {
        int tagStart = pos;
        pos += OpenDelimiter.Length;

        if (pos >= _source.Length)
        {
            token = Token.EOF;
            return false;
        }

        // Check for triple braces {{{var}}}
        bool isTripleBrace = _source[pos] is '{';
        if (isTripleBrace)
        {
            pos++;
        }

        // Determine tag type by first character
        char firstChar = pos < _source.Length ? _source[pos] : '\0';
        TokenType tokenType = TokenType.Variable;
        int contentStart = pos;

        switch (firstChar)
        {
            case '&': // Unescaped variable {{&var}}
                tokenType = TokenType.UnescapedVariable;
                pos++;
                contentStart = pos;
                break;
            case '#': // Section open {{#section}}
                tokenType = TokenType.SectionOpen;
                pos++;
                contentStart = pos;
                break;
            case '/': // Section close {{/section}}
                tokenType = TokenType.SectionClose;
                pos++;
                contentStart = pos;
                break;
            case '^': // Inverted section {{^section}}
                tokenType = TokenType.InvertedSection;
                pos++;
                contentStart = pos;
                break;
            case '!': // Comment {{! comment}}
                tokenType = TokenType.Comment;
                pos++;
                contentStart = pos;
                break;
            case '<': // Partial define {{< partial}}
                tokenType = TokenType.PartialDefine;
                pos++;
                contentStart = pos;
                break;
            case '>': // Partial call {{> partial}}
                tokenType = TokenType.PartialCall;
                pos++;
                contentStart = pos;
                break;
            default:
                if (isTripleBrace)
                {
                    tokenType = TokenType.UnescapedVariable;
                }
                break;
        }

        // Find closing delimiter
        string closingDelim = isTripleBrace ? "}}}" : CloseDelimiter;
        int closePos = IndexOf(_source.Slice(pos), closingDelim);

        if (closePos == -1)
        {
            // Malformed tag - treat as text
            pos = tagStart;
            bool isAtLineEnd = pos + 1 >= _source.Length || _source[pos + 1] is '\n' or '\r';

            token = new Token(TokenType.Text, tagStart, 2, isAtLineStart, isAtLineEnd);
            pos += 2;
            return true;
        }

        int contentEnd = pos + closePos;
        if (tokenType == TokenType.Comment)
        {
            while (contentStart < contentEnd && char.IsWhiteSpace(_source[contentStart]) && _source[contentStart] is not ('\r' or '\n'))
                contentStart++;
            while (contentEnd > contentStart && char.IsWhiteSpace(_source[contentEnd - 1]) && _source[contentStart] is not ('\r' or '\n'))
                contentEnd--;
        }
        else
        {
            while (contentStart < contentEnd && char.IsWhiteSpace(_source[contentStart]))
                contentStart++;

            while (contentEnd > contentStart && char.IsWhiteSpace(_source[contentEnd - 1]))
                contentEnd--;
            // Skip only spaces and tabs, but keep line breaks

        }
        pos += closePos + closingDelim.Length;
        token = new Token(tokenType, contentStart, contentEnd - contentStart, isAtLineStart, pos + 1 >= _source.Length || _source[pos + 1] is '\n' or '\r');
        return true;
    }

    private static int IndexOf(ReadOnlySpan<char> span, string value)
    {
        if (value.Length == 0) return 0;
        if (span.Length < value.Length) return -1;

        for (int i = 0; i <= span.Length - value.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < value.Length; j++)
            {
                if (span[i + j] != value[j])
                {
                    found = false;
                    break;
                }
            }
            if (found) return i;
        }
        return -1;
    }
    public readonly string GetValue(Token token)
    {
        ReadOnlySpan<char> x = _source.Slice(token.Start, token.Length);
        return x.ToString();
    }
}
