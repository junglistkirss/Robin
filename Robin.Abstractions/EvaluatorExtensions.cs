using Microsoft.Extensions.DependencyInjection;
using Robin.Contracts.Variables;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;

public static class EvaluatorExtensions
{

    public delegate bool TryGetMemberValue<T>(T? source, string member, [MaybeNullWhen(false)] out object? value);
    public delegate bool TryGetIndexValue<T>(T? source, int index, [MaybeNullWhen(false)] out object? value);

    private sealed class DelegatedMemberAccessor<T>(TryGetMemberValue<T> tryGetMemberValue) : IMemberAccessor<T>
    {
        public bool TryGetMember(T? source, string name, [MaybeNullWhen(false)] out object? value)
        {
            return tryGetMemberValue(source, name, out value);
        }
    }
    public static IServiceCollection AddMemberAccessor<T>(this IServiceCollection services, TryGetMemberValue<T> tryGet)
    {
        DelegatedMemberAccessor<T> instance = new(tryGet);
        return services
            .AddSingleton<IMemberAccessor<T>>(instance)
            .AddKeyedSingleton<IMemberAccessor>(typeof(T), instance);
    }

    private sealed class DelegatedIndexAccessor<T>(TryGetIndexValue<T> tryGetIndexValue) : IIndexAccessor<T>
    {
        public bool TryGetIndex(T? source, int index, [MaybeNullWhen(false)] out object? value)
        {
            return tryGetIndexValue(source, index, out value);
        }
    }
    public static IServiceCollection AddIndexAccessor<T>(this IServiceCollection services, TryGetIndexValue<T> tryGet)
    {
        DelegatedIndexAccessor<T> instance = new(tryGet);
        return services
            .AddSingleton<IIndexAccessor<T>>(instance)
            .AddKeyedSingleton<IIndexAccessor>(typeof(T), instance);
    }

    public static IServiceCollection AddServiceEvaluator(this IServiceCollection services)
    {
        services.AddSingleton<ServiceEvaluator>();
        services.AddSingleton<IEvaluator, ServiceEvaluator>();
        services.AddSingleton<IVariableSegmentVisitor<EvaluationResult, object?>, ServiceAccesorVisitor>();
        return services;
    }
}