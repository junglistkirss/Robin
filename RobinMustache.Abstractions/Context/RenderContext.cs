using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace RobinMustache.Abstractions.Context;

public abstract class RenderContext
{
    public ReadOnlyDictionary<string, ImmutableArray<INode>>? Partials { get; internal set; }
    public IEvaluator Evaluator { get; internal set; } = null!;
}

public sealed class RenderContext<T> : RenderContext
    where T : class
{
    public T Builder { get; internal set; } = null!;
}
