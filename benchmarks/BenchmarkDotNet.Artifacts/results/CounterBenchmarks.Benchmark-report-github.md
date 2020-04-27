``` ini

BenchmarkDotNet=v0.12.1, OS=ubuntu 18.04
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.102
  [Host]     : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT
  DefaultJob : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT


```
|            Method |     Mean |   Error |  StdDev |
|------------------ |---------:|--------:|--------:|
| WriteEventCounter | 335.5 μs | 1.73 μs | 1.62 μs |
