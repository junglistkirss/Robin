using Robin.Abstractions.Accessors;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Variables;
using Robin.Nodes;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : IVariableSegmentVisitor<EvaluationResult, SourceContext>
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
            accessor = DictionaryMemberAccessor.Instance;
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
            accessor = ListIndexAccessor.Instance;
            return true;
        }
        return false;
    }

    public EvaluationResult VisitIndex(IndexSegment segment, SourceContext args)
    {
        if (args is not null && int.TryParse(args.GetValue(segment.Index), out int index) && TryGetIndexAccessor(args.Data.Current, out IIndexAccessor? typedAccessor) && typedAccessor.TryGetIndex(args.Data.Current, index, out object? value))

            return new EvaluationResult(true, value);

        return new(false, null);
    }

    public EvaluationResult VisitMember(MemberSegment segment, SourceContext args)
    {
        string memberName = args.GetValue(segment.MemberName);
        if (args.Data.Current is not null && TryGetMemberAccessor(args.Data.Current, out IMemberAccessor? typedAccessor) 
            && typedAccessor.TryGetMember(args.Data.Current, memberName, out object? value))

            return new EvaluationResult(true, value);


        return new(false, null);
    }

    public EvaluationResult VisitThis(ThisSegment segment, SourceContext args)
    {
        return new(true, args.Data.Current);
    }
}
