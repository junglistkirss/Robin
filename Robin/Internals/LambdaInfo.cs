namespace Robin.Internals;

internal record LambdaInfo
{
    public required Type InputType { get; init; }
    public required Type ReturnType { get; init; }
    public required Delegate Delegate { get; init; }

    public override string ToString()
    {
        return $"{InputType.Name} -> {ReturnType.Name}";
    }
}
