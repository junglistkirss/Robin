using System.Text;

namespace Robin.Nodes;

public class Context(IReadOnlyDictionary<string, object> variables)
{
    private readonly IReadOnlyDictionary<string, object> _variables = variables;
    public Dictionary<string, Action<Context, INode, StringBuilder>> Helpers { get; } = [];

    public bool TryResolve(string name, out object? value) => _variables.TryGetValue(name, out value);
}

