using Xunit;

namespace TypeWrapper.Tests
{
    public class PropertyTest
    {
        [Fact]
        public void PropertyExists()
        {
            var user = new User { Id = 42, Name = "Ben", Kind = "Allocation annihilator" };
            var wrapped = Wrap.Instance(user);

            var wrappedType = wrapped.GetType(); 

            var id = (int)wrappedType.GetProperty("Id").GetValue(wrapped);
            var name = (string)wrappedType.GetProperty("Name").GetValue(wrapped);
            var kind = (string)wrappedType.GetProperty("Kind").GetValue(wrapped);

            Assert.Equal(user.Id, id);
            Assert.Equal(user.Name, name);
            Assert.Equal(user.Kind, kind);
        }
    }
}
