using Robin.Contracts;
using Robin.Expressions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Nodes;


[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct Token(TokenType type, int start, int length)
{
    public TokenType Type => type;
    public int Start => start;
    public int Length => length;

    public ReadOnlySpan<char> GetValue(ReadOnlySpan<char> source)
    {
        return source.Slice(Start, Length);
    }

    public override string ToString()
    {
        return $"{Type} [{Start}..{Start + Length})";
    }

    [ExcludeFromCodeCoverage]
    private string GetDebuggerDisplay()
    {
        return $"{Type} [{Start}..{Start + Length})";
    }

    public static implicit operator Extract(Token token)
    {
        return new(token.Start, (token.Start + token.Length));
    }
    public static implicit operator Range(Token token)
    {
        return token.Start..(token.Start + token.Length);
    }
}
