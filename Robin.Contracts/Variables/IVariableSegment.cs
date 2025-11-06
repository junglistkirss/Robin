namespace Robin.Contracts.Variables;

public interface IVariableSegment
{
    bool Accept<TArgs>(IVariableSegmentVisitor<TArgs> visitor, TArgs args, out Delegate @delegate);
};
