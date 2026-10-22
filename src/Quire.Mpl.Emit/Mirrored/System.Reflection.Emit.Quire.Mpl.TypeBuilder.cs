#nullable disable
#pragma warning disable

namespace System.Reflection.Emit.Quire.Mpl;

partial class TypeBuilder
{
    public global::System.Reflection.Emit.Quire.Mpl.ConstructorBuilder DefineTypeInitializer()
    {
        if (new global::System.Reflection.Emit.Quire.Mpl.ConstructorBuilder(".cctor", (global::System.Reflection.MethodAttributes.Public | global::System.Reflection.MethodAttributes.Static | global::System.Reflection.MethodAttributes.HideBySig | global::System.Reflection.MethodAttributes.SpecialName | global::System.Reflection.MethodAttributes.RTSpecialName) is var attributes ? attributes : default, (global::System.Reflection.CallingConventions)0, typeof(void), null, this) is var constructorBuilder)
        {
            _methodDefStore.Add(constructorBuilder);
            return constructorBuilder;
        }
        return null!;
    }
}
