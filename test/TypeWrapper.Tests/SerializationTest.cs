using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeWrapper.Tests.Mocks;
using Xunit;

namespace TypeWrapper.Tests
{
    public class SerializationTest
    {
        [Fact]
        public void IsoCheck()
        {
            var instances = Enumerable.Range(0, 10)
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

            var builder = Wrap.Type<RootInstance>()
               .WithSourceProperties();

            var native = JsonConvert.SerializeObject(instances); 
            var wrapped = JsonConvert.SerializeObject(instances.Select(i => builder.Instance(i)));

            Assert.Equal(native, wrapped);
        }
    }
}
