using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IIterator
{
    void Iterate(Action<object?> action);
}


public interface IIndexAccessor
{
    bool TryGetIndex(int index, [NotNull] out Delegate value);
}

public interface IIndexAccessor<T> : IIndexAccessor { }