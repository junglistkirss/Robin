using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using Robin.Extensions;
using System.Collections.Immutable;
using System.Text.Json;

namespace Robin.Benchmarks;

[Config(typeof(AutoBenchmarkConfig))]
[MemoryDiagnoser]
[MarkdownExporter]
public class TweetsBencnmarks
{
    private IServiceProvider serviceProvider = default!;
    private Tweet[] tweets = [];
    private ImmutableArray<INode> template = [];
    private IStringRenderer renderer = default!;

    [GlobalSetup]
    public void Setup()
    {

        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddStringRenderer()
            .AddMemberObjectAccessor<Tweet>(TweetAccessor.GetNamedProperty);

        serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = false,
            ValidateScopes = false,
        });
        string path = Path.Combine(AppContext.BaseDirectory, "datasets", "tweets.json");
        string json = File.ReadAllText(path);
        tweets = JsonSerializer.Deserialize<Tweet[]>(json)!;
        template = TweetsTemplates.List.AsSpan().Parse();
        renderer = serviceProvider.GetRequiredService<IStringRenderer>();

    }
    // [Benchmark]
    //     public INode[] ParseTweetsTemplate() => TweetsTemplates.List.AsSpan().Parse();

    [Benchmark(Baseline = true)]
    public string RenderSingleTweets() => renderer.Render(template, tweets[0]);

    [Benchmark]
    public string RenderEmptyTweets() => renderer.Render(template, Array.Empty<Tweet>());

    [Benchmark]
    public string RenderTake5TweetsAsArray() => renderer.Render(template, tweets.Take(5).ToArray());

    [Benchmark]
    public string RenderTake50Tweets() => renderer.Render(template, tweets.Take(50).ToArray());

    [Benchmark]
    public string RenderAllTweets() => renderer.Render(template, tweets);

    [Benchmark]
    public string RenderNullTweets() => renderer.Render(template, Enumerable.Repeat<Tweet>(null!, 1000).ToArray());

}
