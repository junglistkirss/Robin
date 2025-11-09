using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Robin.Benchmarks;

public class AutoBenchmarkConfig : ManualConfig
{
    public AutoBenchmarkConfig()
    {
        //WithOptions(ConfigOptions.DisableOptimizationsValidator);
        // Choisit la configuration adaptée
#if DEBUG
        // AddJob(Job.Dry);
#endif
        AddJob(Job.Default
            .WithWarmupCount(2)
            .WithIterationCount(8)
            .WithLaunchCount(1)
            );
#if WINDOWS
        //AddDiagnoser(new BenchmarkDotNet.Diagnostics.Windows.EtwProfiler());
#endif
        // AddDiagnoser(MemoryDiagnoser.Default);
        // AddExporter(MarkdownExporter.GitHub);
    }
}
