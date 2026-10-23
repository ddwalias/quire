using System.IO;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class FieldHandleResolver
{
    public static void ResolveHandleEmit(TypeBuilder typeBuilder, FieldInfo field, Action<Handle> action, Action<FieldInfo> actionError)
    {
        Handle? handle = ResolveType(typeBuilder, field);
        if (handle.HasValue)
        {
            action(handle.Value);
        }
        else
        {
            actionError(field);
        }
    }

    public static Handle? ResolveType(TypeBuilder typeBuilder, FieldInfo field)
    {
        ModuleBuilder module = typeBuilder.Module;
        switch (field)
        {
            case FieldBuilder fieldBuilder:
                EntityWrappers.FieldReferenceWrapper key = new EntityWrappers.FieldReferenceWrapper(fieldBuilder);
                if (module._definedFields.ContainsKey(key))
                {
                    return module._definedFields[key];
                }
                return null;
            case FieldInfo externalField:
                if (module._metadataReferenceTracker == null)
                {
                    throw new InvalidOperationException("module._metadataReferenceTracker must be not null");
                }
                return module._metadataReferenceTracker.GetOrAddFieldReference(externalField);
            default:
                throw new InvalidDataException("Unknown field " + field.Name);
        }
    }
}

internal partial class TypeHandleResolver
{
    public static void ResolveHandleEmit(TypeBuilder typeBuilder, Type type, Action<Handle> action, Action<Type> actionError)
    {
        Handle? handle = ResolveType(typeBuilder, type);
        if (handle.HasValue)
        {
            action(handle.Value);
        }
        else
        {
            actionError(type);
        }
    }

    public static Handle? ResolveType(TypeBuilder typeBuilder, Type type)
    {
        ModuleBuilder module = typeBuilder.Module;
        switch (type)
        {
            case TypeBuilder typeBuilder2:
                EntityWrappers.TypeReferenceWrapper key = new EntityWrappers.TypeReferenceWrapper(typeBuilder2);
                if (module._definedTypes.ContainsKey(key))
                {
                    return module._definedTypes[key];
                }
                return null;
            case Type externalType:
                if (module._metadataReferenceTracker == null)
                {
                    throw new InvalidOperationException("module._metadataReferenceTracker must be not null");
                }
                return module._metadataReferenceTracker.GetOrAddTypeReference(externalType);
            default:
                throw new InvalidDataException("Unknown type " + type.Name);
        }
    }
}
