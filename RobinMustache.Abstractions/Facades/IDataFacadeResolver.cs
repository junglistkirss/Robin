namespace RobinMustache.Abstractions.Facades;

public interface IDataFacadeResolver
{
    bool ResolveDataFacade(object? data, out IDataFacade? facade);
}
