using Robin.Abstractions.Nodes;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Robin.Abstractions.Context;

public static class RenderContextPool<T> where T : class
{
    private static readonly ConcurrentBag<RenderContext<T>> Pool = new();
    private const int MaxPoolSize = 5;

    public static RenderContext<T> Get(
        IEvaluator evaluator,
        T builder,
        ReadOnlyDictionary<string, ImmutableArray<INode>>? partials = null)
    {
        if (!Pool.TryTake(out var ctx))
            ctx = new RenderContext<T>();

        ctx.Evaluator = evaluator;
        ctx.Builder = builder;
        ctx.Partials = partials;

        return ctx;
    }

    public static void Return(RenderContext<T> ctx)
    {
        ctx.Partials = null;
        ctx.Builder = null!;
        ctx.Evaluator = null!;
        if (Pool.Count < MaxPoolSize)
            Pool.Add(ctx);
    }
}
