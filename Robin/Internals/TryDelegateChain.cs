using System.Diagnostics.CodeAnalysis;

namespace Robin.Internals;

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
        object? result = input;
        try
        {
            if (shouldResolve)
            {
                int level = 0;
                foreach (var lambda in _chain)
                {
                    try
                    {
                        result = lambda.Delegate.DynamicInvoke(result);
                    }
                    catch (Exception ex)
                    {
                        throw new ResolutionException(level, "Resolution fail", ex);
                    }
                    level++;
                }
            }
            value = result;
            return shouldResolve;
        }
        catch (ResolutionException rex)
        {
            value = null;
            return rex.Level > 0;
        }
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
