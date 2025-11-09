using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;

internal sealed class ListIndexAccessor : IIndexDelegateAccessor, IIndexAccessor<IList>
{
    public readonly static ListIndexAccessor Instance = new();
    private ListIndexAccessor() { }
    public bool TryGetIndex(int index, [NotNull] out Delegate value)
    {
        value = (object? source) =>
        {
            if (source is IList list && index >= 0 && index < list.Count)
            {
                return list[index];
            }
            return null;
        };
        return true;
    }

    public bool TryGetIndex(IList obj, int index, out object? value)
    {
        if (obj is not null && index >= 0 && index < obj.Count)
        {
            value = obj[index];
            return true;
        }
        value = null;
        return false;
    }
}
