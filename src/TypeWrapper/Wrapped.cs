using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TypeWrapper
{
    public abstract class Wrapped<T>
        where T : class
    {
        public T Item { get; set; }
    }

    public class Wrap
    {
        public static Wrapped<T> Instance<T>(T instance)
            where T : class
        {
            AssemblyBuilder dynamicAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicObjects"), AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder dynamicModuleBuilder = dynamicAssemblyBuilder.DefineDynamicModule("DynamicObjectsModule");
            TypeBuilder typeBuilder = dynamicModuleBuilder.DefineType($"Wrapped_{Guid.NewGuid()}", TypeAttributes.Public, typeof(Wrapped<T>));

            var itemPropery = typeof(Wrapped<T>).GetProperty("Item");

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                var type = property.PropertyType;

                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);
                 
                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{name}",
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                    CallingConventions.HasThis, type, Type.EmptyTypes);

                ILGenerator getMethodIL = getMethodBuilder.GetILGenerator();
                getMethodIL.Emit(OpCodes.Ldarg_0);
                getMethodIL.Emit(OpCodes.Call, itemPropery.GetGetMethod());
                getMethodIL.Emit(OpCodes.Callvirt, property.GetGetMethod());

                getMethodIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethodBuilder);
            }

            var wrappedType = typeBuilder.CreateType();

            var wrapped = (Wrapped<T>)Activator.CreateInstance(wrappedType);

            wrapped.Item = instance;

            return wrapped;
        }
    }
}
