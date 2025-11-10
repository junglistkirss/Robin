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
    public readonly bool TryPeekNextToken(out Token? token, out int endPosition)
    {
        int savedPosition = _position;
        bool result = TryGetNextTokenInternal(out token, ref savedPosition);
        endPosition = savedPosition;
        return result;
    }
    public bool TryGetNextToken(out Token? token)
    {
        return TryGetNextTokenInternal(out token, ref _position);

    }
    private readonly bool TryGetNextTokenInternal(out Token? token, ref int pos)
    {
        if (pos >= _source.Length)
        {
            token = null;
            return false;
        }

        // Détection du retour à la ligne avant tout
        if (_source[pos] is '\r' or '\n')
        {
            int start = pos;

            // Gérer \r\n comme un seul token
            if (_source[pos] is '\r' && pos + 1 < _source.Length && _source[pos + 1] is '\n')
                pos += 2;
            else
                pos++;

            token = new Token(TokenType.LineBreak, start, pos - start);
            return true;
        }

        // Look for opening delimiter
        int delimiterPos = IndexOf(_source.Slice(pos), OpenDelimiter);

        // If no delimiter found, rest is text
        if (delimiterPos == -1)
        {
            int start = pos;
            int length = _source.Length - pos;
            pos = _source.Length;
            token = new Token(TokenType.Text, start, length);
            return true;
        }

        // If there's text before the delimiter, return it first
        if (delimiterPos > 0)
        {
            int start = pos;
            //int lastIndex = pos + delimiterPos; // dernier caractère du texte avant le délimiteur
            if (TrimLineBreaksBounds(ref pos, start, delimiterPos, out token))
                return true;

            // Pas de line break : tout le bloc est du texte
            token = new Token(TokenType.Text, start, delimiterPos);
            pos += delimiterPos;
            return true;
        }

        // Parse the mustache tag
        return TryParseMustacheTag(out token, ref pos);
    }

    private readonly bool TrimLineBreaksBounds(ref int pos, int start, int lastIndex, out Token? token)
    {
        // Position de fin effective
        int end = start + lastIndex;

        // Vérifie si le texte se termine par un saut de ligne
        if (end > start && _source[end - 1] is '\n' or '\r')
        {
            int i = end;
            // Remonte tant qu'on trouve des \r ou \n
            while (i > start && _source[i - 1] is '\n' or '\r')
                i--;

            int length = i - start;
            token = new Token(TokenType.Text, start, length);
            pos += length;
            return true;
        }

        token = null;
        return false;
    }

    private readonly bool TryParseMustacheTag(out Token? token, ref int pos)
    {
        int tagStart = pos;
        pos += OpenDelimiter.Length;

        if (pos >= _source.Length)
        {
            token = null;
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
            token = new Token(TokenType.Text, tagStart, 2);
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

        token = new Token(tokenType, contentStart, contentEnd - contentStart);
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
