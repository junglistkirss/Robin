using System.Diagnostics.Contracts;

namespace Robin.Abstractions;

public record class DataContext(object? Data, DataContext? Parent = null)
{
    [Pure]
    public DataContext Child(object? data) => new(data, this);
}



