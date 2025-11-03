using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IMemberAccessor
{
    bool TryGetMember(object? source, string name, [MaybeNullWhen(false)] out object? value);
}
