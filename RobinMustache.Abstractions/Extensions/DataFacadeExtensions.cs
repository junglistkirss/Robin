using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions.Facades;
using static RobinMustache.Abstractions.Extensions.DataFacadeBuilder;

namespace RobinMustache.Abstractions.Extensions;

public static class DataFacadeExtensions
{
    public static IServiceCollection AddDataFacade<T>(this IServiceCollection services, IDataFacade<T> facade)
    {
        if (facade is null)
            throw new ArgumentNullException(nameof(facade));
        return services.AddSingleton<IDataFacade<T>>(facade);
    }

    public static IServiceCollection AddDataFacadeFactory<T>(this IServiceCollection services, DataFacadeFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));
        return services.AddSingleton<IDataFacade<T>>(CreateDataFacade<T>(factory));
    }
}
