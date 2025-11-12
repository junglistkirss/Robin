using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RobinMustache.Nodes;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct Token(TokenType type, int start, int length, bool isAtlineStart, bool isAtLineEnd)
{
    public static readonly Token EOF = new(TokenType.EOF, 0, 0, false, false);

    public TokenType Type => type;
    public int Start => start;
    public int Length => length;

    public bool IsAtLineStart { get; } = isAtlineStart;
    public bool IsAtLineEnd { get; } = isAtLineEnd;

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
}
