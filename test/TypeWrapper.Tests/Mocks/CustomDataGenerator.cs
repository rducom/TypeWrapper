namespace TypeWrapper.Tests.Mocks
{
    public class CustomDataGenerator
    {
        public string GetCustomData(User user) => "http://example.com/Users/" + user.Id;
    }
}