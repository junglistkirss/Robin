using Robin.Abstractions.Helpers;
using System.Diagnostics.Contracts;

namespace Robin.Abstractions.Context;

public record class DataContext(object? Current, DataContext? Parent = null)
{
    public readonly static DataContext Empty = new(null, null); 

    [Pure]
    public DataContext Child(object? data) => new(data, this);
}



