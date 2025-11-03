using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;

public interface IIndexAccessor
{
    bool TryGetIndex(object? source, int index, [MaybeNullWhen(false)] out object? value);
}
