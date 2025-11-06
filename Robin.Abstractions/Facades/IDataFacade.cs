using Robin.Abstractions.Accessors;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

public interface IDataFacade
{
    bool IsTrue(object? obj);
    bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection);
}
