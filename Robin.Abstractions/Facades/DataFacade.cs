using System.Collections;

namespace Robin.Abstractions.Facades;

public static class DataFacade
{
    public delegate IDataFacade DataFacadeFactory(object? data);

    public static readonly IDataFacade Null = NullDataFacade.Instance;

    public static IDataFacade GetPrimitiveFacade(this object? obj)
    {
        if (obj is null)
            return Null;

        return obj switch
        {
            // primitives
            bool => BooleanDataFacade.Instance,
            byte => NumericDataFacade.Instance,
            sbyte => NumericDataFacade.Instance,
            decimal => NumericDataFacade.Instance,
            double => NumericDataFacade.Instance,
            float => NumericDataFacade.Instance,
            int => NumericDataFacade.Instance,
            uint => NumericDataFacade.Instance,
            nint => NumericDataFacade.Instance,
            long => NumericDataFacade.Instance,
            ulong => NumericDataFacade.Instance,
            short => NumericDataFacade.Instance,
            ushort => NumericDataFacade.Instance,
            // commons types
            string => LiteralDataFacade.Instance,
            char => CharDataFacade.Instance,
            Guid => StructDataFacade.Instance,
            DateTime => StructDataFacade.Instance,
            DateTimeOffset => StructDataFacade.Instance,
            //DateOnly => StructDataFacade.Instance,
            //TimeOnly => StructDataFacade.Instance,
            TimeSpan => StructDataFacade.Instance,
            // collection
            IDictionary => DictionaryDataFacade.Instance,
            IList => IListDataFacade.Instance,
            IEnumerator => EnumeratorDataFacade.Instance,
            _ => ObjectDataFacade.Instance,
        };
    }
}
