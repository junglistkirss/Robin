using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text.Json;

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
            .AddMemberAccessor<Tweet>(TweetAccessor.TryGetPropertyValue)
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
    public void Test_Render_Tweets()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "datasets", "tweets.json");
        string json = File.ReadAllText(path);
        var tweets = JsonSerializer.Deserialize<Tweet[]>(json)!;
        IStringRenderer renderer = ServiceProvider.GetRequiredService<IStringRenderer>();
        ImmutableArray<INode> template = TweetsTemplates.List.AsSpan().Parse();
        string result = renderer.Render(template, tweets);
        Assert.NotEmpty(result);
    }

}
