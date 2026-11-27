namespace Quire.Tools.MirrorGen;

/// <summary>
/// The C# name MirrorGen gives each field of a type. A field keeps its Mathcad name unless that name is taken: a nested
/// type, the type's own name, an earlier field's name, or a method's name (C# forbids a field and a method of one name).
/// A taken name is renamed to the Mathcad name plus underscores, the first unused one. Fields are claimed in metadata
/// order, before properties and events, so only methods, nested types, the type's name and earlier fields matter.
/// CSharpWriter.WriteMembers and MethodAudit's field-rename registry both use this rule, so the registry row
/// for a field is the name MirrorGen emits.
/// </summary>
public static class FieldClaims
{
    /// <summary>The renamed fields only: Mathcad field name to the C# name MirrorGen emits for it.</summary>
    /// <param name="typeName">The type's simple name (arity suffix included or not; it is compared without it).</param>
    /// <param name="fields">The type's fields that MirrorGen emits, in metadata order.</param>
    /// <param name="methods">The C# names of the type's methods that MirrorGen emits.</param>
    /// <param name="nestedTypes">The simple names of the type's nested types that MirrorGen emits.</param>
    public static Dictionary<string, string> Renames(string typeName, IEnumerable<string> fields, IEnumerable<string> methods,
        IEnumerable<string> nestedTypes)
    {
        static string Plain(string n) => n.Split('`')[0];
        var methodNames = methods.ToHashSet();
        var taken = new HashSet<string> { Plain(typeName) };
        foreach (var nested in nestedTypes) taken.Add(Plain(nested));
        var renames = new Dictionary<string, string>();
        foreach (var field in fields)
        {
            if (field.Contains('.')) continue;  // explicit names claim nothing
            if (!(taken.Contains(Plain(field)) || methodNames.Contains(field)))
            {
                taken.Add(Plain(field));
                continue;
            }
            string renamed = field + "_";
            while (taken.Contains(renamed) || methodNames.Contains(renamed)) renamed += "_";
            taken.Add(renamed);
            renames[field] = renamed;
        }
        return renames;
    }
}
