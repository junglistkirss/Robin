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
                    _ => (Func<TestSample, object?>)(_ => null),
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

}
