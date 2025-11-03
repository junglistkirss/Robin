using Robin.Abstractions.Helpers;
using System.Diagnostics.Contracts;

namespace Robin.Abstractions.Context;

public record class DataContext(object? Data, DataContext? Parent = null)
{
    public Helper Helper { get; } = new();

    [Pure]
    public DataContext Child(object? data) => new(data, this);
}



