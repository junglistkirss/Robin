using Robin.Abstractions.Accessors;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Robin.Abstractions;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : IVariableSegmentVisitor<EvaluationResult, object?>
{
    private static ConcurrentDictionary<Type, IMemberAccessor?> _membersAccessors = [];
    private static ConcurrentDictionary<Type, IIndexAccessor?> _indexAccessors = [];

    private bool TryGetMemberAccessor(object? data, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        if (data is IDictionary)
        {
            accessor = DictionaryMemberAccessor.Instance;
        }
        else
        {

            Type type = data.GetType();
            accessor = _membersAccessors.GetOrAdd(type, (t) =>
            {
                Type type = typeof(IMemberAccessor<>).MakeGenericType(t);
                IMemberAccessor? memberAccessor = (IMemberAccessor?)serviceProvider.GetService(type);
                if (memberAccessor is not null)
                    return memberAccessor;


                return memberAccessor;
            });
        }

        return accessor is not null;
    }

    private bool TryGetIndexAccessor(object? data, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
         if (data is IList)
        {
            accessor = ListIndexAccessor.Instance;
            return true;
        }
        else
        {
             Type type = data.GetType();
            accessor = _indexAccessors.GetOrAdd(type, (t) =>
            {
                Type type = typeof(IIndexAccessor<>).MakeGenericType(t);
                IIndexAccessor? indexAccessor = (IIndexAccessor?)serviceProvider.GetService(type);
                if (indexAccessor is not null)
                    return indexAccessor;


                return indexAccessor;
            });
        }
        
       
        return accessor is not null;
    }

    public EvaluationResult VisitIndex(IndexSegment segment, object? args)
    {
        if (args is not null && TryGetIndexAccessor(args, out IIndexAccessor? typedAccessor) && typedAccessor.TryGetIndex(args, segment.Index, out object? value))

            return new EvaluationResult(true, value);

        return new(false, null);
    }

    public EvaluationResult VisitMember(MemberSegment segment, object? args)
    {
        if (args is not null && TryGetMemberAccessor(args, out IMemberAccessor? typedAccessor)
            && typedAccessor.TryGetMember(args, segment.MemberName, out object? value))

            return new EvaluationResult(true, value);


        return new(false, null);
    }

    public EvaluationResult VisitThis(ThisSegment segment, object? args)
    {
        return new(true, args);
    }
}
