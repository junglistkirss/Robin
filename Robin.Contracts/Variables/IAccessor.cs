namespace Robin.Contracts.Variables;

public interface IAccessor
{
    TOut Accept<TOut, TArgs>(IAccessorVisitor<TOut, TArgs> visitor, TArgs args);
};
