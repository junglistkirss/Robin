namespace Robin.Abstractions.Accessors;

public abstract class BaseIndexAccessor<T> : IIndexAccessor<T>
{
    public abstract bool TryGetIndex(T obj, int index, out object? value);

    bool IIndexAccessor.TryGetIndex(object? obj, int index, out object? value)
    {
        if (obj is T typed)
        {
            return TryGetIndex(typed, index, out value);
        }
        value = null;
        return false;
    }
}