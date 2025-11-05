using Robin.Abstractions.Helpers;

namespace Robin.Abstractions.Context;

public record SourceContext
{
    public Helper Helper { get; } = new();
    public required DataContext Data { get; init; }
    public required string Source { get; init; }

    public string GetValue(Range range)
    {
        return Source[range];
    }
}


public record RenderContext
{
    //public ImmutableDictionary<string, ImmutableArray<INode>> Partials { get; init; } = ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
    public required SourceContext Context { get; init; }
    public required IEvaluator Evaluator { get; init; }



}

public record RenderContext<T> : RenderContext
    where T : class
{
    public required T Builder { get; init; }

}



