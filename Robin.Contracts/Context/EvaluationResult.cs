namespace Robin.Contracts.Context;

public enum ResoltionState
{
    Found,
    NotFound,
    Partial,
}

public record EvaluationResult(ResoltionState Status, object? Value);
