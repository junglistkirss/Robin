using Robin.Abstractions.Accessors;
using Robin.Abstractions.Extensions;
using Robin.Internals;
using static Robin.Abstractions.Extensions.AccessorBuilder;

namespace Robin;
public sealed class StaticAccessorBuilder
{
    private readonly Dictionary<Type, IMemberAccessor> memberAccessors = [];
    private readonly Dictionary<Type, IIndexAccessor> indexAccessors = [];

    public StaticAccessorBuilder CreateIndexObjectAccessor<T>(TryGetIndexObjectAccessor<T> tryGet)
    {
        if (!indexAccessors.ContainsKey(typeof(T)))
            indexAccessors.Add(typeof(T), AccessorBuilder.CreateIndexObjectAccessor<T>(tryGet));
        return this;
    }
    public StaticAccessorBuilder CreateMemberObjectAccessor<T>(TryGetMemberObjectAccessor<T> tryGet)
    {
        if (!memberAccessors.ContainsKey(typeof(T)))
            memberAccessors.Add(typeof(T), AccessorBuilder.CreateMemberObjectAccessor<T>(tryGet));
        return this;
    }

    internal StaticServiceAccesorVisitor Build()
    {
        return new(memberAccessors, indexAccessors);
    }
}