using Robin.Abstractions.Accessors;
using Robin.Contracts.Variables;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;


internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : BaseAccessorVisitor
{
    private readonly ConcurrentDictionary<Type, IMemberAccessor?> memberCache = new();
    private readonly ConcurrentDictionary<Type, IIndexAccessor?> indexCache = new();

    private bool TryGetMemberAccessor(Type dataType, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        accessor = memberCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IMemberAccessor<>).MakeGenericType(key);
            IMemberAccessor? memberAccessor = (IMemberAccessor?)serviceProvider.GetService(genType);
            if (memberAccessor is null && (key.IsAssignableTo(typeof(IDictionary)) || (key.IsGenericType && key.GetGenericTypeDefinition().IsAssignableTo(typeof(IDictionary<,>)))))
                memberAccessor = DictionaryMemberAccessor.Instance;
            return memberAccessor;
        });
        return accessor is not null;
    }

    private bool TryGetIndexAccessor(Type dataType, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        accessor = indexCache.GetOrAdd(dataType, (key) =>
        {
            Type genType = typeof(IIndexAccessor<>).MakeGenericType(key);
            IIndexAccessor? indexAccessor = (IIndexAccessor?)serviceProvider.GetService(genType);
            if (indexAccessor is null && (key.IsAssignableTo(typeof(IList)) || key.IsArray))
                indexAccessor = ListIndexAccessor.Instance;

            return indexAccessor;
        });



        return accessor is not null;
    }

    public override bool VisitIndex(IndexSegment segment, Type args, out ChainableGetter getter)
    {
        if (TryGetIndexAccessor(args, out IIndexAccessor? typedAccessor))
        {
            int index = segment.Index;
            getter = new ChainableGetter((object? input, out object? value) =>
            {
                if (typedAccessor.TryGetIndex(input, index, out object? indexValue))
                {
                    value = indexValue;
                    return true;
                }
                value = null;
                return false;
            });
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }

    public override bool VisitMember(MemberSegment segment, Type args, out ChainableGetter getter)
    {
        if (TryGetMemberAccessor(args, out IMemberAccessor? typedAccessor))
        {
            string memberName = segment.MemberName;
            getter = new ChainableGetter((object? input, out object? value) =>
            {
                if (typedAccessor.TryGetMember(input, memberName, out object? memberValue))
                {
                    value = memberValue;
                    return true;
                }
                value = null;
                return false;
            });
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }
}
