using Microsoft.Extensions.Caching.Memory;
using Robin.Abstractions.Context;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Internals;

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
        if (node.Path.Segments.Length == 0)
        {
            value = null;
            return false;
        }

        Type type = current.GetType();
        TryDelegateChain? @delegate = cache.GetOrCreate(new CacheKey(type, node.Path), (entry) =>
        {
            CacheKey cacheKey = (CacheKey)entry.Key;
            Type currentType = cacheKey.Type;
            VariablePath path = cacheKey.Path;

            TryDelegateChain chain = new(currentType);
            int limit = cacheKey.Path.Segments.Length;
            int i = 0;
            IVariableSegment current = path.Segments[i];
            bool resolved = current.Accept(accessorVisitor, currentType, out Delegate @delegate);
            if (!resolved)
                return chain.Fail();
            chain.Push(@delegate);
            i++;
            while (resolved && i < limit)
            {
                currentType = @delegate.GetReturnType();
                current = path.Segments[i]; ;
                resolved = current.Accept(accessorVisitor, currentType, out @delegate);
                if (!resolved) // avoid precedence
                    return chain.Fail();
                chain.Push(@delegate);
                i++;
            }

            return chain;
        });
        if (@delegate is not null)
        {
            bool resolved = @delegate!.Execute(current, out value);
            if (resolved)
                return true;
        }
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
