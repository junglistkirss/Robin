using Robin.Abstractions.Accessors;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Robin.Abstractions.Facades;
internal sealed class ArrayIterator(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is Array arr)
        {
            foreach (var item in arr)
                action(item);
        }
    }
}

internal sealed class ImmutableArrayIterator<T>(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is ImmutableArray<T> arr)
        {
            var span = arr.AsSpan();
            for (int i = 0; i < span.Length; i++)
                action(span[i]!); // nullable suppression si nécessaire
        }
    }
}

internal sealed class ListIterator<T>(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is List<T> list)
        {
            foreach (object? item in list)
                action(item);
        }
    }
}

public static class IteratorCache
{
    public delegate IIterator Factory(object? value);
    private static readonly ConcurrentDictionary<Type, Factory> _cache = new();

    public static bool GetIterator(object? value, [NotNullWhen(true)] out IIterator? collection)
    {
        if (value is IEnumerable)
        {
            Factory factory = GetIteratorFactory(value.GetType());
            collection = factory(value);
            return true;
        }

        collection = null;
        return false;
    }
    public static Factory GetIteratorFactory(Type type)
    {
        return _cache.GetOrAdd(type, (Type t) =>
        {
            if (t.IsArray)
                return o => new ArrayIterator(o);

            if (t.IsGenericType)
            {
                var genericDef = t.GetGenericTypeDefinition();
                if (genericDef == typeof(ImmutableArray<>))
                {
                    var elementType = t.GetGenericArguments()[0];
                    var iterType = typeof(ImmutableArrayIterator<>).MakeGenericType(elementType);
                    return CreateFactory(iterType);
                }

                if (typeof(List<>).IsAssignableFrom(genericDef))
                {
                    var elementType = t.GetGenericArguments()[0];
                    var iterType = typeof(ListIterator<>).MakeGenericType(elementType);
                    return CreateFactory(iterType);
                }
            }

            
            if (typeof(IEnumerable).IsAssignableFrom(t))
                return o => new EnumerableIterator(o);

            return (_) => _empty;
        });

    }

    private static Factory CreateFactory(Type iterType)
    {
        var ctor = iterType.GetConstructor([typeof(object)]) ?? throw new InvalidOperationException($"Aucun constructeur (object) trouvé sur {iterType}");

        var param = Expression.Parameter(typeof(object), "instance");
        var newExpr = Expression.New(ctor, param);

        // label de retour (inutile ici mais conforme à ton code d'origine)
        var returnLabel = Expression.Label(typeof(IIterator), "returnLabel");

        var returnExpr = Expression.Return(returnLabel, newExpr, typeof(IIterator));
        var labelTarget = Expression.Label(returnLabel, Expression.Default(typeof(IIterator)));
        var block = Expression.Block(returnExpr, labelTarget);
        var lambda = Expression.Lambda<Factory>(block, param);
        Factory factory = lambda.Compile();
        return factory;
    }

    private static readonly IIterator _empty = new EmptyIterator();

    private class EmptyIterator : IIterator
    {
        public void Iterate(Action<object?> action) { }
    }
}

internal sealed class IListIterator(IList list) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        foreach (object? item in list)
            action(item);
    }
}

internal sealed class IListDataFacade : IDataFacade
{
    public readonly static IListDataFacade Instance = new();
    private IListDataFacade() { }
    public bool IsTrue(object? obj) => obj is IList list && list.Count > 0;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IIterator? collection)
    {
        return IteratorCache.GetIterator(obj, out collection);
    }
}
