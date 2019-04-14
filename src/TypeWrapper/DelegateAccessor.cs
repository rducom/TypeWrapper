using System;

namespace TypeWrapper
{
    public abstract class DelegateAccessor
    {
        public abstract object GetValue(object target);
    }

    public class DelegateAccessor<T, TProperty> : DelegateAccessor
        where T : class
    {
        private readonly Func<T, TProperty> _func;
        public DelegateAccessor(Func<T, TProperty> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public override object GetValue(object target)
        {
            if (target is T o)
            {
                return _func(o);
            }
            throw new Exception();
        }
    }
}