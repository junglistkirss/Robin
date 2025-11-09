using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class NullDataFacade : IDataFacade
{
    public readonly static NullDataFacade Instance = new();
    private NullDataFacade() { }
    public bool IsCollection(object? _, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = null;
        return false;
    }

    public bool IsTrue([NotNullWhen(true)] object? _) => false;
}
