using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Accessors;
using Robin.Abstractions.Context;
using Robin.Contracts.Variables;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Extensions;

public static class EvaluatorExtensions
{

    public static IServiceCollection AddServiceEvaluator(this IServiceCollection services)
    {
        services.AddSingleton<ServiceEvaluator>();
        services.AddSingleton<IEvaluator, ServiceEvaluator>();
        services.AddSingleton<IVariableSegmentVisitor<EvaluationResult, SourceContext>, ServiceAccesorVisitor>();
        return services;
    }
}