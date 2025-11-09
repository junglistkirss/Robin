using System.Linq.Expressions;
using System.Reflection;

namespace Robin.Abstractions.Accessors;

public static class DelegateHelper
{
    public static ChainableGetter AsChainable(this Delegate @delegate)
    {
        if(@delegate is null)
        throw new ArgumentNullException(nameof(@delegate));

        MethodInfo method = @delegate.Method;
        ConstantExpression? target = @delegate.Target == null ? null : Expression.Constant(@delegate.Target);

        ParameterExpression inputParam = Expression.Parameter(typeof(object), "input");
        ParameterExpression outputParam = Expression.Parameter(typeof(object).MakeByRefType(), "value");
        LabelTarget returnLabel = Expression.Label(typeof(bool), "returnLabel");

        Type funcType = @delegate.GetType();
        Type[] genericArgs = funcType.GetGenericArguments();
        Type argType = genericArgs[0];

        UnaryExpression convertedArg = Expression.Convert(inputParam, argType);

        Expression call = target == null
            ? Expression.Call(method, convertedArg)
            : Expression.Call(target, method, convertedArg);
        UnaryExpression convertedResult = Expression.Convert(call, typeof(object));
        BinaryExpression assignOutput = Expression.Assign(outputParam, convertedResult);
        LabelExpression returnTrue = Expression.Label(returnLabel, Expression.Constant(true));

        BlockExpression body = Expression.Block(assignOutput, returnTrue);

        Expression<ChainableGetter> lambda = Expression.Lambda<ChainableGetter>(body, inputParam, outputParam);

        return lambda.Compile();
    }

}
