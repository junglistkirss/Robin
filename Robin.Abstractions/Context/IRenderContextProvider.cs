namespace Robin.Abstractions.Context;

public interface IRenderContextProvider
{
    RenderContext GetRenderContext(object? data);
}
