using Robin.Abstractions.Context;
using Robin.Abstractions.Nodes;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace Robin.Abstractions.Iterators;

public static class IteratorCache
{
    public delegate IIterator Factory(object? value);
    private static readonly ConcurrentDictionary<Type, Factory> _cache = new();

    public static bool GetIterator(object? value, out IIterator? collection)
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
        return _cache.GetOrAdd(type, (t) =>
        {
            if (t.IsArray)
                return _ => ArrayIterator.Instance;

            if (t.IsGenericType)
            {
                Type genericDef = t.GetGenericTypeDefinition();
                if (genericDef == typeof(ImmutableArray<>))
                {
                    Type elementType = t.GetGenericArguments()[0];
                    Type iterType = typeof(ImmutableArrayIterator<>).MakeGenericType(elementType);
                    return GetStaticInstance(iterType, nameof(ImmutableArrayIterator<object?>.Instance));
                }

                if (typeof(List<>).IsAssignableFrom(genericDef))
                {
                    Type elementType = t.GetGenericArguments()[0];
                    Type iterType = typeof(ListIterator<>).MakeGenericType(elementType);
                    return GetStaticInstance(iterType, nameof(ListIterator<object?>.Instance));
                }
            }

            if (typeof(IEnumerable).IsAssignableFrom(t))
                return _ => EnumerableIterator.Instance;

            return (_) => None;
        });

    }
    private static Factory GetStaticInstance(Type iterType, string fieldName)
    {

        LabelTarget returnLabel = Expression.Label(typeof(IIterator), "returnLabel");
        ParameterExpression param = Expression.Parameter(typeof(object), "_");


        FieldInfo staticInstance = iterType.GetField(fieldName, BindingFlags.Static | BindingFlags.Public) ?? throw new InvalidOperationException($"Aucun constructeur (object) trouvé sur {iterType}");

        MemberExpression retrieveStaticInstance = Expression.MakeMemberAccess(null, staticInstance);

        // label de retour (inutile ici mais conforme à ton code d'origine)

        GotoExpression returnExpr = Expression.Return(returnLabel, retrieveStaticInstance, typeof(IIterator));
        LabelExpression labelTarget = Expression.Label(returnLabel, Expression.Default(typeof(IIterator)));
        BlockExpression block = Expression.Block(returnExpr, labelTarget);
        Expression<Factory> lambda = Expression.Lambda<Factory>(block, param);
        Factory factory = lambda.Compile();
        return factory;
    }

    //private static Factory CreateFactory(Type iterType)
    //{

    //    LabelTarget returnLabel = Expression.Label(typeof(IIterator), "returnLabel");
    //    ParameterExpression param = Expression.Parameter(typeof(object), "instance");


    //    System.Reflection.ConstructorInfo ctor = iterType.GetConstructor([typeof(object)]) ?? throw new InvalidOperationException($"Aucun constructeur (object) trouvé sur {iterType}");
    //    NewExpression newExpr = Expression.New(ctor, param);

    //    // label de retour (inutile ici mais conforme à ton code d'origine)

    //    GotoExpression returnExpr = Expression.Return(returnLabel, newExpr, typeof(IIterator));
    //    LabelExpression labelTarget = Expression.Label(returnLabel, Expression.Default(typeof(IIterator)));
    //    BlockExpression block = Expression.Block(returnExpr, labelTarget);
    //    Expression<Factory> lambda = Expression.Lambda<Factory>(block, param);
    //    Factory factory = lambda.Compile();
    //    return factory;
    //}

    public static readonly IIterator None = new NoneIterator();

    private class NoneIterator : IIterator
    {
        public void Iterate(object? iterable, Action<object?> action) { }

        public void Iterate<T>(object? iterable, RenderContext<T> context, ReadOnlySpan<INode> partialTemplate, INodeVisitor<RenderContext<T>> visitor) where T : class
        { }
    }
}
