using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class LiteralDataFacade : IDataFacade
{
    public readonly static LiteralDataFacade Instance = new();
    private LiteralDataFacade() { }
    public bool IsTrue([NotNullWhen(true)] object? obj) => obj is string value && !string.IsNullOrEmpty(value);
    public bool IsCollection(object? _, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }
}
