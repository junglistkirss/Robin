using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text.Json;

namespace Robin.Benchmarks;

[Config(typeof(AutoBenchmarkConfig))]
[MemoryDiagnoser]
[MarkdownExporter]

public class TweetsBencnmarks
{
    private readonly IServiceProvider serviceProvider;
    private readonly Tweet[] tweets;
    private readonly ImmutableArray<INode> template;
    private readonly IStringRenderer renderer;
    public TweetsBencnmarks()
    {

        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddStringRenderer()
            .AddMemberAccessor<Tweet>(TweetAccessor.GetPropertyDelegate);
        foreach (var descriptor in services)
        {
            if (descriptor == null)
                throw new InvalidOperationException("Un service descriptor est null");

            if (descriptor.ImplementationInstance is Delegate del && del.Target == null)
                Console.WriteLine($"⚠️ Delegate sans cible : {del.Method.Name}");

            if (descriptor.ImplementationFactory == null &&
                descriptor.ImplementationInstance == null &&
                descriptor.ImplementationType == null)
                Console.WriteLine($"⚠️ ServiceDescriptor incomplet : {descriptor.ServiceType}");
        }
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


    [Benchmark]
    public string RenderSingleTweets() => renderer.Render(template, tweets[0]);

    [Benchmark]
    public string RenderTake5TweetsAsArray() => renderer.Render(template, tweets.Take(5).ToArray());

    [Benchmark]
    public string RenderTake50Tweets() => renderer.Render(template, tweets.Take(50));

    [Benchmark]
    public string RenderEmptyTweets() => renderer.Render(template, Array.Empty<Tweet>());

    [Benchmark]
    public string RenderEnumerableTweets() => renderer.Render(template, Enumerable.Empty<Tweet>());

    [Benchmark]
    public string RenderTakeAllTweets() => renderer.Render(template, tweets.Take(100));

    [Benchmark]
    public string RenderAllTweets() => renderer.Render(template, tweets);


}
