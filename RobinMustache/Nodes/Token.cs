using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RobinMustache.Nodes;

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
}
