using System.Diagnostics.CodeAnalysis;

namespace Robin;

public ref struct Lexer
{
    private ReadOnlySpan<char> _source;
    private int _position;
    private const string OpenDelimiter = "{{";
    private const string CloseDelimiter = "}}";

    public Lexer(ReadOnlySpan<char> source)
    {
        _source = source;
        _position = 0;
    }

    public bool TryGetNextToken([NotNullWhen(true)] out Token? token)
    {
        if (_position >= _source.Length)
        {
            token = null;
            return false;
        }

        // Look for opening delimiter
        int delimiterPos = IndexOf(_source.Slice(_position), OpenDelimiter);

        // If no delimiter found, rest is text
        if (delimiterPos == -1)
        {
            int start = _position;
            int length = _source.Length - _position;
            _position = _source.Length;
            token = new Token(TokenType.Text, start, length);
            return true;
        }

        // If there's text before the delimiter, return it first
        if (delimiterPos > 0)
        {
            int start = _position;
            _position += delimiterPos;
            token = new Token(TokenType.Text, start, delimiterPos);
            return true;
        }

        // Parse the mustache tag
        return TryParseMustacheTag(out token);
    }

    private bool TryParseMustacheTag([NotNullWhen(true)] out Token? token)
    {
        int tagStart = _position;
        _position += OpenDelimiter.Length;

        if (_position >= _source.Length)
        {
            token = null;
            return false;
        }

        // Check for triple braces {{{var}}}
        bool isTripleBrace = _source[_position] == '{';
        if (isTripleBrace)
        {
            _position++;
        }

        // Determine tag type by first character
        char firstChar = _position < _source.Length ? _source[_position] : '\0';
        TokenType tokenType = TokenType.Variable;
        int contentStart = _position;

        switch (firstChar)
        {
            case '&': // Unescaped variable {{&var}}
                tokenType = TokenType.UnescapedVariable;
                _position++;
                contentStart = _position;
                break;
            case '#': // Section open {{#section}}
                tokenType = TokenType.SectionOpen;
                _position++;
                contentStart = _position;
                break;
            case '/': // Section close {{/section}}
                tokenType = TokenType.SectionClose;
                _position++;
                contentStart = _position;
                break;
            case '^': // Inverted section {{^section}}
                tokenType = TokenType.InvertedSection;
                _position++;
                contentStart = _position;
                break;
            case '!': // Comment {{! comment}}
                tokenType = TokenType.Comment;
                _position++;
                contentStart = _position;
                break;
            case '>': // Partial {{> partial}}
                tokenType = TokenType.Partial;
                _position++;
                contentStart = _position;
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
        int closePos = IndexOf(_source.Slice(_position), closingDelim);

        if (closePos == -1)
        {
            // Malformed tag - treat as text
            _position = tagStart;
            token = new Token(TokenType.Text, tagStart, 2);
            _position += 2;
            return true;
        }

        // Skip whitespace around content
        int contentEnd = _position + closePos;
        while (contentStart < contentEnd && char.IsWhiteSpace(_source[contentStart]))
        {
            contentStart++;
        }
        while (contentEnd > contentStart && char.IsWhiteSpace(_source[contentEnd - 1]))
        {
            contentEnd--;
        }

        _position += closePos + closingDelim.Length;

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
    public readonly ReadOnlyMemory<char> GetValue(Token token)
    {
        ReadOnlySpan<char> x = _source.Slice(token.Start, token.Length);
        return  new ReadOnlyMemory<char>(x.ToArray());
    }
    // Convenience method to tokenize entire input

}
