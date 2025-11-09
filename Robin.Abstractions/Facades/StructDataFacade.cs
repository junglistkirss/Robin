using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class StructDataFacade : IDataFacade
{
    public readonly static StructDataFacade Instance = new();
    private StructDataFacade() { }
    public bool IsTrue([NotNullWhen(true)] object? obj) => obj is not null;
    public bool IsCollection(object? _, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }
}