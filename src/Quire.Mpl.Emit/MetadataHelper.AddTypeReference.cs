#nullable enable
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal static partial class MetadataHelper
{
    internal static TypeReferenceHandle AddTypeReference(MetadataBuilder metadata, Type type, AssemblyReferenceHandle parent)
    {
        return AddTypeReference(metadata, parent, type.Name, type.Namespace);
    }

    internal static TypeReferenceHandle AddTypeReference(MetadataBuilder metadata, AssemblyReferenceHandle parent, string name, string? nameSpace)
    {
        return metadata.AddTypeReference(parent, (nameSpace == null) ? default : metadata.GetOrAddString(nameSpace), metadata.GetOrAddString(name));
    }
}
