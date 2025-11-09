using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

public interface IDataFacadeResolver
{
    bool ResolveDataFacade(object? data, [NotNullWhen(true)] out IDataFacade? facade);
}
