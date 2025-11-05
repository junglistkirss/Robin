using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class StructDataFacade : IDataFacade
{
    public readonly static StructDataFacade Instance = new();
    private StructDataFacade() { }
    public bool IsTrue(object? obj) => obj is not null;
    public bool IsCollection(object? _, [NotNullWhen(true)] out IEnumerable? collection)
    {
        collection = null;
        return false;
    }
}