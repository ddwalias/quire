using System.Reflection;
using System.Reflection.Metadata;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public TypeReferenceHandle GetOrAddTypeReference(Type type)
    {
        EntityWrappers.TypeReferenceWrapper key = new EntityWrappers.TypeReferenceWrapper(type);
        if (_typeRefs.TryGetValue(key, out var value))
        {
            return value;
        }
        TypeInfo typeInfo = type.GetTypeInfo();
        _ = typeInfo.IsGenericType;
        _ = typeInfo.IsGenericTypeDefinition;
        _ = typeInfo.ContainsGenericParameters;
        _ = typeInfo.GenericTypeParameters;
        AssemblyName resolvedAssemblyName = AssemblyNameResolver.GetResolvedAssemblyName(type);
        value = _metadata.AddTypeReference(GetOrAddAssemblyReference(resolvedAssemblyName), _metadata.GetOrAddString(type.Namespace ?? string.Empty), _metadata.GetOrAddString(type.Name));
        _typeRefs[key] = value;
        return value;
    }
}
