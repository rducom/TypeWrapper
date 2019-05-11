using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeWrapper.Tests.Mocks;

namespace TypeWrapper.Benchmark
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class TypeWrapperBenchmark
    {
        private List<RootInstance> _instances;
        private WrapBuilder<RootInstance> _builder;

        [GlobalSetup]
        public void Setup()
        {
            _instances = Enumerable.Range(0, N)
               .Select(i => new RootInstance
               {
                   Id = i,
                   Name = "Root" + i,
                   Kind = Guid.NewGuid().ToString(),
                   SubInstance = new SubInstance
                   {
                       Id = i,
                       SubName = "User" + i,
                       SubKind = Guid.NewGuid().ToString(),
                   }
               }).ToList();

            _builder = Wrap.Type<RootInstance>()
               .WithSourceProperties();

            // warmup
            _builder.Instance(new RootInstance());
        }

        [Params(10, 10000)]
        public int N;

        [Benchmark]
        public string SerializeNewtonsoft()
        {
            var data = _instances.AsEnumerable();
            return JsonConvert.SerializeObject(data);
        }

        [Benchmark(Baseline = true)]
        public string SerializeNative()
        {
            var data = _instances.AsEnumerable();
            return System.Text.Json.Serialization.JsonSerializer.ToString(data);
        }

        [Benchmark]
        public string SerializeWrappedNewtonsoft()
        {
            var data = _instances.Select(i => _builder.Instance(i));
            return JsonConvert.SerializeObject(data);
        }

        [Benchmark]
        public string SerializeWrappedNative()
        {
            var data = _instances.Select(i => _builder.Instance(i));
            return System.Text.Json.Serialization.JsonSerializer.ToString(data);
        }
    }
}