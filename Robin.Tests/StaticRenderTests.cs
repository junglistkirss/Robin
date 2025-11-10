using Robin.Abstractions.Nodes;
using Robin.Helpers;
using System.Collections.Immutable;

namespace Robin.tests;

public class StaticRenderTests
{
    public IStringRenderer StringRenderer { get; private set; } = default!;

    public StaticRenderTests()
    {
        StringHelpers.AsGlobalHelpers();
        StringRenderer = Renderer.CreateStringRenderer(accessors: x =>
        {
            x.CreateMemberObjectAccessor<TestSample>(static (TestSample sample, string member, out object? value) =>
            {
                value = member switch
                {
                    "Name" => sample.Name,
                    "Age" => sample.Age,
                    _ => throw new InvalidDataException($"Member does not exists : {member}"),
                };
                return true;
            });
        });
    }

    [Fact]
    public void Test_Render_SimpleTemplate()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Name: {{ Name }}, Age: {{ Age }}".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal("Name: Alice, Age: 30", result);
    }
}
