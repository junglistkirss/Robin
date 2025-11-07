using System.Collections;

namespace Robin.Abstractions.Facades;

public interface IDataFacadeResolver
{
    IDataFacade ResolveDataFacade(object? data);
}

public static class DataFacade
{
    public delegate IDataFacade DataFacadeFactory(object? data);

    public static readonly IDataFacade Null = NullDataFacade.Instance;

    public static IDataFacade GetFacade(this object? obj)
    {
        if (obj is null)
            return Null;

        switch (obj)
        {
            // primitives
            case bool: return BooleanDataFacade.Instance;
            case byte: return NumericDataFacade.Instance;
            case sbyte: return NumericDataFacade.Instance;
            case decimal: return NumericDataFacade.Instance;
            case double: return NumericDataFacade.Instance;
            case float: return NumericDataFacade.Instance;
            case int: return NumericDataFacade.Instance;
            case uint: return NumericDataFacade.Instance;
            case nint: return NumericDataFacade.Instance;
            case long: return NumericDataFacade.Instance;
            case ulong: return NumericDataFacade.Instance;
            case short: return NumericDataFacade.Instance;
            case ushort: return NumericDataFacade.Instance;
            // commons types
            case string: return LiteralDataFacade.Instance;
            case char: return CharDataFacade.Instance;
            case Guid: return StructDataFacade.Instance;
            case DateTime: return StructDataFacade.Instance;
            case DateTimeOffset: return StructDataFacade.Instance;
            case DateOnly: return StructDataFacade.Instance;
            case TimeOnly: return StructDataFacade.Instance;
            case TimeSpan: return StructDataFacade.Instance;
            // collection
            case IDictionary: return DictionaryDataFacade.Instance;
            case IList: return IListDataFacade.Instance;
            case IEnumerator: return EnumeratorDataFacade.Instance;
            default: return ObjectDataFacade.Instance;
        }
    }
}
