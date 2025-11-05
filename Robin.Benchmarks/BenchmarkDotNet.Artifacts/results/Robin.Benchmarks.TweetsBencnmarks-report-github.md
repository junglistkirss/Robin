```

BenchmarkDotNet v0.15.5, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
Intel Core i5-8350U CPU 1.70GHz (Max: 1.90GHz) (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK 9.0.306
  [Host] : .NET 9.0.10 (9.0.10, 9.0.1025.47515), X64 RyuJIT x86-64-v3
  Fast   : .NET 9.0.10 (9.0.10, 9.0.1025.47515), X64 RyuJIT x86-64-v3

Job=Fast  IterationCount=8  LaunchCount=1  
WarmupCount=2  

```
| Method       | Mean     | Error    | StdDev   | Gen0      | Gen1      | Gen2     | Allocated |
|------------- |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
| RenderTweets | 31.25 ms | 16.02 ms | 8.380 ms | 1281.2500 | 1031.2500 | 968.7500 | 188.61 MB |
