# Multinet.DMARC

A simple little project to parse DMARC aggregate reports.

## Benchmark
Benchmarking both with and without DNS lookups.
The DNS lookups are the slowest part of the process,
so it's worth seeing how much of a difference it makes.

We use DNS to get the PTR record for the IP address of the sending server.

Benchmark is made on a unzipped report with a size of 1 372 bytes.

```plain
BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a,
Windows 11 (10.0.22621.2428/22H2/2022Update/SunValley2)
13th Gen Intel Core i9-13900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 7.0.402
  [Host] : .NET 7.0.12 (7.0.1223.47720), X64 RyuJIT AVX2


| Method                      | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|---------------------------- |---------:|----------:|----------:|-------:|-------:|----------:|
| ParseReport_Benchmark_NoDNS | 8.742 μs | 0.1481 μs | 0.1818 μs | 1.2207 | 0.0305 |  22.64 KB |
| ParseReport_Benchmark_DNS   | 9.221 μs | 0.0883 μs | 0.0737 μs | 1.2817 | 0.0305 |  23.61 KB |
```