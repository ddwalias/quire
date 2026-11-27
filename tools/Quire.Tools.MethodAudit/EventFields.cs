using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

/// <summary>
/// The backing field of an auto-event. Roslyn gives <c>event Action ValueChanged;</c> a compiler-generated private field
/// named after the event, while Mathcad's field is obfuscated (<c>b</c>). The Mathcad field is read from the original: the
/// one field its add and remove accessors operate on. No claim is written anywhere.
/// </summary>
static class EventFields
{
    /// <summary>(declaring type metadata full name, event name, backing field name) of each event of the module whose
    /// accessors' backing field is a compiler-generated private field of the event's own name.</summary>
    public static List<(string Type, string Event, string Field)> AutoEvents(MetadataReader md)
    {
        var rows = new List<(string Type, string Event, string Field)>();
        foreach (var typeHandle in md.TypeDefinitions)
        {
            var type = md.GetTypeDefinition(typeHandle);
            foreach (var eventHandle in type.GetEvents())
            {
                string name = md.GetString(md.GetEventDefinition(eventHandle).Name);
                foreach (var fieldHandle in type.GetFields())
                {
                    var field = md.GetFieldDefinition(fieldHandle);
                    if (md.GetString(field.Name) != name || (field.Attributes & System.Reflection.FieldAttributes.Private) == 0) continue;
                    if (field.GetCustomAttributes().Any(h => FieldProvenance.AttributeType(md, md.GetCustomAttribute(h))
                        == "System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
                        rows.Add((FieldProvenance.FullName(md, typeHandle), name, name));
                }
            }
        }
        return rows;
    }

    /// <summary>The fields the add and remove accessors of an event of an original type read or write: the field names
    /// each accessor uses; an operand that is not a field of the declaring type reads as "?token". Null where the type or
    /// the event is not in the module.</summary>
    public static (List<string> Adder, List<string> Remover)? AccessorFields(MetadataReader md, PEReader pe, string typeName, string eventName)
    {
        foreach (var typeHandle in md.TypeDefinitions)
        {
            if (FieldProvenance.FullName(md, typeHandle) != typeName) continue;
            foreach (var eventHandle in md.GetTypeDefinition(typeHandle).GetEvents())
            {
                var e = md.GetEventDefinition(eventHandle);
                if (md.GetString(e.Name) != eventName) continue;
                var accessors = e.GetAccessors();
                if (accessors.Adder.IsNil || accessors.Remover.IsNil) return null;
                return (Used(md, pe, typeHandle, accessors.Adder), Used(md, pe, typeHandle, accessors.Remover));
            }
        }
        return null;
    }

    /// <summary>The name of a field of <paramref name="owner"/> an operand names: a field definition of it, or a member
    /// reference through an instantiation of it (the accessors of a generic type name <c>T&lt;!0&gt;::field</c>).</summary>
    private static string? FieldName(MetadataReader md, TypeDefinitionHandle owner, EntityHandle handle)
    {
        if (handle.Kind == HandleKind.FieldDefinition)
        {
            var field = md.GetFieldDefinition((FieldDefinitionHandle)handle);
            return field.GetDeclaringType() == owner ? md.GetString(field.Name) : null;
        }
        if (handle.Kind != HandleKind.MemberReference) return null;
        var reference = md.GetMemberReference((MemberReferenceHandle)handle);
        if (reference.Parent.Kind != HandleKind.TypeSpecification) return null;
        var blob = md.GetBlobReader(md.GetTypeSpecification((TypeSpecificationHandle)reference.Parent).Signature);
        if (blob.ReadByte() != 0x15) return null;                    // GENERICINST
        if (blob.ReadByte() is not (0x11 or 0x12)) return null;      // VALUETYPE | CLASS
        var generic = blob.ReadTypeHandle();
        return generic.Kind == HandleKind.TypeDefinition && (TypeDefinitionHandle)generic == owner ? md.GetString(reference.Name) : null;
    }

    private static List<string> Used(MetadataReader md, PEReader pe, TypeDefinitionHandle owner, MethodDefinitionHandle handle)
    {
        var used = new List<string>();
        int rva = md.GetMethodDefinition(handle).RelativeVirtualAddress;
        if (rva == 0) return used;
        var il = pe.GetMethodBody(rva).GetILReader();
        while (il.RemainingBytes > 0)
        {
            byte first = il.ReadByte();
            var code = StructureReader.Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            switch (code.OperandType)
            {
                case System.Reflection.Emit.OperandType.InlineNone: break;
                case System.Reflection.Emit.OperandType.ShortInlineBrTarget:
                case System.Reflection.Emit.OperandType.ShortInlineI:
                case System.Reflection.Emit.OperandType.ShortInlineVar: il.ReadByte(); break;
                case System.Reflection.Emit.OperandType.InlineVar: il.ReadUInt16(); break;
                case System.Reflection.Emit.OperandType.InlineI8:
                case System.Reflection.Emit.OperandType.InlineR: il.ReadInt64(); break;
                case System.Reflection.Emit.OperandType.InlineSwitch:
                    int count = il.ReadInt32();
                    il.Offset += 4 * count;
                    break;
                case System.Reflection.Emit.OperandType.InlineField:
                    int token = il.ReadInt32();
                    string name = FieldName(md, owner, MetadataTokens.EntityHandle(token)) ?? $"?{token:x8}";
                    if (!used.Contains(name)) used.Add(name);
                    break;
                default: il.ReadInt32(); break;
            }
        }
        return used;
    }
}
