using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : IVariableSegmentVisitor<EvaluationResult, object?>
{
    private bool TryGetMemberAccessor(object? data, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        if (TryGetKeyedMemberAccessor(data, out IMemberAccessor? keyed))
        {
            accessor = keyed;
            return true;
        }
        if (TryGetDynamicMemberAccessor(data, out IMemberAccessor? dynamic))
        {
            accessor = dynamic;
            return accessor is not null;
        }
        accessor = null;
        return false;
    }

    private bool TryGetIndexAccessor(object? data, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        if (TryGetKeyedIndexAccessor(data, out IIndexAccessor? keyed))
        {
            accessor = keyed;
            return true;
        }
        if( TryGetDynamicIndexAccessor(data, out IIndexAccessor? dynamic))
        {
            accessor = dynamic;
            return accessor is not null;
        }
        accessor = null;
        return false;
    }
    private bool TryGetKeyedMemberAccessor(object? data, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        accessor = (IMemberAccessor?)serviceProvider.GetRequiredKeyedService(typeof(IMemberAccessor), data.GetType());
        return accessor is not null;
    }

    private bool TryGetKeyedIndexAccessor(object? data, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        accessor = (IIndexAccessor?)serviceProvider.GetRequiredKeyedService(typeof(IIndexAccessor), data.GetType());
        return accessor is not null;
    }

    private bool TryGetDynamicMemberAccessor(object? data, [NotNullWhen(true)] out IMemberAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        Type type = typeof(IMemberAccessor<>).MakeGenericType(data.GetType());
        accessor = (IMemberAccessor?)serviceProvider.GetService(type);
        return accessor is not null;
    }

    private bool TryGetDynamicIndexAccessor(object? data, [NotNullWhen(true)] out IIndexAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        Type type = typeof(IIndexAccessor<>).MakeGenericType(data.GetType());
        accessor = (IIndexAccessor?)serviceProvider.GetService(type);
        return accessor is not null;
    }

    public EvaluationResult VisitIndex(IndexSegment segment, object? args)
    {
        if (args is not null && TryGetIndexAccessor(args, out IIndexAccessor? typedAccessor) && typedAccessor.TryGetIndex(args, segment.Index, out object? value))

            return new EvaluationResult(ResoltionState.Found, value.AsFacade());

        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitMember(MemberISegment segment, object? args)
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
