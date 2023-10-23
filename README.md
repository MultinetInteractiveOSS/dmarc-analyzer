# Multinet.DMARC

A simple little project to parse DMARC aggregate reports.

## Benchmark
_(It's only using an XML serializer, so there's not much to benchmark)_

```plain
BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a,
Windows 11 (10.0.22621.2428/22H2/2022Update/SunValley2)
13th Gen Intel Core i9-13900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 7.0.402
  [Host] : .NET 7.0.12 (7.0.1223.47720), X64 RyuJIT AVX2

```

| Method                | Mean     | Error    | StdDev   | Gen0   | Gen1   | Allocated |
|---------------------- |---------:|---------:|---------:|-------:|-------:|----------:|
| ParseReport_Benchmark | 11.54 μs | 0.043 μs | 0.038 μs | 1.1749 | 0.0610 |  21.84 KB |