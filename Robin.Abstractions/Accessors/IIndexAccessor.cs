namespace Robin.Abstractions.Accessors;

public interface IIndexAccessor
{
    bool TryGetIndex(object? obj, int index, out object? value);
}
public interface IIndexAccessor<T> : IIndexAccessor
{  

    bool TryGetIndex(T obj, int index, out object? value);
}
