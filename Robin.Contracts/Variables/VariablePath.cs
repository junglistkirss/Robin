using System.Collections.Immutable;
using System.Text;

namespace Robin.Contracts.Variables;

public sealed  class VariablePath(ImmutableArray<IVariableSegment> segments)
{
    public static implicit operator string(VariablePath value) => value.ToString();
    public ImmutableArray<IVariableSegment> Segments { get; } = segments;
    public override string ToString()
    {
        return Segments.Aggregate(
            new StringBuilder(),
            (sb, segment) =>
            {
                return segment.Accept(InlineSegmentPrinter.Instance, sb);
            },
            sb => sb.ToString()
        );
    }

}
