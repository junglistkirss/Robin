using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public interface IEvaluator
{
    IDataFacade Resolve(IExpressionNode expression, DataContext? data);
}
public interface ITypedAccessor
{
    EvaluationResult GetProperty(object? source, string name);
}
public interface ITypedAccessor<T> : ITypedAccessor
{
    EvaluationResult ITypedAccessor.GetProperty(object? source, string name)
    {
        if (source is T typed)
            return GetProperty(typed, name);
        return new EvaluationResult(ResoltionState.NotFound, DataFacade.Null);
    }
    EvaluationResult GetProperty(T? source, string name);
}
/*
internal sealed class ServiceAccesorVisitor(IServiceProvider serviceProvider) : IAccessorVisitor<EvaluationResult, DataContext>
{
    public EvaluationResult VisitIndex(IndexAccessor accessor, DataContext context)
    {
        if (context.Data is IEnumerable<KeyAccessor> enumerable)
        {
            return new(ResoltionState.Found, enumerable[accessor.Index].AsFacade());
        }
        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitKey(KeyAccessor accessor, DataContext context)
    {
        EvaluationResult resolvedKey = accessor.Key.Evaluate(this, context);

        if (resolvedKey.Status == ResoltionState.NotFound && context.Parent is not null)
            resolvedKey = accessor.Key.Evaluate(this, context.Parent);

        if (resolvedKey.Status == ResoltionState.Found)
        {
            string key = resolvedKey.Value.RawValue?.ToString() ?? string.Empty;

            if (context.Data is not null)
            {
                return new(ResoltionState.Found, keyNode.AsJsonFacade());
            }
            else if (context.Parent?.Data is not null && prevJson.TryGetPropertyValue(key, out JsonNode? prevKeyNode))
            {
                return new(ResoltionState.Found, prevKeyNode.AsJsonFacade());
            }
        }
        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitMember(MemberAccessor accessor, DataContext context)
    {
        if (context.Data is not null)
        {
            Type type = typeof(ITypedAccessor<>).MakeGenericType(context.Data.GetType());
            ITypedAccessor typeAccessor = (ITypedAccessor)serviceProvider.GetRequiredService(type);
            return typeAccessor.GetProperty(context.Data, accessor.MemberName);
        }
        
        // if (context.Parent?.Data is JsonObject jsonPrev && jsonPrev.TryGetPropertyValue(accessor.MemberName, out JsonNode? nodePrev))
        //     return new(ResoltionState.Found, nodePrev.AsJsonFacade());

        return new(ResoltionState.NotFound, DataFacade.Null);
    }

    public EvaluationResult VisitParent(ParentAccessor accessor, DataContext context)
    {
        DataContext parent = context;
        while (parent.Parent is not null)
        {
            parent = parent.Parent;
        }
        return new(ResoltionState.Found, parent.Data.AsFacade());
    }

    public EvaluationResult VisitThis(ThisAccessor accessor, DataContext context)
    {
        return new(ResoltionState.Found, context.Data.AsFacade());
    }
}*/
/*internal sealed class ServiceEvaluator(ServiceAccesorVisitor accesorVisitor) : IEvaluator
{
    public IDataFacade Resolve(IExpressionNode expression, DataContext? data)
    {
        var visitor = new ExpressionNodeVisitor(accesorVisitor);
        if (data is null)
            return DataFacade.Null;

        EvaluationResult result = expression.Accept(visitor, data);

        if (result.Status == ResoltionState.NotFound && data.Parent is not null)
            result = expression.Accept(visitor, data.Parent);

        if (result.Status == ResoltionState.Found)
            return result.Value;

        return DataFacade.Null;
    }
}*/

