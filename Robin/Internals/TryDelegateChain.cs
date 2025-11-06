using System.Diagnostics.CodeAnalysis;

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Robin.Internals;

internal delegate object? ChainableGetter(object? input, ref int level);
internal delegate bool ChainedGetter(object? input, out object? value);

internal static class DelegateHelper
{
    public static ChainedGetter BuildResolver(ChainableGetter[] chain)
    {
        ArgumentNullException.ThrowIfNull(chain);
        if (chain.Length == 0)
            throw new InvalidDataException("Chain is empty");

        // Paramètres
        ParameterExpression inputParam = Expression.Parameter(typeof(object), "input");
        ParameterExpression valueParam = Expression.Parameter(typeof(object).MakeByRefType(), "value"); // out param
        LabelTarget returnLabel = Expression.Label(typeof(bool), "returnLabel");

        // int level;
        ParameterExpression levelVar = Expression.Variable(typeof(int), "level");
        // int level = 0;
        // BinaryExpression initLevelVar = Expression.Assign(levelVar, Expression.Constant(0));
        Expression current = inputParam;
        foreach (ChainableGetter f in chain)
        {
            var funcConst = Expression.Constant(f);
            current = Expression.Invoke(funcConst, current, levelVar);
        }
        var assignOutput = Expression.Assign(valueParam, current!);

        var returnTrue = Expression.Return(returnLabel, Expression.Constant(true));
        var invokeBlock = Expression.Block([], assignOutput, returnTrue);

        // value = null; 
        var assignNull = Expression.Assign(valueParam, Expression.Constant(null, typeof(object)));

        // level > 0
        var compareLevel = Expression.GreaterThan(levelVar, Expression.Constant(1));
        // return level > 0 
        var catchBlock = Expression.Block(assignNull, Expression.Return(returnLabel, compareLevel));
        var tryCatchExpr = Expression.TryCatch(invokeBlock, Expression.Catch(typeof(ResolutionException), catchBlock));
        var body = Expression.Block(
            [levelVar],
            tryCatchExpr,
            Expression.Label(returnLabel, Expression.Constant(false)) // valeur de fallback
        );
        var lambda = Expression.Lambda<ChainedGetter>(body, inputParam, valueParam);

        return lambda.Compile();
    }
    public static ChainableGetter AsChainable(this Delegate del)
    {
        ArgumentNullException.ThrowIfNull(del);

        MethodInfo method = del.Method;
        ConstantExpression? target = del.Target == null ? null : Expression.Constant(del.Target);

        // Paramètre du Func<object?, object?>
        ParameterExpression inputParam = Expression.Parameter(typeof(object), "input");
        ParameterExpression levelParam = Expression.Parameter(typeof(int).MakeByRefType(), "level");

        Type funcType = del.GetType();
        Type[] genericArgs = funcType.GetGenericArguments();
        Type argType = genericArgs[0];
        // Type resultType = genericArgs[1];

        UnaryExpression convertedArg = Expression.Convert(inputParam, argType);

        // Appel : del(arg)
        Expression call = target == null
            ? Expression.Call(method, convertedArg)
            : Expression.Call(target, method, convertedArg);
        UnaryExpression convertedResult = Expression.Convert(call, typeof(object));

        UnaryExpression incrementLevel = Expression.PostIncrementAssign(levelParam);
        var body = Expression.Block(incrementLevel, convertedResult);

        var exceptionVariable = Expression.Variable(typeof(Exception), "ex");
        var tryCatch = Expression.TryCatch(
                body,
                Expression.Catch(exceptionVariable,
                    Expression.Throw(
                        Expression.New(
                            typeof(ResolutionException).GetConstructor(new[] { typeof(int), typeof(string), typeof(Exception) })!,
                            levelParam,
                            Expression.Constant("Resolution fail"),
                            exceptionVariable
                        ),
                        typeof(object)
                    )
                )
            );
        // Conversion du résultat en object

        var lambda = Expression.Lambda<ChainableGetter>(tryCatch, inputParam, levelParam);
        return lambda.Compile();
    }

}

public sealed class ResolutionException : Exception
{
    public int Level { get; set; }
    public ResolutionException(int level, string? message) : base(message)
    {
        Level = level;
    }


    public ResolutionException(int level, string? message, Exception? innerException) : base(message, innerException)
    {
        Level = level;
    }
}

internal sealed class TryDelegateChain(Type initialType)
{
    private readonly Type initialType = initialType;
    private readonly List<LambdaInfo> _chain = [];
    private Type _currentType = initialType;
    private bool shouldResolve = true;

    public TryDelegateChain Fail()
    {
        shouldResolve = false;
        return this;
    }
    public TryDelegateChain Push(Delegate lambda)
    {
        LambdaInfo info = lambda.GetLambdaInfo();
        if (!info.InputType.IsAssignableFrom(_currentType))
        {
            throw new InvalidOperationException(
                $"Incompatibilité de type: la lambda attend {info.InputType.Name} " +
                $"mais le type actuel est {initialType.Name}");
        }

        _chain.Add(info);
        _currentType = info.ReturnType;

        return this;
    }

    public ChainedGetter Compile()
    {
        if (shouldResolve)
            return DelegateHelper.BuildResolver([.. _chain.Select(x => x.Delegate)]);
        else return (object? _, out object? value) =>
        {
            value = null;
            return false;
        };
    }

    // public Type GetCurrentReturnType()
    // {
    //     return _currentType;
    // }

    // public bool Execute(object? input, out object? value)
    // {
    //     object? result = input;
    //     try
    //     {
    //         if (shouldResolve)
    //         {
    //             int level = 0;
    //             foreach (LambdaInfo lambda in _chain)
    //             {
    //                 try
    //                 {
    //                     result = lambda.Delegate(result);
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     throw new ResolutionException(level, "Resolution fail", ex);
    //                 }
    //                 level++;
    //             }
    //         }
    //         value = result;
    //         return shouldResolve;
    //     }
    //     catch (ResolutionException rex)
    //     {
    //         value = null;
    //         return rex.Level > 0;
    //     }
    // }

    // public bool ExecuteAs<T>(object? input, [NotNullWhen(true)] out T? value)
    // {
    //     bool result = Execute(input, out object? obj);
    //     if (result && obj is T typed)
    //     {
    //         value = typed;
    //         return true;
    //     }
    //     value = default;
    //     return false;
    // }

    // public void PrintChainInfo()
    // {
    //     Console.WriteLine($"  Start: {initialType.Name}");
    //     for (int i = 0; i < _chain.Count; i++)
    //     {
    //         var info = _chain[i];
    //         Console.WriteLine($"  [{i + 1}] -> {info.ReturnType.Name}");
    //     }
    // }
}
