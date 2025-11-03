using System.Collections;

namespace Robin.Abstractions.Accessors;

internal sealed class ListIndexAccessor : IIndexAccessor
{
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
