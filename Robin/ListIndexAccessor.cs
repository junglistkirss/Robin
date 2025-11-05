using System.Collections;

namespace Robin.Abstractions.Accessors;

internal sealed class ListIndexAccessor : IIndexAccessor
{
    public readonly static ListIndexAccessor Instance = new();
    private ListIndexAccessor(){}
    bool IIndexAccessor.TryGetIndex(object? source, int index, out object? value)
    {

        if (source is IList list && index >= 0 && index < list.Count)
        {
            value = list[index];
            return true;
        }
        value = null;
        return false;
    }
}
