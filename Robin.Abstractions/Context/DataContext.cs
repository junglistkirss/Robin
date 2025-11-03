using System.Diagnostics.Contracts;

namespace Robin.Abstractions.Context;

public record class DataContext(object? Data, DataContext? Parent = null)
{
    [Pure]
    public DataContext Child(object? data) => new(data, this);
}



