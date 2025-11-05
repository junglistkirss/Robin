using Microsoft.Extensions.Caching.Memory;
using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Facades;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

namespace Robin.Abstractions;

internal static class LambdaExpressionHelper
{
    public static Type GetInputType(this Delegate lambda)
    {
        var parameters = lambda.Method.GetParameters();
        if (parameters.Length == 0)
            throw new ArgumentException("La lambda doit avoir au moins un paramètre");

        return parameters[0].ParameterType;
    }

    public static Type GetReturnType(this Delegate lambda)
    {
        return lambda.Method.ReturnType;
    }

    public static bool CanChain(this Delegate first, Delegate second)
    {
        var firstReturnType = GetReturnType(first);
        var secondInputType = GetInputType(second);

        return secondInputType.IsAssignableFrom(firstReturnType);
    }

    public static LambdaInfo GetLambdaInfo(this Delegate lambda)
    {
        return new LambdaInfo
        {
            InputType = GetInputType(lambda),
            ReturnType = GetReturnType(lambda),
            Delegate = lambda
        };
    }
}

internal record LambdaInfo
{
    public required Type InputType { get; init; }
    public required Type ReturnType { get; init; }
    public required Delegate Delegate { get; init; }

    public override string ToString()
    {
        return $"{InputType.Name} -> {ReturnType.Name}";
    }
}

internal sealed class TryDelegateChain(Type initialType)
{
    private readonly Type initialType = initialType;
    private readonly List<LambdaInfo> _chain = [];
    private Type _currentType = typeof(object);
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

    /// <summary>
    /// Obtient le type de retour actuel de la chaîne
    /// </summary>
    public Type GetCurrentReturnType()
    {
        return _currentType;
    }

    ///// <summary>
    ///// Obtient la liste des types dans la chaîne
    ///// </summary>
    //public List<string> GetTypeChain()
    //{
    //    var types = new List<string> { _initialValue.GetType().Name };
    //    types.AddRange(_chain.Select(info => info.ReturnType.Name));
    //    return types;
    //}

    /// <summary>
    /// Exécute toute la chaîne d'expressions
    /// </summary>
    public bool Execute(object? input, out object? value)
    {
        object? result = null;
        if (shouldResolve)
        {
            foreach (var lambda in _chain)
            {
                result = lambda.Delegate.DynamicInvoke(result);
            }
        }
        value = result;
        return shouldResolve;
    }

    /// <summary>
    /// Exécute et retourne le résultat typé
    /// </summary>
    public bool ExecuteAs<T>(object? input, [NotNullWhen(true)] out T? value)
    {
        bool result = Execute(input, out object? obj);
        if (result && obj is T typed)
        {
            value = typed;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Affiche les informations de la chaîne
    /// </summary>
    public void PrintChainInfo()
    {
        Console.WriteLine($"  Start: {initialType.Name}");
        for (int i = 0; i < _chain.Count; i++)
        {
            var info = _chain[i];
            Console.WriteLine($"  [{i + 1}] -> {info.ReturnType.Name}");
        }
    }
}

internal sealed class ExpressionNodeVisitor(IVariableSegmentVisitor<Type> accessorVisitor, IMemoryCache cache) : IExpressionNodeVisitor<DataContext>
{

    private record struct CacheKey(Type Type, VariablePath Path);

    public bool VisitFunctionCall(FunctionCallNode node, DataContext args, out object? value)
    {
        //if (args.Helper.TryGetFunction(node.FunctionName, out Helper.Function? function) && function is not null)
        //{
        //    object?[] evaluatedArgs = new object?[node.Arguments.Length];
        //    for (int i = 0; i < node.Arguments.Length; i++)
        //    {
        //        EvaluationResult evalResult = node.Arguments[i].Accept(this, args);
        //        if (evalResult.IsResolved)
        //        {
        //            evaluatedArgs[i] = evalResult.Value;
        //        }
        //        else
        //        {
        //            evaluatedArgs[i] = null;
        //        }
        //    }
        //    object? functionResult = function(evaluatedArgs);
        //    return new EvaluationResult(true, functionResult);
        //}
        value = null;
        return false;
    }

    public bool VisitIdenitifer(IdentifierExpressionNode node, DataContext args, out object? value)
    {
        object? current = args.Data;
        if (current is null)
        {
            value = null;
            return false;
        }
        Type type = current.GetType();
#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
        TryDelegateChain @delegate = cache.GetOrCreate(new CacheKey(type, node.Path), (key) =>
        {
            CacheKey cacheKey = (CacheKey)key.Key;
            Type currentType = cacheKey.Type;
            VariablePath path = cacheKey.Path;

            TryDelegateChain chain = new(currentType);
            int limit = cacheKey.Path.Segments.Length;
            if (limit == 0)
                chain.Fail();
            else
            {
                int i = 0;
                IVariableSegment current = path.Segments[i];
                bool resolved = current.Accept(accessorVisitor, currentType, out Delegate @delegate);
                i++;
                while (resolved && i < limit)
                {
                    currentType = @delegate.GetReturnType();
                    current = path.Segments[i]; ;
                    resolved = current.Accept(accessorVisitor, currentType, out @delegate);
                    if (!resolved)
                    {
                        // avoid precedence
                        chain.Fail();
                        break;
                    }
                    else
                    {
                        chain.Push(@delegate);
                    }
                    i++;
                }
            }
            return chain;
        });
#pragma warning restore CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
        bool resolved = @delegate!.Execute(current, out value);
        if (resolved)
            return true;
        if (args.Parent is not null)
            return VisitIdenitifer(node, args.Parent, out value);

        value = null;
        return false;
    }

    public bool VisitLiteral(LiteralExpressionNode node, DataContext _, out object? value)
    {
        value = node.Constant;
        return true;
    }

    public bool VisitIndex(IndexExpressionNode node, DataContext _, out object? value)
    {
        value = node.Constant;
        return true;
    }
}
