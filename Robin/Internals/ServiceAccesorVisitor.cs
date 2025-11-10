using Robin.Abstractions.Accessors;
using Robin.Abstractions.Facades;
using System.Collections;
using System.Collections.Concurrent;

namespace Robin.Internals;


internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : BaseObjectAccessorVisitor
{
    private readonly ConcurrentDictionary<Type, IMemberAccessor?> memberCache = new();
    private readonly ConcurrentDictionary<Type, IIndexAccessor?> indexCache = new();

    protected override bool TryGetMemberAccessor(Type dataType, out IMemberAccessor? accessor)
    {
        accessor = memberCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IMemberAccessor<>).MakeGenericType(key);
            IMemberAccessor? memberAccessor = (IMemberAccessor?)serviceProvider.GetService(genType);
            if (memberAccessor is null && (key is IDictionary || (key.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(key.GetGenericTypeDefinition()))))
                memberAccessor = DictionaryMemberAccessor.Instance;
            return memberAccessor;
        });
        return accessor is not null;
    }

    protected override bool TryGetIndexAccessor(Type dataType, out IIndexAccessor? accessor)
    {
        accessor = indexCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IIndexAccessor<>).MakeGenericType(key);
            IIndexAccessor? indexAccessor = (IIndexAccessor?)serviceProvider.GetService(genType);
            if (indexAccessor is null && (key is IList || key.IsArray))
                indexAccessor = ListIndexAccessor.Instance;

            return indexAccessor;
        });



        return accessor is not null;
    }
}


