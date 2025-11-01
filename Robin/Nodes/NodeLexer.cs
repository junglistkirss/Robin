using System.Diagnostics.CodeAnalysis;

namespace Robin.Nodes;

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
    public readonly bool TryPeekNextToken([NotNullWhen(true)] out Token? token, out int endPosition)
    {
        int savedPosition = _position;
        bool result = TryGetNextTokenInternal(out token, ref savedPosition);
        endPosition = savedPosition;
        return result;
    }
    public bool TryGetNextToken([NotNullWhen(true)] out Token? token)
    {
        return TryGetNextTokenInternal(out token, ref _position);

    }
    private readonly bool TryGetNextTokenInternal([NotNullWhen(true)] out Token? token, ref int pos)
    {
        if (pos >= _source.Length)
        {
            token = null;
            return false;
        }

        // Look for opening delimiter
        int delimiterPos = IndexOf(_source[pos..], OpenDelimiter);

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
            pos += delimiterPos;
            token = new Token(TokenType.Text, start, delimiterPos);
            return true;
        }

        // Parse the mustache tag
        return TryParseMustacheTag(out token, ref pos);
    }

    private readonly bool TryParseMustacheTag([NotNullWhen(true)] out Token? token, ref int pos)
    {
        int tagStart = pos;
        pos += OpenDelimiter.Length;

        if (pos >= _source.Length)
        {
            token = null;
            return false;
        }

        // Check for triple braces {{{var}}}
        bool isTripleBrace = _source[pos] == '{';
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
            case '<': // Partial {{> partial}}
                tokenType = TokenType.PartialDefine;
                pos++;
                contentStart = pos;
                break;
            case '>': // Partial {{> partial}}
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
        int closePos = IndexOf(_source[pos..], closingDelim);

        if (closePos == -1)
        {
            // Malformed tag - treat as text
            pos = tagStart;
            token = new Token(TokenType.Text, tagStart, 2);
            pos += 2;
            return true;
        }

        // Skip whitespace around content
        int contentEnd = pos + closePos;
        while (contentStart < contentEnd && char.IsWhiteSpace(_source[contentStart]))
        {
            contentStart++;
        }
        while (contentEnd > contentStart && char.IsWhiteSpace(_source[contentEnd - 1]))
        {
            contentEnd--;
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
    // Convenience method to tokenize entire input

}
