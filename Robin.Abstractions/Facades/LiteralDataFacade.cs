using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class LiteralDataFacade : IDataFacade
{
    public readonly static LiteralDataFacade Instance = new();
    private LiteralDataFacade() { }
    public bool IsTrue(object? obj) => obj is string value && !string.IsNullOrEmpty(value);
    public bool IsCollection(object? _, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }
}
