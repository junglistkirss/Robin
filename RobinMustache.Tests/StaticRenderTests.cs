using RobinMustache.Abstractions.Nodes;
using RobinMustache.Helpers;
using System.Collections.Immutable;

namespace RobinMustache.tests;

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

    [Fact]
    public void Test_Render_StandaloneTagRemoval()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "{{#.}}\r\nBonjour\r\n{{/.}}".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal("Bonjour", result);
    }
    [Fact]
    public void Test_Render_StandaloneTagRemoval2()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Hello\r\n{{^.}}\r\nDetails:\r\n  - Name: {{&Name}}\r\n  - Age: {{&Age}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal("Hello\r\nBye", result);
    }
    [Fact]
    public void Test_Render_StandaloneTagRemoval3()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Hello\r\n{{#.}}\r\nDetails:\r\n  - Name: {{&Name}}\r\n  - Age: {{&Age}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal("Hello\r\nDetails:\r\n  - Name: Alice\r\n  - Age: 30\r\nBye", result);
    }
}
