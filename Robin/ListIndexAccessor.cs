using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

internal sealed class ListIndexAccessor : IIndexAccessor
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
}
