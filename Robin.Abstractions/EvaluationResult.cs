using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Robin.Abstractions.Facades;

namespace Robin.Abstractions;

public record struct  EvaluationResult(bool IsResolved, object? Value);

