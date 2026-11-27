using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

/// <summary>
/// The renamed fields of a Quire assembly: each field MirrorGen emitted with <c>[Quire.Mirror.MirrorName]</c>, naming the
/// Mathcad field it mirrors. The attribute is in the assembly that was built from the emitted source, so the mapping is
/// exactly what MirrorGen decided, with no separate file to drift.
/// </summary>
static class FieldProvenance
{
    /// <summary>(declaring type metadata full name, Quire field name, Mathcad field name), for each MirrorName attribute.</summary>
    public static List<(string Type, string Field, string MathcadField)> Read(MetadataReader md)
    {
        var rows = new List<(string Type, string Field, string MathcadField)>();
        foreach (var handle in md.FieldDefinitions)
        {
            var field = md.GetFieldDefinition(handle);
            foreach (var attributeHandle in field.GetCustomAttributes())
            {
                var attribute = md.GetCustomAttribute(attributeHandle);
                if (AttributeType(md, attribute) != "Quire.Mirror.MirrorNameAttribute") continue;
                var blob = md.GetBlobReader(attribute.Value);
                if (blob.ReadUInt16() != 1) continue;  // the custom-attribute prolog
                string? mathcadField = blob.ReadSerializedString();
                if (mathcadField is null) continue;
                rows.Add((FullName(md, field.GetDeclaringType()), md.GetString(field.Name), mathcadField));
            }
        }
        return rows;
    }

    private static string AttributeType(MetadataReader md, CustomAttribute attribute) => attribute.Constructor.Kind switch
    {
        HandleKind.MemberReference => Parent(md, md.GetMemberReference((MemberReferenceHandle)attribute.Constructor).Parent),
        HandleKind.MethodDefinition => FullName(md, md.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor).GetDeclaringType()),
        _ => "",
    };

    private static string Parent(MetadataReader md, EntityHandle parent)
    {
        if (parent.Kind == HandleKind.TypeDefinition) return FullName(md, (TypeDefinitionHandle)parent);
        if (parent.Kind != HandleKind.TypeReference) return "";
        var type = md.GetTypeReference((TypeReferenceHandle)parent);
        string ns = md.GetString(type.Namespace);
        return (ns.Length == 0 ? "" : ns + ".") + md.GetString(type.Name);
    }

    /// <summary>The metadata full name of a type of this module (nested types with '+'), as Structure names it.</summary>
    private static string FullName(MetadataReader md, TypeDefinitionHandle handle)
    {
        var type = md.GetTypeDefinition(handle);
        string name = md.GetString(type.Name);
        if (!type.GetDeclaringType().IsNil) return FullName(md, type.GetDeclaringType()) + "+" + name;
        string ns = md.GetString(type.Namespace);
        return ns.Length == 0 ? name : ns + "." + name;
    }
}
