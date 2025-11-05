using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text.Json;

namespace Robin.Benchmarks;

/// <summary>
/// Configuration automatique : choisit Fast ou Debug selon une variable d'environnement ou un argument CLI.
/// </summary>
public class AutoBenchmarkConfig : ManualConfig
{
    private const string DEBUG_MODE = "Debug";
    private const string FAST_MODE = "Fast";
    private const string ENV_MODE = "BENCH_MODE";

    public AutoBenchmarkConfig()
    {
        // Vérifie d'abord une variable d'environnement (par ex. BENCH_MODE=Debug)
        string? mode = Environment.GetEnvironmentVariable(ENV_MODE);

        // Sinon, vérifie les arguments de ligne de commande
        string[] args = Environment.GetCommandLineArgs();
        if (mode is null && args.Length > 1)
        {
            foreach (string arg in args)
            {
                if (arg.Equals("--debug", StringComparison.OrdinalIgnoreCase))
                    mode = DEBUG_MODE;
                else if (arg.Equals("--fast", StringComparison.OrdinalIgnoreCase))
                    mode = FAST_MODE;
            }
        }

        // Choisit la configuration adaptée
        if (string.Equals(mode, DEBUG_MODE, StringComparison.OrdinalIgnoreCase))
        {
            AddJob(Job.Dry
                .WithWarmupCount(1)
                .WithIterationCount(3)
                .WithLaunchCount(1)
                .WithId(DEBUG_MODE));
        }
        else
        {
            AddJob(Job.Default
                .WithWarmupCount(2)
                .WithIterationCount(8)
                .WithLaunchCount(1)
                .WithId(FAST_MODE));
        }
#if WINDOWS
        AddDiagnoser(new BenchmarkDotNet.Diagnostics.Windows.EtwProfiler());
#endif
        // AddDiagnoser(MemoryDiagnoser.Default);
        // AddExporter(MarkdownExporter.GitHub);
    }
}

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
            .AddMemberAccessor<Tweet>(TweetAccessor.TryGetPropertyValue);
        serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
        string path = Path.Combine(AppContext.BaseDirectory, "datasets", "tweets.json");
        string json = File.ReadAllText(path);
        tweets = JsonSerializer.Deserialize<Tweet[]>(json)!;
        template = TweetsTemplates.List.AsSpan().Parse();
        renderer = serviceProvider.GetRequiredService<IStringRenderer>();

    }

    [Benchmark]
    public string RenderTweets() => renderer.Render(template, TweetsTemplates.List, tweets);

}
