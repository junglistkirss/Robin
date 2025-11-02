namespace Robin.Abstractions.Facades;

public static class DataFacade
{
    public delegate IDataFacade DataFacadeFactory(object? data);



    public static readonly IDataFacade Null = new NullDataFacade();

    public static readonly IDataFacade Undefined = new UndefinedDataFacade();
    private readonly static HierarchicalTypeDictionary<DataFacadeFactory> _facadeFactories = new();

    public static bool RegisterFacadeFactory<T>(DataFacadeFactory factory)
    {
        return _facadeFactories.TryAdd<T>(factory);
    }

    public static IDataFacade AsFacade(this object? obj)
    {
        if (obj is null)
            return Null;

        switch (obj)
        {
            case bool v: return new BooleanDataFacade(v);
            case string v: return new LiteralDataFacade(v);
            case double v: return new DoubleDataFacade(v);
            case long v: return new DoubleDataFacade(v);
            case int v: return new IntDataFacade(v);
            case short v: return new ShortDataFacade(v);
            default:
                if (_facadeFactories.TryGetValue(obj.GetType(), out DataFacadeFactory? factory) && factory is not null)
                    return factory(obj);
                return Undefined;
        }
    }
}
