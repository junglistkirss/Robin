using System.Diagnostics.Contracts;

namespace Robin.Contracts.Context;

public record class DataContext(object? Data, DataContext? Previous = null)
{
    [Pure]
    public DataContext Child(object? data) => new(data, this);
}



