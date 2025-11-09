using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using Robin.Extensions;
using System.Collections.Immutable;

namespace Robin.Benchmarks;

[Config(typeof(AutoBenchmarkConfig))]
[MemoryDiagnoser]
[MarkdownExporter]
public class RenderBencnmarks
{
    private record class Data(int N);

    private IServiceProvider serviceProvider = default!;
    private Data[] data = [];
    private ImmutableArray<INode> template = [];
    private IStringRenderer renderer = default!;

    [GlobalSetup]
    public void Setup()
    {

        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddStringRenderer()
            .AddMemberObjectAccessor<Data>((Data obj, string member, out object? value) =>
            {
                value = member switch
                {
                    "N" => obj.N,
                    _ => null
                };
                return true;
            });

        serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = false,
            ValidateScopes = false,
        });
        string path = Path.Combine(AppContext.BaseDirectory, "datasets", "tweets.json");
        string json = File.ReadAllText(path);
        data = [.. Enumerable.Range(1, 1000).Select(x => new Data(x))];
        template = @"Bencmarck that !!
{{#N}}{{{.}}}
{{/N}}".AsSpan().Parse();
        renderer = serviceProvider.GetRequiredService<IStringRenderer>();

    }
    // [Benchmark]
    //     public INode[] ParseTweetsTemplate() => TweetsTemplates.List.AsSpan().Parse();

    [Benchmark(Baseline = true)]
    public string RenderSingle() => renderer.Render(template, data[0]);

    [Benchmark]
    public string RenderEmpty() => renderer.Render(template, Array.Empty<Tweet>());

    [Benchmark]
    public string RenderTake5AsArray() => renderer.Render(template, data.Take(5).ToArray());

    [Benchmark]
    public string RenderTake50() => renderer.Render(template, data.Take(50).ToArray());

    [Benchmark]
    public string RenderAll() => renderer.Render(template, data);

    [Benchmark]
    public string RenderNull() => renderer.Render(template, Enumerable.Repeat<Tweet>(null!, 1000).ToArray());

}
