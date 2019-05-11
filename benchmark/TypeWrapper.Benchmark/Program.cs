using BenchmarkDotNet.Running;
using System;

namespace TypeWrapper.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TypeWrapperBenchmark>();
        }
    }
}
