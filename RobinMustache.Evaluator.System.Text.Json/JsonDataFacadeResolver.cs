using RobinMustache.Abstractions.Facades;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace RobinMustache.Evaluator.System.Text.Json;

internal sealed class JsonDataFacadeResolver : IDataFacadeResolver
{
    public bool ResolveDataFacade(object? data, [NotNullWhen(true)] out IDataFacade? facade)
    {
        if (data is null)
        {
            facade = DataFacade.Null;
            return true;
        }
        if (data is JsonNode)
        {
            facade = JsonNodeFacade.Instance;
            return true;
        }
        facade = null;
        return false;
    }
}
