using System.Collections.Immutable;
using System.Text;

namespace Robin.Contracts.Variables;

public readonly struct VariablePath(ImmutableArray<IAccessor> segments)
{
    public static implicit operator string(VariablePath value) => value.ToString();
    public ImmutableArray<IAccessor> Segments { get; } = segments;
    public override string ToString()
    {
        return Segments.Aggregate(
            new StringBuilder(),
            (sb, segment) =>
            {
                return segment.Accept(InlineAccessorPrinter.Instance, sb);
            },
            sb => sb.ToString()
        );
    }

}
