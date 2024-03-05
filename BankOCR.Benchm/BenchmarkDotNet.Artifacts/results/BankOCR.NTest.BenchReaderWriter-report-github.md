```

BenchmarkDotNet v0.13.12, macOS Big Sur 11.7.8 (20G1351) [Darwin 20.6.0]
Intel Core i7-4770HQ CPU 2.20GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.200
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2


```
| Method   | Mean     | Error   | StdDev  | Gen0     | Gen1     | Gen2   | Allocated |
|--------- |---------:|--------:|--------:|---------:|---------:|-------:|----------:|
| ReadFile | 751.2 μs | 6.28 μs | 5.25 μs | 336.9141 | 162.1094 | 0.9766 |   1.08 MB |
