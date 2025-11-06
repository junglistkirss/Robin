namespace Robin.Internals;

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
        var firstReturnType = first.GetReturnType();
        var secondInputType = second.GetInputType();

        return secondInputType.IsAssignableFrom(firstReturnType);
    }

    public static LambdaInfo GetLambdaInfo(this Delegate lambda)
    {
        return new LambdaInfo
        {
            InputType = lambda.GetInputType(),
            ReturnType = lambda.GetReturnType(),
            Delegate = lambda
        };
    }
}
