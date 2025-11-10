namespace RobinMustache.Abstractions.Accessors;

public interface IMemberDelegateAccessor
{
    bool TryGetMember(string name, out Delegate value);
}
public interface IMemberDelegateAccessor<T> : IMemberDelegateAccessor { }

