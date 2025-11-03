using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IMemberAccessor<T> : IMemberAccessor
{
    bool IMemberAccessor.TryGetMember(object? source, string name, [MaybeNullWhen(false)] out object? value)
    {
        if (source is T typed)
            return TryGetMember(typed, name, out value);
        value = null;
        return false;
    }
    bool TryGetMember(T? source, string name, [MaybeNullWhen(false)] out object? value);
}
