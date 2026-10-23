using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class MethodHandleResolver
{
    public static Handle? ResolveMethod(TypeBuilder thisTypeBuilder, MethodBase method)
    {
        switch (method)
        {
            case ConstructorBuilder constructorBuilder:
                return ResolveConstructor(thisTypeBuilder, constructorBuilder);
            case ConstructorInfo constructorInfo when constructorInfo.GetType().Name == "RuntimeConstructorInfo":
                return ResolveConstructor(thisTypeBuilder, constructorInfo);
            case MethodBuilder methodBuilder:
                return ResolveMethod(thisTypeBuilder, methodBuilder);
            case MethodInfo methodInfo when methodInfo.GetType().Name == "RuntimeMethodBase":
                return ResolveMethod(thisTypeBuilder, methodInfo);
            default:
                return ResolveMethod(thisTypeBuilder, (MethodInfo)method);
        }
    }
}
