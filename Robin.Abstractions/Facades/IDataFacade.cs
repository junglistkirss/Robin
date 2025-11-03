using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

public interface IDataFacade
{
    bool IsTrue();
    bool IsCollection([NotNullWhen(true)] out IEnumerator? collection);
    object? RawValue { get; }
}
