namespace Robin.Contracts.Variables;

public interface IVariableSegment
{
    TOut Accept<TOut, TArgs>(IVariableSegmentVisitor<TOut, TArgs> visitor, TArgs args);
};
