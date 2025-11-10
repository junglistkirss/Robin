using RobinMustache.Abstractions.Facades;
using RobinMustache.Internals;
using static RobinMustache.Abstractions.Extensions.DataFacadeBuilder;

namespace RobinMustache;

public sealed class StaticDataFacadeResolverBuilder
{
    private readonly Dictionary<Type, IDataFacade> facades = [];

    public StaticDataFacadeResolverBuilder CreateIndexObjectAccessor<T>(DataFacadeFactory factory)
    {
        if (!facades.ContainsKey(typeof(T)))
            facades.Add(typeof(T), CreateDataFacade<T>(factory));
        return this;
    }

    public StaticDataFacadeResolverBuilder Add<T>(IDataFacade<T> facade)
    {
        if (!facades.ContainsKey(typeof(T)))
            facades.Add(typeof(T), facade);
        return this;
    }
    internal StaticDataFacadeResolver Build()
    {
        return new(facades);
    }
}
