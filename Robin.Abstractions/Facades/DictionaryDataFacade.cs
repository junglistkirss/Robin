using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class IDictionaryIterator(IDictionary dictionary) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        foreach (object? item in dictionary)
        {
            action(item);
        }
    }
}

internal sealed class DictionaryDataFacade : IDataFacade
{
    public readonly static DictionaryDataFacade Instance = new();
    private DictionaryDataFacade() { }
    public bool IsTrue(object? obj) => obj is IDictionary value && value.Count > 0;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection)
    {
        if (obj is IDictionary value)
        {
            collection = new IDictionaryIterator(value);
            return true;
        }
        collection = null;
        return false;
    }
}
