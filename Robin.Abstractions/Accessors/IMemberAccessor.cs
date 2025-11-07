using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IMemberAccessor
{
    bool TryGetMember(string name, [NotNull] out Delegate value);
}

public interface IMemberAccessor<T> : IMemberAccessor { }