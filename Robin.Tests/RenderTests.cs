using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using Robin.Nodes;
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
        string inline = "Name: {{ Name }}, Age: {{ Age }}";
        NodeLexer lexer = new(inline);
        ImmutableArray<INode> nodes = lexer.Parse();
        string result = renderer.Render(nodes, inline, sample);
        Assert.Equal("Name: Alice, Age: 30", result);
    }

}
