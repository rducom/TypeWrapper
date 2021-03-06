﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace TypeWrapper
{
    public class WrapBuilder<T>
        where T : class
    {
        private static ModuleBuilder _dynamicModuleBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName("DynamicObjects"), AssemblyBuilderAccess.RunAndCollect)
            .DefineDynamicModule("DynamicObjectsModule");

        private static readonly MethodInfo GetInternalPropertyValue =
            typeof(Wrapped<T>).GetMethod(nameof(Wrapped<T>.GetInternalPropertyValue), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly PropertyInfo _internalItemProperty =
            typeof(Wrapped<T>).GetProperty(nameof(Wrapped<T>.InternalItem));

        internal Dictionary<string, DelegateAccessor> AccessorStore { get; } = new Dictionary<string, DelegateAccessor>();

        private readonly TypeBuilder _typeBuilder;
        private Func<Wrapped<T>> _instanceBuilder;

        public WrapBuilder()
        {
            _typeBuilder = _dynamicModuleBuilder.DefineType($"Wrapped_{Guid.NewGuid()}", TypeAttributes.Public, typeof(Wrapped<T>));
        }

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
                getMethodIL.Emit(OpCodes.Call, _internalItemProperty.GetGetMethod());
                getMethodIL.Emit(OpCodes.Callvirt, property.GetGetMethod());
                getMethodIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethodBuilder);
            }
            return this;
        }

        public WrapBuilder<T> WithProperty<TProperty>(string propertyName, Func<T, TProperty> func)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

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


        private Wrapped<T> CreateInstance()
        {
            if (_instanceBuilder != null)
            {
                return _instanceBuilder();
            }
            Type wrappedType = _typeBuilder.CreateType(); 
            _instanceBuilder = Expression.Lambda<Func<Wrapped<T>>>(Expression.Block(wrappedType, Expression.New(wrappedType))).Compile();
            return _instanceBuilder();
        }

        public Wrapped<T> Instance(T instance)
        {
            var wrapped = CreateInstance();
            wrapped.InternalItem = instance ?? throw new ArgumentNullException(nameof(instance));
            wrapped.InternalWrapBuilder = this;
            return wrapped;
        }
    }
}