using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IMemberDelegateAccessor
{
    bool TryGetMember(string name, [NotNull] out Delegate value);
}
public interface IMemberDelegateAccessor<T> : IMemberDelegateAccessor { }

