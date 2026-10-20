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
        EntityWrappers.FieldReferenceWrapper key;
        FieldBuilder field2;
        FieldInfo field3;
        Handle? handle;
        field2 = field as FieldBuilder;
        if (field2 is not null)
        {
            goto ResolveDefinedField;
        }
        if ((object)field == null) goto UnknownField;
        goto ResolveExternalField;

    ResolveDefinedField:
        key = new EntityWrappers.FieldReferenceWrapper(field2);
        if (module._definedFields.ContainsKey(key))
        {
            return module._definedFields[key];
        }
        handle = null;
        return handle;

    ResolveExternalField:
        field3 = field;
        if (module._metadataReferenceTracker == null)
        {
            throw new InvalidOperationException("module._metadataReferenceTracker must be not null");
        }
        return module._metadataReferenceTracker.GetOrAddFieldReference(field3);

    UnknownField:
        throw new InvalidDataException("Unknown field " + field.Name);
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
        EntityWrappers.TypeReferenceWrapper key;
        TypeBuilder type2;
        Type type3;
        Handle? handle;
        type2 = type as TypeBuilder;
        if (type2 is not null)
        {
            goto ResolveDefinedType;
        }
        if ((object)type == null) goto UnknownType;
        goto ResolveExternalType;

    ResolveDefinedType:
        key = new EntityWrappers.TypeReferenceWrapper(type2);
        if (module._definedTypes.ContainsKey(key))
        {
            return module._definedTypes[key];
        }
        handle = null;
        return handle;

    ResolveExternalType:
        type3 = type;
        if (module._metadataReferenceTracker == null)
        {
            throw new InvalidOperationException("module._metadataReferenceTracker must be not null");
        }
        return module._metadataReferenceTracker.GetOrAddTypeReference(type3);

    UnknownType:
        throw new InvalidDataException("Unknown type " + type.Name);
    }
}
