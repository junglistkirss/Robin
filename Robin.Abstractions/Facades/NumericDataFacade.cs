using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class NumericDataFacade : IDataFacade
{
    public readonly static NumericDataFacade Instance = new();
    private NumericDataFacade() { }
    public bool IsTrue(object? obj) => obj is float f ? f is not float.NaN : obj is not double d || d is not double.NaN;
    public bool IsCollection(object? _, [NotNullWhen(true)] out IEnumerable? collection)
    {
        collection = null;
        return false;
    }
}

