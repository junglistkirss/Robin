using Microsoft.Extensions.DependencyInjection;
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
            .AddMemberAccessor<TestSample>(static (string member, out Delegate value) =>
            {
                value = member switch
                {
                    "Name" => (Func<TestSample, string?>)(x => x.Name),
                    "Age" => (Func<TestSample, int>)(x => x.Age),
                    _ => throw new InvalidDataException("Member does not exists"),
                };
                return true;
            })
            .AddMemberAccessor<ParentTestSample>(static (string member, out Delegate value) =>
            {
                value = member.ToLowerInvariant() switch
                {
                    "alias" => (Func<ParentTestSample, string?>)(x => x.Alias),
                    "nested" => (Func<ParentTestSample, TestSample?>)(x => x.Nested),
                    _ => throw new InvalidDataException("Member does not exists"),
                };
                return true;
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
    public void ParentTest_Render_SimpleTemplate()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        var sample = new TestSample { Name = "Alice", Age = 30 };
        var parent = new ParentTestSample { Alias = "Bob", Nested = sample };
        ImmutableArray<INode> template = "Name: {{ Alias }}, Nested: {{ nested.Name }}".AsSpan().Parse();
        string result = renderer.Render(template, parent);
        Assert.Equal("Name: Bob, Nested: Alice", result);
    }

    [Fact]
    public void ParentTest_Render_SectionTemplate()
    {
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();

        ImmutableArray<INode> template = "{{#.}}Name: {{ Alias }}, {{# nested }}Nested: {{ Name }}{{/ nested }}{{/.}}".AsSpan().Parse();
        string result = renderer.Render(template, Enumerable.Range(0, 100).Select(i => new ParentTestSample { Alias = "Bob", Nested = new TestSample { Name = "Alice", Age = i } }).ToArray());
        Assert.Contains("Name: Bob, Nested: Alice", result);
    }
}
