using System.Collections.Immutable;
using System.Text;

namespace Robin.Contracts.Variables;

public record struct VariablePath(ImmutableArray<IVariableSegment> segments)
{
    public static implicit operator string(VariablePath value) => value.ToString();
    public ImmutableArray<IVariableSegment> Segments { get; } = segments;
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in segments)
        {
            sb.Append(item switch
            {
                IndexSegment segment => $"[{segment.Index}]",
                ThisSegment segment => ".",
                MemberSegment segment => $"{sb.Length > 0:'.':''}{segment.MemberName}",
                _ => ""
            });
        }
        return sb.ToString();
    }

}
