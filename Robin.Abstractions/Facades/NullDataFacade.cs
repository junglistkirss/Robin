using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class NullDataFacade : IDataFacade
{
    public readonly static NullDataFacade Instance = new();
    private NullDataFacade() { }
    public bool IsCollection(object? _, [NotNullWhen(true)] out IEnumerable? collection)
    {
        collection = null;
        return false;
    }

    public bool IsTrue(object? _) => false;
}
