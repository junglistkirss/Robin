using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public interface IEvaluator
{
    IDataFacade Resolve(IExpressionNode expression, DataContext? data);
}
public interface ITypedAccessor
{
    bool TryGetProperty(object? source, string name, [MaybeNullWhen(false)] out object? value);
}
public interface ITypedAccessor<T> : ITypedAccessor
{
    bool ITypedAccessor.TryGetProperty(object? source, string name, [MaybeNullWhen(false)] out object? value)
    {
        if (source is T typed)
            return TryGetProperty(typed, name, out value);
        value = null;
        return false;
    }
    bool TryGetProperty(T? source, string name, [MaybeNullWhen(false)] out object? value);
}

public delegate bool TryGetMemberValue<T>(T? source, string member, [MaybeNullWhen(false)] out object? value);

internal sealed class TypedAccessor<T>(TryGetMemberValue<T> tryGetMemberValue) : ITypedAccessor<T>
{
    public bool TryGetProperty(T? source, string name, [MaybeNullWhen(false)] out object? value)
    {
        return tryGetMemberValue(source, name, out value);
    }
}

internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : IAccessorVisitor<EvaluationResult, object?>
{
    private bool TryGetAccessor(object? data, [NotNullWhen(true)] out ITypedAccessor? accessor)
    {
        if (data is null)
        {
            accessor = null;
            return false;
        }
        Type type = typeof(ITypedAccessor<>).MakeGenericType(data.GetType());
        accessor = (ITypedAccessor?)serviceProvider.GetService(type);
        return accessor is not null;
    }

    public EvaluationResult VisitIndex(IndexAccessor accessor, object? args)
    {
        if (args is IList enumerable)
        {
            return new(ResoltionState.Found, enumerable[accessor.Index].AsFacade());
        }
        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitMember(MemberAccessor accessor, object? args)
    {
        if (args is not null && TryGetAccessor(args, out ITypedAccessor? typedAccessor) && typedAccessor.TryGetProperty(args, accessor.MemberName, out object? value))

            return new EvaluationResult(ResoltionState.Found, value.AsFacade());


        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitThis(ThisAccessor accessor, object? args)
    {
        return new(ResoltionState.Found, args.AsFacade());
    }
}
public sealed class ServiceEvaluator(IAccessorVisitor<EvaluationResult, object?> accesorVisitor) : IEvaluator
{
    public IDataFacade Resolve(IExpressionNode expression, DataContext? data)
    {
        var visitor = new ExpressionNodeVisitor(accesorVisitor);
        if (data is null)
            return DataFacade.Null;

        EvaluationResult result = expression.Accept(visitor, data);

        if (result.Status == ResoltionState.NotFound && data.Parent is not null)
            result = expression.Accept(visitor, data.Parent);

        if (result.Status == ResoltionState.Found)
            return result.Value;

        return DataFacade.Null;
    }
}

public static class EvaluatorExtensions
{
    public static IServiceCollection AddServiceEvaluator(this IServiceCollection services)
    {
        services.AddSingleton<ServiceEvaluator>();
        services.AddSingleton<IEvaluator, ServiceEvaluator>();
        services.AddSingleton<IAccessorVisitor<EvaluationResult, object?>, ServiceAccesorVisitor>();
        return services;
    }
}