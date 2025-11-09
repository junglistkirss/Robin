using Robin.Abstractions.Iterators;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class DictionaryDataFacade : IDataFacade
{
    public readonly static DictionaryDataFacade Instance = new();
    private DictionaryDataFacade() { }
    public bool IsTrue([NotNullWhen(true)] object? obj) => obj is IDictionary value && value.Count > 0;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = IDictionaryIterator.Instance;
        return true;
    }
}
