# Multinet.DMARC

A simple little project to parse DMARC aggregate reports.


## Multinet.DMARC.AggregateAnalyzer

This project is a library that lets you parse DMARC aggregate reports.

### Benchmark
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

## Multinet.DMARC.ReportCollector

This project is a service/console application which hosts a SMTP server,
that will collect any incoming emails, and parse the ones containing DMARC reports.

They will be stored in a database (configurable), for further analysis.

### Configuration


#### DNS Records

To be able to receive DMARC reports,
you need to add some DNS records to your domain of choice.

In this example, we will use `dmarc-reports.example.com`
as the domain we want to be able to receive reports to,
so that emails to `*@dmarc-reports.example.com` will be received.

So, we need to add the following DNS records to be able to receive the mail:

```plain
*._report._dmarc.dmarc-reports.example.com. 60 IN TXT "v=DMARC1"
dmarc-reports.example.com. 60 IN A <IP address of the server>
dmarc-reports.example.com. 60 IN MX 10 dmarc-reports.example.com.
```

The `*._report._dmarc` record is used to tell the sending server
that we want to receive reports, in this case for all domains.

Best practices would be to only receive reports for the domains you want to,
and then you need to change the `*` to the domain you want to receive reports for.

Like this:

```plain
example.com._report._dmarc.dmarc-reports.example.com. 60 IN TXT "v=DMARC1"
```

#### Database

Set the config for the database in the `appsettings.json`-file.

Example for Sqlite:

```json
"Database": {
  "Type": "Sqlite",
  "ConnectionString": "Data Source=reports.db"
}
```

## Multinet.DMARC.ReportViewer

This project is a web application that will let you view the parsed DMARC reports.
