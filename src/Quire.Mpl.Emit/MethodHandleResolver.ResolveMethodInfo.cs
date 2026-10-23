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
        switch (methodInfo)
        {
            case MethodBuilder methodBuilder:
                return module._metadataReferenceTracker.GetOrAddMethodHandle(methodBuilder);
            case MethodInfo runtimeMethod when runtimeMethod.GetType().Name == "RuntimeMethodBase":
                return module._metadataReferenceTracker.GetOrAddMethodHandle(runtimeMethod);
            default:
                return module._metadataReferenceTracker.GetOrAddMethodHandle(methodInfo);
        }
    }
}
