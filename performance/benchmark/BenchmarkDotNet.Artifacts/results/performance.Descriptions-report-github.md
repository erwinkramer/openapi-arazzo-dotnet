```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8655/25H2/2025Update/HudsonValley2)
Snapdragon X 12-core X1E80100 3.40 GHz (Max: 3.42GHz), 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a
  ShortRun : .NET 8.0.28 (8.0.28, 8.0.2826.26413), Arm64 RyuJIT armv8.0-a

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | Mean      | Error     | StdDev   | Gen0    | Allocated |
|--------------- |----------:|----------:|---------:|--------:|----------:|
| BnplYaml       |        NA |        NA |       NA |      NA |        NA |
| FormalBnplYaml | 335.51 μs | 53.773 μs | 2.947 μs | 92.2852 | 378.23 KB |
| FapiParYaml    | 257.98 μs | 28.064 μs | 1.538 μs | 85.9375 | 351.38 KB |
| MinimalJson    |  11.23 μs |  3.051 μs | 0.167 μs |  5.2338 |  21.38 KB |

Benchmarks with issues:
  Descriptions.BnplYaml: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
