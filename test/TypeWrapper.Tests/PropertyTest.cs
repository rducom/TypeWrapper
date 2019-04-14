using System;
using TypeWrapper.Tests.Mocks;
using Xunit;

namespace TypeWrapper.Tests
{
    public class PropertyTest
    {
        private readonly User _user = new User { Id = 42, Name = "Ben", Kind = "Allocation annihilator" };

        [Fact]
        public void CustomProperty()
        {
            var custom = new CustomDataGenerator();
            var wrapped = Wrap.Type<User>()
                .WithSourceProperties()
                .WithProperty("Custom", i => custom.GetCustomData(i))
                .Instance(_user);

            AssertUserIsFine(wrapped, out Type wrappedType);

            var customProperty = (string)wrappedType.GetProperty("Custom").GetValue(wrapped);
            Assert.Equal("http://example.com/Users/42", customProperty);
        }

        [Fact]
        public void PropertyExists()
        {
            var wrapped = Wrap.Type<User>()
                .WithSourceProperties()
                .Instance(_user);

            AssertUserIsFine(wrapped, out Type wrappedType);
        }

        [Fact]
        public void ThrowsOnNullInstance()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Wrap.Type<User>().Instance(null);
            }); 
        }

        [Fact]
        public void ThrowsOnNullDelegate()
        {
            Func<User, string> func = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                Wrap.Type<User>()
                    .WithProperty("Null", func);
            });
        }

        [Fact]
        public void ThrowsOnEmptyProperty()
        {
            Func<User, string> func = i => i.Kind;
            Assert.Throws<ArgumentNullException>(() =>
            {
                Wrap.Type<User>()
                    .WithProperty("  ", func);
            });
        }

        private void AssertUserIsFine(Wrapped<User> wrapped, out Type wrappedType)
        {
            wrappedType = wrapped.GetType();

            var id = (int) wrappedType.GetProperty("Id").GetValue(wrapped);
            var name = (string) wrappedType.GetProperty("Name").GetValue(wrapped);
            var kind = (string) wrappedType.GetProperty("Kind").GetValue(wrapped);

            Assert.Equal(_user.Id, id);
            Assert.Equal(_user.Name, name);
            Assert.Equal(_user.Kind, kind);
        }
    }
}
