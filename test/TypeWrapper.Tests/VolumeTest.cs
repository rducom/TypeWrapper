using System.Diagnostics;
using System.Linq;
using TypeWrapper.Tests.Mocks;
using Xunit;

namespace TypeWrapper.Tests
{
    public class VolumeTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100000)]
        public void MoreWrappers(int number)
        {
            var builder = Wrap.Type<User>()
                .WithSourceProperties();

            // warmup
            builder.Instance(new User());

            Stopwatch sw = Stopwatch.StartNew();
            var users = Enumerable.Range(0, number).Select(i =>  new User { Id = i, Name = "User"+i, Kind = "Some strings" }).ToList();
            sw.Stop();

            Stopwatch sww = Stopwatch.StartNew();
            var wrapped  = users.Select(u => builder.Instance(u)).ToList();
            sww.Stop();
            Assert.True(sww.ElapsedTicks < 3 * sw.ElapsedTicks, $"wrap : {sww.ElapsedTicks} initial: {sw.ElapsedTicks}");
        }
    }
}
