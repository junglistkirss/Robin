namespace Robin.Abstractions;

public interface IRenderContextProvider
{
    RenderContext GetRenderContext(object? data);
}

// public interface IRenderContextProvider<T> : IRenderContextProvider
// {
//     RenderContext<T> GetRenderContext(T? data);
//     RenderContext IRenderContextProvider.GetRenderContext(object? data) =>
//         data is T typed ? GetRenderContext(typed) : throw new InvalidOperationException($"Invalid data type. Expected {typeof(T)}");
// }