using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class DictionaryDataFacade : IDataFacade
{
    public readonly static DictionaryDataFacade Instance = new();
    private DictionaryDataFacade() { }
    public bool IsTrue(object? obj) => obj is IDictionary value && value.Count > 0;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IEnumerable? collection)
    {
        if (obj is IDictionary value)
        {
            collection = value;
            return value.Count > 0;
        }
        collection = null;
        return false;
    }
}
