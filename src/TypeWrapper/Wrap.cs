namespace TypeWrapper
{
    public class Wrap
    {
        public static WrapBuilder<T> Type<T>() where T : class => new WrapBuilder<T>();
    }
}