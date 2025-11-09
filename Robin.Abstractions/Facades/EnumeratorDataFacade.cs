using Robin.Abstractions.Iterators;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class EnumeratorDataFacade : IDataFacade
{
    public readonly static EnumeratorDataFacade Instance = new();
    private EnumeratorDataFacade() { }
    public bool IsTrue([NotNullWhen(true)] object? _) => true;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection)
    {
        collection = IEnumeratorIterator.Instance;
        return true;
    }
}
