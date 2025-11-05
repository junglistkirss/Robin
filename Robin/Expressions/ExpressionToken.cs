using Robin.Contracts;
using Robin.Nodes;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Expressions;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct ExpressionToken(ExpressionType type, int start, int length)
{
    public ExpressionType Type => type;
    public int Start => start;
    public int Length => length;

    public ReadOnlySpan<char> GetValue(ReadOnlySpan<char> source)
    {
        return source.Slice(Start, Length);
    }

    [ExcludeFromCodeCoverage]
    private string GetDebuggerDisplay()
    {
        return $"{Type} [{Start}..{Start + Length})";
    }
    public static implicit operator Range (ExpressionToken token)
    {
        return token.Start..(token.Start + token.Length);
    }
    public static implicit operator Extract(ExpressionToken token)
    {
        return new(token.Start, (token.Start + token.Length));
    }
}