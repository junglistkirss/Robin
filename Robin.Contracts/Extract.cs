using System.Diagnostics.Contracts;

namespace Robin.Contracts;

public readonly record struct Extract(int Start, int End)
{
    public readonly static Extract Empty = new Extract(0, 0);
    public static Extract Full(int length) => new Extract(0, length);

    public int Length => End - Start;

    public static implicit operator Range(Extract extract)
    {
        return extract.Start..extract.End;
    }
    [Pure]
    public Extract Limit(int lim)
    {
        return new Extract(Start, Start + lim);
    }
    [Pure]
    public Extract Offset(int start)
    {
        return new Extract(Start + start, End);
    }
}
