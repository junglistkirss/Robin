using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class CharDataFacade : IDataFacade
{
    public readonly static CharDataFacade Instance = new();
    private CharDataFacade() { }
    public bool IsTrue([NotNullWhen(true)] object? obj) => obj is char c && c is not '\0';
    public bool IsCollection(object? _, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }
}
