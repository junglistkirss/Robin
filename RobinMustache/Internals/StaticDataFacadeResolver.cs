using RobinMustache.Abstractions.Facades;
using System.Collections.Concurrent;

namespace RobinMustache.Internals;

internal class StaticDataFacadeResolver(IEnumerable<KeyValuePair<Type, IDataFacade>> facades) : IDataFacadeResolver
{
    private readonly ConcurrentDictionary<Type, IDataFacade> cache = new(facades);

    public bool ResolveDataFacade(object? data, out IDataFacade? facade)
    {
        if (data is null)
        {
            facade = DataFacade.Null;
            return true;
        }
        Type type = Nullable.GetUnderlyingType(data.GetType()) ?? data.GetType();
        return cache.TryGetValue(type, out facade);
    }
}


