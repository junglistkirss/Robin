using Robin.Abstractions.Facades;
using Robin.Internals;
using static Robin.Abstractions.Extensions.DataFacadeBuilder;

namespace Robin;

public sealed class StaticDataFacadeResolverBuilder
{
    private readonly Dictionary<Type, IDataFacade> facades = new Dictionary<Type, IDataFacade>();

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
