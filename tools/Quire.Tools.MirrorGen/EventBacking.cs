using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace Quire.Tools.MirrorGen;

/// <summary>
/// The backing field of a field-like event, read from the accessors' IL: Roslyn writes a field-like event as add and remove
/// accessors that run a compare-exchange loop over one private field, whatever the compiler (or obfuscator) named it.
/// MirrorGen leaves that field out of the generated source, since the event declaration recreates it, and MethodAudit
/// maps the Quire event's own field to it.
/// </summary>
public static class EventBacking
{
    private static readonly Dictionary<ushort, OpCode> Codes = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null)!)
        .ToDictionary(o => unchecked((ushort)o.Value));

    /// <summary>The field of <paramref name="owner"/> that both accessors of the event operate on: each uses exactly that
    /// one field and runs Interlocked.CompareExchange. Null for an event with accessors of any other shape.</summary>
    public static string? Of(MetadataReader md, PEReader pe, TypeDefinitionHandle owner, MethodDefinitionHandle adder, MethodDefinitionHandle remover)
    {
        if (adder.IsNil || remover.IsNil) return null;
        var add = Scan(md, pe, owner, adder);
        var remove = Scan(md, pe, owner, remover);
        return add.Fields.Count == 1 && remove.Fields.Count == 1 && add.Fields[0] == remove.Fields[0]
            && !add.Fields[0].StartsWith('?') && add.CompareExchange && remove.CompareExchange ? add.Fields[0] : null;
    }

    /// <summary>The fields of <paramref name="owner"/> a method reads or writes, in order of first use (an operand that is no
    /// such field reads as "?token"), and whether it calls Interlocked.CompareExchange.</summary>
    public static (List<string> Fields, bool CompareExchange) Scan(MetadataReader md, PEReader pe, TypeDefinitionHandle owner, MethodDefinitionHandle method)
    {
        var used = new List<string>();
        bool exchange = false;
        int rva = md.GetMethodDefinition(method).RelativeVirtualAddress;
        if (rva == 0) return (used, false);
        var il = pe.GetMethodBody(rva).GetILReader();
        while (il.RemainingBytes > 0)
        {
            byte first = il.ReadByte();
            var code = Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            switch (code.OperandType)
            {
                case OperandType.InlineNone: break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar: il.ReadByte(); break;
                case OperandType.InlineVar: il.ReadUInt16(); break;
                case OperandType.InlineI8:
                case OperandType.InlineR: il.ReadInt64(); break;
                case OperandType.InlineSwitch:
                    int count = il.ReadInt32();
                    il.Offset += 4 * count;
                    break;
                case OperandType.InlineField:
                    int token = il.ReadInt32();
                    string name = FieldName(md, owner, MetadataTokens.EntityHandle(token)) ?? $"?{token:x8}";
                    if (!used.Contains(name)) used.Add(name);
                    break;
                case OperandType.InlineMethod:
                    exchange |= IsCompareExchange(md, MetadataTokens.EntityHandle(il.ReadInt32()));
                    break;
                default: il.ReadInt32(); break;
            }
        }
        return (used, exchange);
    }

    private static bool IsCompareExchange(MetadataReader md, EntityHandle handle)
    {
        if (handle.Kind == HandleKind.MethodSpecification) handle = md.GetMethodSpecification((MethodSpecificationHandle)handle).Method;
        if (handle.Kind != HandleKind.MemberReference) return false;
        var reference = md.GetMemberReference((MemberReferenceHandle)handle);
        if (md.GetString(reference.Name) != "CompareExchange" || reference.Parent.Kind != HandleKind.TypeReference) return false;
        var type = md.GetTypeReference((TypeReferenceHandle)reference.Parent);
        return md.GetString(type.Name) == "Interlocked" && md.GetString(type.Namespace) == "System.Threading";
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
}
