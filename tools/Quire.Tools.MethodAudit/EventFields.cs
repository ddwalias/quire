using System.Reflection.Metadata;
using Quire.Tools.MirrorGen;
using System.Reflection.PortableExecutable;

/// <summary>
/// The backing field of an auto-event. Roslyn gives <c>event Action ValueChanged;</c> a compiler-generated private field
/// named after the event, while Mathcad's field is obfuscated (<c>b</c>). The Mathcad field is read from the original: the
/// one field its add and remove accessors operate on (<see cref="EventBacking"/>, which MirrorGen leaves out of the
/// generated source for the same events). No claim is written anywhere.
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
    /// the event is not in the module; FieldLike: both accessors run an Interlocked.CompareExchange loop.</summary>
    public static (List<string> Adder, List<string> Remover, bool FieldLike)? AccessorFields(MetadataReader md, PEReader pe, string typeName, string eventName)
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
                var add = EventBacking.Scan(md, pe, typeHandle, accessors.Adder);
                var remove = EventBacking.Scan(md, pe, typeHandle, accessors.Remover);
                return (add.Fields, remove.Fields, add.CompareExchange && remove.CompareExchange);
            }
        }
        return null;
    }
}
