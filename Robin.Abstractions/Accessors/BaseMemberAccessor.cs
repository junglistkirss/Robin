namespace Robin.Abstractions.Accessors;

public abstract class BaseMemberAccessor<T> : IMemberAccessor<T>
{
    public abstract bool TryGetMember(T obj, string name, out object? value);
    bool IMemberAccessor.TryGetMember(object? obj, string name, out object? value)
    {
        if (obj is T typed)
        {
            return TryGetMember(typed, name, out value);
        }
        value = null;
        return false;
    }

}