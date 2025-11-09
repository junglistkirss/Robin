using System.Collections.Immutable;
using System.Text;

namespace RobinMustache.Abstractions.Variables;

public readonly record struct VariablePath(ImmutableArray<IVariableSegment> Segments)
{
    public static implicit operator string(VariablePath value) => value.ToString();
    public ImmutableArray<IVariableSegment> Segments { get; } = Segments;

    public int Length => Segments.Length;
    public IVariableSegment this[int index] => Segments[index];
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (var item in Segments)
        {
            sb.Append(item switch
            {
                IndexSegment segment => $"[{segment.Index}]",
                ThisSegment => ".",
                MemberSegment segment => $"{(sb.Length > 0 ? "." : "")}{segment.MemberName}",
                _ => ""
            });
        }
        return sb.ToString();
    }

}
