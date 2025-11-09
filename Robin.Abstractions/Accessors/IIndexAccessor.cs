namespace Robin.Abstractions.Accessors;

public interface IIndexAccessor
{
    bool TryGetIndex(object? obj, int index, out object? value);
}
public interface IIndexAccessor<T> : IIndexAccessor
{
    bool IIndexAccessor.TryGetIndex(object? obj, int index, out object? value)
    {
        if (obj is T typed)
        {
            return TryGetIndex(typed, index, out value);
        }
        value = null;
        return false;
    }

    bool TryGetIndex(T obj, int index, out object? value);
}