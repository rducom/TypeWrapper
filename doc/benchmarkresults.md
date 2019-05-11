``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.475 (1809/October2018Update/Redstone5)
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview5-011568
  [Host] : .NET Core 3.0.0-preview5-27626-15 (CoreCLR 4.6.27622.75, CoreFX 4.700.19.22408), 64bit RyuJIT
  Core   : .NET Core 3.0.0-preview5-27626-15 (CoreCLR 4.6.27622.75, CoreFX 4.700.19.22408), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                     Method |     N |         Mean |       Error |      StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
|--------------------------- |------ |-------------:|------------:|------------:|------:|--------:|----------:|---------:|---------:|-----------:|
|        **SerializeNewtonsoft** |    **10** |     **20.66 us** |   **0.2253 us** |   **0.2108 us** |  **1.80** |    **0.03** |    **2.8381** |        **-** |        **-** |    **8.77 KB** |
|            SerializeNative |    10 |     11.45 us |   0.1235 us |   0.1155 us |  1.00 |    0.00 |    1.1444 |        - |        - |    3.54 KB |
| SerializeWrappedNewtonsoft |    10 |     20.87 us |   0.2383 us |   0.2229 us |  1.82 |    0.03 |    2.9907 |        - |        - |    9.18 KB |
|     SerializeWrappedNative |    10 |     13.66 us |   0.0921 us |   0.0862 us |  1.19 |    0.02 |    1.3885 |        - |        - |    4.28 KB |
|                            |       |              |             |             |       |         |           |          |          |            |
|        **SerializeNewtonsoft** | **10000** | **23,405.62 us** | **201.9948 us** | **188.9460 us** |  **1.75** |    **0.06** |  **968.7500** | **718.7500** | **343.7500** |  **7195.7 KB** |
|            SerializeNative | 10000 | 13,395.96 us | 264.7600 us | 396.2803 us |  1.00 |    0.00 |  453.1250 | 453.1250 | 453.1250 | 5399.71 KB |
| SerializeWrappedNewtonsoft | 10000 | 23,864.86 us | 275.8272 us | 258.0089 us |  1.79 |    0.07 | 1000.0000 | 656.2500 | 312.5000 |  7509.1 KB |
|     SerializeWrappedNative | 10000 | 15,868.92 us | 133.6218 us | 124.9899 us |  1.19 |    0.04 |  562.5000 | 468.7500 | 468.7500 |  6045.1 KB |
