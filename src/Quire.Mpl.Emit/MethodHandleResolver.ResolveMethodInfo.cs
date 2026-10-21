using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class MethodHandleResolver
{
    public static Handle? ResolveMethod(TypeBuilder thisTypeBuilder, MethodInfo methodInfo)
    {
        if (methodInfo.DeclaringType == null)
            throw new ArgumentNullException("DeclaringType must be not null");
        ModuleBuilder module = thisTypeBuilder.Module;
        EntityWrappers.MethodReferenceWrapper key = new EntityWrappers.MethodReferenceWrapper(methodInfo);
        if (module._definedMethods.ContainsKey(key))
            return module._definedMethods[key];
        if (module._metadataReferenceTracker == null)
            throw new ArgumentNullException("_metadataReferenceTracker must be not null");
        MethodBuilder methodBuilder;
        MethodInfo methodInfoLocal = methodInfo;
        methodBuilder = methodInfoLocal as MethodBuilder;
        if (methodBuilder is { })
            goto ResolvedMethodBuilder;
        if ((object)methodInfoLocal == null)
            goto DefaultMethodInfo;
        goto RuntimeMethodInfo;
    ResolvedMethodBuilder:
        return module._metadataReferenceTracker.GetOrAddMethodHandle(methodBuilder);
    RuntimeMethodInfo:
        if (methodInfoLocal.GetType().Name == "RuntimeMethodBase")
            return module._metadataReferenceTracker.GetOrAddMethodHandle(methodInfoLocal);
    DefaultMethodInfo:
        return module._metadataReferenceTracker.GetOrAddMethodHandle(methodInfo);
    }
}
