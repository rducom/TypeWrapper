using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

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

    public abstract class Wrapped<T>
        where T : class
    {
        internal static PropertyInfo InternalItemProperty { get; } = typeof(Wrapped<T>).GetProperty(nameof(InternalItem));

        protected internal TProperty GetInternalPropertyValue<TProperty>(string propertyName)
        {
            object value = InternalWrapBuilder.AccessorStore[propertyName].GetValue(InternalItem);
            return (TProperty)value;
        }

        public T InternalItem { get; internal set; }
        internal WrapBuilder<T> InternalWrapBuilder { get; set; }
    }
     

    public class WrapBuilder<T>
        where T : class
    {
        private static ModuleBuilder _dynamicModuleBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("DynamicObjects"), AssemblyBuilderAccess.RunAndCollect)
            .DefineDynamicModule("DynamicObjectsModule");

        private static readonly MethodInfo GetInternalPropertyValue =
            typeof(Wrapped<T>).GetMethod("GetInternalPropertyValue", BindingFlags.Instance | BindingFlags.NonPublic);

        internal Dictionary<string, DelegateAccessor> AccessorStore { get; } = new Dictionary<string, DelegateAccessor>();

        public WrapBuilder()
        {
            _typeBuilder = _dynamicModuleBuilder.DefineType($"Wrapped_{Guid.NewGuid()}", TypeAttributes.Public, typeof(Wrapped<T>));
        }

        private readonly TypeBuilder _typeBuilder;

        public WrapBuilder<T> WithSourceProperties()
        {
            var properties = typeof(T).GetProperties();
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];

                PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);

                MethodBuilder getMethodBuilder = _typeBuilder.DefineMethod($"get_{property.Name}",
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                    CallingConventions.HasThis, property.PropertyType, Type.EmptyTypes);

                ILGenerator getMethodIL = getMethodBuilder.GetILGenerator();

                getMethodIL.Emit(OpCodes.Ldarg_0);
                getMethodIL.Emit(OpCodes.Call, Wrapped<T>.InternalItemProperty.GetGetMethod());
                getMethodIL.Emit(OpCodes.Callvirt, property.GetGetMethod());
                getMethodIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethodBuilder);
            }
            return this;
        }

        public WrapBuilder<T> WithProperty<TProperty>(string propertyName, Func<T, TProperty> func)
        {
            AccessorStore.Add(propertyName, new DelegateAccessor<T, TProperty>(func));

            var targetType = typeof(TProperty);
            PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, targetType, null);

            MethodBuilder getMethodBuilder = _typeBuilder.DefineMethod($"get_{propertyName}",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.HasThis, targetType, Type.EmptyTypes);

            ILGenerator getMethodIL = getMethodBuilder.GetILGenerator();

            getMethodIL.Emit(OpCodes.Ldarg_0);
            getMethodIL.Emit(OpCodes.Ldstr, propertyName);
            getMethodIL.Emit(OpCodes.Callvirt, GetInternalPropertyValue.MakeGenericMethod(typeof(TProperty)));
            getMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);

            return this;
        }

        public Wrapped<T> Instance(T instance)
        {
            var wrappedType = _typeBuilder.CreateType();
            var wrapped = (Wrapped<T>)Activator.CreateInstance(wrappedType);
            wrapped.InternalItem = instance;
            wrapped.InternalWrapBuilder = this;
            return wrapped;
        }
    }

    public class Wrap
    {
        public static WrapBuilder<T> Type<T>() where T : class => new WrapBuilder<T>();
    }
     
}
