using System.Text;

namespace Robin.Contracts.Variables;

internal class InlineSegmentPrinter : IVariableSegmentVisitor<StringBuilder, StringBuilder>
{
    public static readonly InlineSegmentPrinter Instance = new();
    public StringBuilder VisitIndex(IndexSegment segment, StringBuilder args)
    {
        return args.Append('[').Append(segment.Index).Append(']');
    }

    public StringBuilder VisitMember(MemberISegment segment, StringBuilder args)
    {
        if (args.Length > 0)
            args.Append('.');
        return args.Append(segment.MemberName);
    }

    public StringBuilder VisitThis(ThisSegment segment, StringBuilder args)
    {
        return args.Append('.');
    }
}
