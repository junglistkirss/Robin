using Robin.Abstractions.Accessors;

namespace Robin.Abstractions.Extensions;

public static class AccessorBuilder
{
    private sealed class ObjectMemberAccessor<T>(TryGetMemberObjectAccessor<T> tryGetMemberValue) : BaseMemberAccessor<T>
    {
        public override bool TryGetMember(T obj, string name, out object? value)
        {
            if (tryGetMemberValue is null)
                throw new ArgumentNullException(nameof(tryGetMemberValue));
            return tryGetMemberValue(obj, name, out value);
        }
    }
    private sealed class ObjectIndexAccessor<T>(TryGetIndexObjectAccessor<T> tryGetIndexValue) : BaseIndexAccessor<T>
    {
        public override bool TryGetIndex(T obj, int index, out object? value)
        {
            if (tryGetIndexValue is null)
                throw new ArgumentNullException(nameof(tryGetIndexValue));
            return tryGetIndexValue(obj, index, out value);
        }
    }

    public delegate bool TryGetMemberObjectAccessor<T>(T obj, string member, out object? value);
    public delegate bool TryGetIndexObjectAccessor<T>(T obj, int index, out object? value);
    public static IIndexAccessor<T> CreateIndexObjectAccessor<T>(TryGetIndexObjectAccessor<T> tryGet)
    {
        if (tryGet is null)
            throw new ArgumentNullException(nameof(tryGet));
        return new ObjectIndexAccessor<T>(tryGet);
    }
    public static IMemberAccessor<T> CreateMemberObjectAccessor<T>(TryGetMemberObjectAccessor<T> tryGet)
    {
        if (tryGet is null)
            throw new ArgumentNullException(nameof(tryGet));
        return new ObjectMemberAccessor<T>(tryGet);
    }
}