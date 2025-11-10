using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Helpers;
using Robin.Abstractions.Nodes;
using Robin.Internals;
using System.Text;

namespace Robin;

public static class RendererExtensions
{
    private static IRenderer<TOut> CreateRenderer<T, TOut>(
        Func<T> builder,
        Func<T, TOut> output,
        INodeVisitor<RenderContext<T>> visitor,
        IEvaluator evaluator,
        Action<Helper>? helperConfig = null)
        where T : class
        => new RendererImpl<T, TOut>(builder, output, visitor, evaluator, helperConfig);

    public static IServiceCollection AddStringRenderer(
        this IServiceCollection services,
        Func<IServiceProvider, IEvaluator>? evaluatorProvider = null,
        Action<Helper>? helperConfig = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        services.Add(new ServiceDescriptor(typeof(IStringRenderer), typeof(StringRendererImpl), serviceLifetime));
        return services.AddRenderer(_ => new StringBuilder(), sb => sb.ToString(), evaluatorProvider ?? (s => s.GetRequiredService<IEvaluator>()), s => s.GetRequiredService<INodeVisitor<RenderContext<StringBuilder>>>(), helperConfig, serviceLifetime);
    }

    public static IServiceCollection AddRenderer<T, TOut>(
        this IServiceCollection services,
        Func<IServiceProvider, T> builderFactory,
        Func<T, TOut> output,
        Func<IServiceProvider, IEvaluator> evaluatorProvider,
        Func<IServiceProvider, INodeVisitor<RenderContext<T>>> visitorProvider,
        Action<Helper>? helperConfig = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where T : class
    {
        services.Add(new ServiceDescriptor(typeof(IRenderer<TOut>), factory: sp =>
        {
            T builder() => builderFactory(sp);
            IEvaluator evaluator = evaluatorProvider(sp);
            INodeVisitor<RenderContext<T>> visitor = visitorProvider(sp);
            return CreateRenderer(builder, output, visitor, evaluator, helperConfig);
        }, serviceLifetime));
        return services;
    }
}
