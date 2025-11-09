namespace Robin.Contracts.Variables;

public interface IVariableSegment
{
    bool Accept<TArgs, TOut>(IVariableSegmentVisitor<TArgs, TOut> visitor, TArgs args, out TOut result);
};
