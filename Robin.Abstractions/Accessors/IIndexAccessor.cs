using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IIndexAccessor
{
    bool TryGetIndex(object? source, int index, [MaybeNullWhen(false)] out object? value);
}
