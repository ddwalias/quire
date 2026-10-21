using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class MethodHandleResolver
{
    public static Handle? ResolveMethod(TypeBuilder thisTypeBuilder, MethodBase method)
    {
        ConstructorBuilder constructorBuilder;
        ConstructorInfo constructorInfo;
        MethodBuilder methodBuilder;
        MethodInfo methodInfo;
        MethodBase methodBase = method;

        constructorBuilder = methodBase as ConstructorBuilder;
        if (constructorBuilder is { })
            goto ConstructorBuilderResolved;
        constructorInfo = methodBase as ConstructorInfo;
        if (constructorInfo is { })
            goto ConstructorInfoResolved;
        methodBuilder = methodBase as MethodBuilder;
        if (methodBuilder is { })
            goto MethodBuilderResolved;
        methodInfo = methodBase as MethodInfo;
        if ((object)methodInfo == null)
            goto DefaultMethodInfo;
        goto MethodInfoResolved;

    ConstructorBuilderResolved:
        return ResolveConstructor(thisTypeBuilder, constructorBuilder);
    ConstructorInfoResolved:
        if (constructorInfo.GetType().Name == "RuntimeConstructorInfo")
            return ResolveConstructor(thisTypeBuilder, constructorInfo);
        goto DefaultMethodInfo;
    MethodBuilderResolved:
        return ResolveMethod(thisTypeBuilder, methodBuilder);
    MethodInfoResolved:
        if (methodInfo.GetType().Name == "RuntimeMethodBase")
            return ResolveMethod(thisTypeBuilder, methodInfo);
    DefaultMethodInfo:
        return ResolveMethod(thisTypeBuilder, (MethodInfo)method);
    }
}
