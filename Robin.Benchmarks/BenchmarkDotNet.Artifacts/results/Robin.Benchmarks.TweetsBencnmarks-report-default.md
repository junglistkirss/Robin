
BenchmarkDotNet v0.15.5, Linux Ubuntu 24.04.3 LTS (Noble Numbat) (container)
13th Gen Intel Core i7-1370P 2.19GHz, 1 CPU, 20 logical and 10 physical cores
.NET SDK 9.0.305
  [Host] : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
  Debug  : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3

Job=Debug  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

 Method                     | Mean      | Error | Allocated |
--------------------------- |----------:|------:|----------:|
 RenderSingleTweets         | 11.332 ms |    NA |  59.63 KB |
 RenderManyTweets           | 12.343 ms |    NA |  191.6 KB |
 RenderEnumerableManyTweets |  3.833 ms |    NA |   1.48 KB |
 RenderEmptyTweets          |  3.978 ms |    NA |   1.98 KB |
 RenderTakeAllTweets        |  6.753 ms |    NA |   1.48 KB |
 RenderAllTweets            | 15.972 ms |    NA | 2543.9 KB |
