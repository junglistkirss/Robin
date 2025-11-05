using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IIndexAccessor
{
    bool TryGetIndex(int index, [NotNull] out Delegate value);
}
