using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin.Abstractions.Context;

public record class RenderContext<T>
    where T : class
{
    public ImmutableDictionary<string, ImmutableArray<INode>> Partials { get; init; } = ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
    public DataContext? Data { get; init; }
    public required IEvaluator Evaluator { get; init; }
    public required T Builder { get; init; }

}



