using Newtonsoft.Json;
using System.Reflection;

namespace TypeWrapper
{
    public abstract class Wrapped<T>
        where T : class
    {
        protected internal TProperty GetInternalPropertyValue<TProperty>(string propertyName) 
            => (TProperty)InternalWrapBuilder.AccessorStore[propertyName].GetValue(InternalItem);

        [JsonIgnore]
        public T InternalItem { get; internal set; }
        internal WrapBuilder<T> InternalWrapBuilder { get; set; }
    }
}
