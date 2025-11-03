using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class BooleanDataFacade(bool? value) : IDataFacade
{
    public static readonly BooleanDataFacade True = new(true);
    public static readonly BooleanDataFacade False = new(false);

    public object? RawValue => value;
    public bool IsCollection() => false;
    public bool IsTrue() => value == true;
    public bool IsCollection([NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = null;
        return false;
    }
}