namespace RobinMustache.Abstractions.Accessors;

public delegate bool ChainableGetter(object? input, out object? value);

public static class ChainableGetters
{
    public static readonly ChainableGetter ReturnNull = new((object? _, out object? value) =>
    {

        value = null;
        return false;
    });
}
