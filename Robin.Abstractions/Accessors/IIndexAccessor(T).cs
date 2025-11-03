using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IIndexAccessor<T> : IIndexAccessor
{
    bool IIndexAccessor.TryGetIndex(object? source, int index, [MaybeNullWhen(false)] out object? value)
    {
        if (source is T typed)
            return TryGetIndex(typed, index, out value);
        value = null;
        return false;
    }
    bool TryGetIndex(T? source, int index, [MaybeNullWhen(false)] out object? value);
}
