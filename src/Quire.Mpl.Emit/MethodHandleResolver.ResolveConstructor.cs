using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class MethodHandleResolver
{
    public static Handle? ResolveConstructor(TypeBuilder thisTypeBuilder, ConstructorInfo ctorInfo)
    {
        if (ctorInfo.DeclaringType == null)
            throw new ArgumentNullException("DeclaringType must be not null");
        ModuleBuilder module = thisTypeBuilder.Module;
        EntityWrappers.ConstructorReferenceWrapper key = new EntityWrappers.ConstructorReferenceWrapper(ctorInfo);
        if (module._definedConstructors.ContainsKey(key))
            return module._definedConstructors[key];
        if (module._metadataReferenceTracker == null)
            throw new ArgumentNullException("_metadataReferenceTracker must be not null");
        ConstructorBuilder ctor;
        ConstructorInfo constructorInfo = ctorInfo;
        ctor = constructorInfo as ConstructorBuilder;
        if (ctor is { })
            goto ResolvedConstructorBuilder;
        if ((object)constructorInfo == null)
            goto UnknownConstructor;
        goto RuntimeConstructor;
    ResolvedConstructorBuilder:
        return module._metadataReferenceTracker.GetOrAddConstructorReference(ctor);
    RuntimeConstructor:
        if (constructorInfo.GetType().Name == "RuntimeConstructorInfo")
            return module._metadataReferenceTracker.GetOrAddConstructorReference(constructorInfo);
    UnknownConstructor:
        throw new InvalidDataException("Unknown counstructor " + ctorInfo.Name);
    }
}
