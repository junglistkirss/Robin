using RobinMustache.Abstractions.Nodes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace RobinMustache.Abstractions.Context;

public sealed class PartialsScope<T> : IDisposable
    where T : class
{
    private readonly RenderContext<T> _context;
    private readonly ReadOnlyDictionary<string, ImmutableArray<INode>>? _original;

    public PartialsScope(RenderContext<T> context, ReadOnlyDictionary<string, ImmutableArray<INode>>? newPartials)
    {
        _context = context;
        _original = context.Partials;
        _context.Partials = newPartials;
    }

    public void Dispose()
    {
        _context.Partials = _original;
        GC.SuppressFinalize(this);
    }
}