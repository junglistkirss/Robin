using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class BooleanDataFacade : IDataFacade
{
    public readonly static BooleanDataFacade Instance = new();
    private BooleanDataFacade() { }

    public bool IsTrue(object? obj) => obj is bool b && b;
    public bool IsCollection(object? _, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }
}