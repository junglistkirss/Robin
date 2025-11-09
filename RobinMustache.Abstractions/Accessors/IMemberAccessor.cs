namespace RobinMustache.Abstractions.Accessors;

public interface IMemberAccessor
{
    bool TryGetMember(object? obj, string name, out object? value);
}
public interface IMemberAccessor<T> : IMemberAccessor
{
    bool TryGetMember(T obj, string name, out object? value);
}
