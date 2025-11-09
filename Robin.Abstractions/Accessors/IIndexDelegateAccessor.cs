using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;


public interface IIndexDelegateAccessor
{
    bool TryGetIndex(int index, [NotNull] out Delegate value);
}

public interface IIndexDelegateAccessor<T> : IIndexDelegateAccessor { }
