using Robin.Abstractions.Facades;
using System.Collections.Concurrent;

namespace Robin.Internals;

internal sealed class DataFacadeResolver(IServiceProvider provider) : IDataFacadeResolver
{
    private readonly ConcurrentDictionary<Type, IDataFacade?> cache = new();

    public bool ResolveDataFacade(object? data, out IDataFacade? facade)
    {
        if (data is null)
        {
            facade = DataFacade.Null;
            return true;
        }
        Type type = Nullable.GetUnderlyingType(data.GetType()) ?? data.GetType();
        facade = cache.GetOrAdd(type, (_) =>
        {
            Type genType = typeof(IDataFacade<>).MakeGenericType(type);
            IDataFacade? cachedFacade = (IDataFacade?)provider.GetService(genType);
            return cachedFacade;
        });
        return facade is not null;
    }
}
