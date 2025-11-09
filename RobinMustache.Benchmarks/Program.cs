// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using RobinMustache.Benchmarks;


Type[] benchmarks =
[
    typeof(EvaluatorBenchmarks),
    typeof(RenderBencnmarks),
    typeof(TweetsBencnmarks)
];

BenchmarkSwitcher.FromTypes(benchmarks).Run(args);