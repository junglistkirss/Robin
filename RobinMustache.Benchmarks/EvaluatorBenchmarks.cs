using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using Microsoft.Extensions.DependencyInjection;
using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Extensions;
using RobinMustache.Abstractions.Facades;
using RobinMustache.Abstractions.Variables;
using RobinMustache.Extensions;
using System.Text.Json;

namespace RobinMustache.Benchmarks;

[Config(typeof(AutoBenchmarkConfig))]
[MemoryDiagnoser]
[MarkdownExporter]
public class EvaluatorBenchmarks
{
    private IServiceProvider serviceProvider = default!;
    private Tweet[] tweets = [];
    private IEvaluator evaluator = default!;

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
        evaluator = serviceProvider.GetRequiredService<IEvaluator>();

    }

    [Benchmark(Baseline = true)]
    public void ResolveEntireCollection()
    {
        IdentifierExpressionNode node = new(new VariablePath([ThisSegment.Instance]));
        using (DataContext.Push(tweets))
            evaluator.Resolve(node, DataContext.Current, out IDataFacade facade);
    }
    [Benchmark]
    public void ResolveUniqueItem()
    {
        int i = 0;
        IdentifierExpressionNode node = new($"[{i}]".Parse());
        using (DataContext.Push(tweets))
            evaluator.Resolve(node, DataContext.Current, out IDataFacade facade);
    }

    [Benchmark]
    public void ResolveItems()
    {
        int i = 0;
        using (DataContext.Push(tweets))
            while (i < 100)
            {
                IdentifierExpressionNode node = new($"[{i}]".Parse());
                evaluator.Resolve(node, DataContext.Current, out IDataFacade facade);
                i++;
            }
    }

    [Benchmark]
    public void ResolveItemsManyTime()
    {
        int j = 0;
        using (DataContext.Push(tweets))
            while (j < 10)
            {
                int i = 0;
                while (i < 100)
                {
                    IdentifierExpressionNode node = new($"[{i}]".Parse());
                    evaluator.Resolve(node, DataContext.Current, out IDataFacade facade);
                    i++;
                }
                j++;
            }
    }


    [Benchmark]
    public void ResolveItemsValue()
    {
        int i = 0;
        using (DataContext.Push(tweets))
            while (i < 100)
            {
                IdentifierExpressionNode node = new($"[{i}].content".Parse());
                evaluator.Resolve(node, DataContext.Current, out IDataFacade facade);
                i++;
            }
    }

    [Benchmark]
    public void ResolveItemsValueManyTimes()
    {
        int j = 0;
        using (DataContext.Push(tweets))
            while (j < 10)
            {
                int i = 0;
                while (i < 100)
                {
                    IdentifierExpressionNode node = new($"[{i}].content".Parse());
                    evaluator.Resolve(node, DataContext.Current, out IDataFacade facade);
                    i++;
                }
                j++;
            }
    }

    [Benchmark]
    public void ResolveUniqueItemValue()
    {

        int i = 0;
        IdentifierExpressionNode node = new($"[{i}].content".Parse());
        using (DataContext.Push(tweets))
            evaluator.Resolve(node, DataContext.Current, out IDataFacade facade);
    }
}
