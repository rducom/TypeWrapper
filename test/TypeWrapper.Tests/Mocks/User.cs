namespace TypeWrapper.Tests.Mocks
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
    }


    public class RootInstance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public SubInstance SubInstance { get; set; }
    }

    public class SubInstance
    {
        public int Id { get; set; }
        public string SubName { get; set; }
        public string SubKind { get; set; }
    }
}