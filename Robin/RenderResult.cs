using Robin.Contracts.Variables;

namespace Robin;

public record struct RenderResult(bool IsComplete, Exception? Exception)
{
    public readonly static RenderResult Complete = new RenderResult(true, null);
    public static RenderResult Fail(Exception? exception = null) => new RenderResult(false, exception);
};

