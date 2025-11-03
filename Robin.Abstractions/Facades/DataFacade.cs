using System.Collections;

namespace Robin.Abstractions.Facades;

public static class DataFacade
{
    public delegate IDataFacade DataFacadeFactory(object? data);



    public static readonly IDataFacade Null = new NullDataFacade();

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
            // primitives
            case bool v: return new BooleanDataFacade(v);
            case byte v: return new NumericDataFacade<byte>(v);
            case sbyte v: return new NumericDataFacade<sbyte>(v);
            case decimal v: return new NumericDataFacade<decimal>(v);
            case double v: return new NumericDataFacade<double>(v);
            case float v: return new NumericDataFacade<float>(v);
            case int v: return new NumericDataFacade<int>(v);
            case uint v: return new NumericDataFacade<uint>(v);
            case nint v: return new NumericDataFacade<nint>(v);
            case long v: return new NumericDataFacade<long>(v);
            case ulong v: return new NumericDataFacade<ulong>(v);
            case short v: return new NumericDataFacade<short>(v);
            case ushort v: return new NumericDataFacade<ushort>(v);
            // commons types
            case string v: return new LiteralDataFacade(v);
            case char v: return new CharDataFacade(v);
            case Guid v: return new StructDataFacade<Guid>(v);
            case DateTime v: return new StructDataFacade<DateTime>(v);
            case DateTimeOffset v: return new StructDataFacade<DateTimeOffset>(v);
            case DateOnly v: return new StructDataFacade<DateOnly>(v);
            case TimeOnly v: return new StructDataFacade<TimeOnly>(v);
            case TimeSpan v: return new StructDataFacade<TimeSpan>(v);
            // collection
            case IDictionary v: return new DictionaryDataFacade(v);
            case IList v: return new ListDataFacade(v);
            case IEnumerator v: return new EnumeratorDataFacade(v);
            default:
                if (_facadeFactories.TryGetValue(obj.GetType(), out DataFacadeFactory? factory) && factory is not null)
                    return factory(obj);
                return new ObjectDataFacade(obj);
        }
    }
}
