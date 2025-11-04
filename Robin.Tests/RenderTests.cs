using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.tests;

public class RenderTests
{
    public IServiceProvider ServiceProvider { get; private set; } = default!;

    public RenderTests()
    {
        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddStringRenderer()
            .AddMemberAccessor(static (TestSample? obj, string member, out object? value) =>
            {
                value = member switch
                {
                    "Name" => obj!.Name,
                    "Age" => obj!.Age,
                    _ => null,
                };
                return value is not null;
            });
        ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }

    [Fact]
    public void Test_Render_SimpleTemplate()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        var sample = new TestSample { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Name: {{ Name }}, Age: {{ Age }}".AsSpan().Parse();
        string result = renderer.Render(template, sample);
        Assert.Equal("Name: Alice, Age: 30", result);
    }

    [Fact]
    public void Test_Render_SimpleTemplateList()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        TestSample[] sample = [.. Enumerable.Range(0, 100).Select(i => new TestSample { Name = "Alice", Age = i +1 })];
        ImmutableArray<INode> template = "{{#.}}Name: {{ Name }}, Age: {{ Age }}{{/.}}".AsSpan().Parse();
        string result = renderer.Render(template, sample);
        Assert.Contains("Name: Alice, Age: 1", result);
        Assert.Contains("Name: Alice, Age: 100", result);
    }
}
