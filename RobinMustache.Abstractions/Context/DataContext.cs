using RobinMustache.Abstractions.Helpers;
using System.Collections.Concurrent;

namespace RobinMustache.Abstractions.Context;

public sealed class DataContext
{
    private static readonly AsyncLocal<DataContext?> _current = new();
    private static readonly ConcurrentBag<DataContext> _pool = new();
    private const int MaxPoolSize = 10;
    private object? _data;
    private Helper _helper = new();
    private DataContext? _parent;

    private DataContext() { }

    public object? Data => _data;
    public Helper Helper => _helper;
    public DataContext? Parent => _parent;

    public static DataContext Current => _current.Value ?? GetFromPool(null, null);

    private static DataContext GetFromPool(object? data, DataContext? parent)
    {
        if (!_pool.TryTake(out var ctx))
            ctx = new DataContext();

        ctx._data = data;
        ctx._parent = parent;
        ctx._helper = new Helper(); // reset ou réutilisable via pool si nécessaire
        _current.Value = ctx;
        return ctx;
    }

    public static IDisposable Push(object? data)
    {
        var parent = _current.Value;
        var child = GetFromPool(data, parent);
        _current.Value = child;
        return new Popper(parent, child);
    }
    private static void ReturnToPool(DataContext ctx)
    {
        if (_pool.Count < MaxPoolSize)
            _pool.Add(ctx);
        // sinon on laisse l'objet à GC
    }
    private sealed class Popper : IDisposable
    {
        private readonly DataContext? _previous;
        private readonly DataContext _popped;
        private bool _disposed;

        public Popper(DataContext? previous, DataContext popped)
        {
            _previous = previous;
            _popped = popped;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _current.Value = _previous;

                // reset et retour au pool
                _popped._data = null;
                _popped._parent = null;
                _popped._helper = new Helper();
                ReturnToPool(_popped);

                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}