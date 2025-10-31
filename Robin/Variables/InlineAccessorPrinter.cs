using System.Text;

namespace Robin.Variables;

internal class InlineAccessorPrinter : IAccessorVisitor<StringBuilder, StringBuilder>
{
    public static readonly InlineAccessorPrinter Instance = new();
    public StringBuilder VisitIndex(IndexAccessor accessor, StringBuilder args)
    {
        return args.Append('[').Append(accessor.Index).Append(']');
    }

    public StringBuilder VisitKey(KeyAccessor accessor, StringBuilder args)
    {
        return args.Append('[').Append(accessor.Key.ToString()).Append(']');
    }

    public StringBuilder VisitMember(MemberAccessor accessor, StringBuilder args)
    {
        if (args.Length > 0)
            args.Append('.');
        return args.Append(accessor.MemberName);
    }

    public StringBuilder VisitParent(ParentAccessor accessor, StringBuilder args)
    {
        return args.Append('~');
    }

    public StringBuilder VisitThis(ThisAccessor accessor, StringBuilder args)
    {
        return args.Append('.');
    }
}
