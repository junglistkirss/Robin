using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : IVariableSegmentVisitor<EvaluationResult, object?>
{
    private bool TryGetMemberAccessor(object? data, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        Type type = typeof(IMemberAccessor<>).MakeGenericType(data.GetType());
        accessor = (IMemberAccessor?)serviceProvider.GetService(type);
        if (accessor is not null)
            return true;

        if (data is IDictionary)
        {
            accessor = new DictionaryMemberAccessor();
            return true;
        }
        return false;
    }

    private bool TryGetIndexAccessor(object? data, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        Type type = typeof(IIndexAccessor<>).MakeGenericType(data.GetType());
        accessor = (IIndexAccessor?)serviceProvider.GetService(type);
        
        if (accessor is not null)
            return true;

        if (data is IList)
        {
            accessor = new ListIndexAccessor();
            return true;
        }
        return false;
    }

    public EvaluationResult VisitIndex(IndexSegment segment, object? args)
    {
        if (args is not null && TryGetIndexAccessor(args, out IIndexAccessor? typedAccessor) && typedAccessor.TryGetIndex(args, segment.Index, out object? value))

            return new EvaluationResult(ResoltionState.Found, value.AsFacade());

        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitMember(MemberSegment segment, object? args)
    {
        if (args is not null && TryGetMemberAccessor(args, out IMemberAccessor? typedAccessor) && typedAccessor.TryGetMember(args, segment.MemberName, out object? value))

            return new EvaluationResult(ResoltionState.Found, value.AsFacade());


        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitThis(ThisSegment segment, object? args)
    {
        return new(ResoltionState.Found, args.AsFacade());
    }
}
