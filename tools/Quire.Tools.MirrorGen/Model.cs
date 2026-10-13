using System.Collections.Immutable;
using System.Text;

namespace Quire.Tools.MirrorGen;

/// <summary>A type as it appears in a signature. Names are metadata names: namespace-qualified, nested types joined by '+'.</summary>
public abstract record TypeSig
{
    public sealed override string ToString() => Canonical.Write(this);
}

/// <summary>A named type, with its defining assembly's simple name and its generic arguments (outermost first).</summary>
public sealed record NamedSig(string Assembly, string Name, ImmutableArray<TypeSig> Args) : TypeSig
{
    public NamedSig(string assembly, string name) : this(assembly, name, []) { }
    public bool Equals(NamedSig? other) =>
        other is not null && Name == other.Name && Args.SequenceEqual(other.Args);
    public override int GetHashCode() => Name.GetHashCode();
}

public sealed record ArraySig(TypeSig Element, int Rank) : TypeSig; // Rank 0 = single-dimensional, zero-based
public sealed record PointerSig(TypeSig Element) : TypeSig;
public sealed record ByRefSig(TypeSig Element, bool ReadOnly = false) : TypeSig; // ReadOnly: C# `in` / `ref readonly`
public sealed record GenericParamSig(int Index, bool OfMethod) : TypeSig;
public sealed record FunctionPointerSig : TypeSig;

public enum TypeKind { Class, Struct, Interface, Enum, Delegate }

/// <summary>C# accessibility. Members and nested types can have all six; top-level types only Public or Internal.</summary>
public enum Access { Private, PrivateProtected, Internal, Protected, ProtectedInternal, Public }

public sealed record GenericParam(string Name, int Variance, bool Class, bool Struct, bool New, ImmutableArray<TypeSig> Constraints);

public sealed record ParamModel(string Name, TypeSig Type, bool In, bool Out, bool Params, bool HasDefault, object? Default);

/// <summary>
/// A mirrored member's state (PLAN.md §3c rule 5), from its attributes. Unclassified means hand-written code that
/// predates the per-method rule and still has to be verified, documented, blocked or marked Quire-only.
/// </summary>
public enum MemberStatus { Unclassified, Stub, Blocked, Verified, Documented, QuireOnly }

/// <summary>A member's status and the attribute text behind it (reason, test class or source).</summary>
public interface IHasStatus
{
    MemberStatus Status { get; }
    string? Evidence { get; }
    bool Stub => Status == MemberStatus.Stub;
}

public sealed record FieldModel(string Name, TypeSig Type, Access Access, bool Static, bool ReadOnly, bool Const, object? Value) : IHasStatus
{
    public MemberStatus Status { get; init; }
    public string? Evidence { get; init; }
    public bool Stub => Status == MemberStatus.Stub;
}

public sealed record MethodModel(
    string Name, Access Access, bool Static, bool Abstract, bool Virtual, bool NewSlot, bool Final, bool SpecialName,
    TypeSig Return, ImmutableArray<ParamModel> Params, ImmutableArray<GenericParam> GenericParams,
    ImmutableArray<(TypeSig Interface, string Method)> Implements, bool Extern, bool Extension, bool InitOnly) : IHasStatus
{
    public bool IsConstructor => Name is ".ctor" or ".cctor";
    public MemberStatus Status { get; init; }
    public string? Evidence { get; init; }
    public bool Stub => Status == MemberStatus.Stub;
}

public sealed record PropertyModel(string Name, TypeSig Type, MethodModel? Getter, MethodModel? Setter, ImmutableArray<ParamModel> Index) : IHasStatus
{
    public MemberStatus Status { get; init; }
    public string? Evidence { get; init; }
    public bool Stub => Status == MemberStatus.Stub;
}

public sealed record EventModel(string Name, TypeSig Type, MethodModel? Adder, MethodModel? Remover) : IHasStatus
{
    public MemberStatus Status { get; init; }
    public string? Evidence { get; init; }
    public bool Stub => Status == MemberStatus.Stub;
}

public sealed class TypeModel
{
    public required string Assembly { get; init; }
    public required string FullName { get; init; }          // e.g. Ptc.Controls.Foo`1+Bar
    public required string Namespace { get; init; }         // of the outermost type
    public required string Name { get; init; }              // simple metadata name, with arity suffix
    public string? DeclaringType { get; init; }
    public required TypeKind Kind { get; init; }
    public required Access Access { get; set; }
    public bool Abstract { get; init; }
    public bool Sealed { get; init; }
    public bool CompilerGenerated { get; init; }
    public ImmutableArray<GenericParam> GenericParams { get; set; } = [];
    public TypeSig? BaseType { get; set; }
    public ImmutableArray<TypeSig> Interfaces { get; set; } = [];
    public List<FieldModel> Fields { get; } = [];
    public List<MethodModel> Methods { get; } = [];
    public List<PropertyModel> Properties { get; } = [];
    public List<EventModel> Events { get; } = [];
    public string? EnumUnderlying { get; init; }
    public bool IsFlags { get; init; }
    public string? DefaultMember { get; init; }
    public int? NativeSize { get; init; }
    public bool IsReadOnly { get; init; }
    public bool IsByRefLike { get; init; }
    public bool IsStatic => Kind == TypeKind.Class && Abstract && Sealed;

    // Set on mapped (Quire-side) models only.
    public string? MirrorOf { get; init; }   // "Ptc.Controls.Foo, Ptc.Controls"
    public string? Project { get; init; }
    public List<string> Notes { get; } = [];
    public HashSet<string> CustomAttributes { get; init; } = [];
    public override string ToString() => FullName;
}

/// <summary>Canonical text for signatures: stable, comparable, and parseable for names with C++ punctuation.</summary>
public static class Canonical
{
    public static string Write(TypeSig t)
    {
        var sb = new StringBuilder();
        Write(sb, t);
        return sb.ToString();
    }

    public static void Write(StringBuilder sb, TypeSig t)
    {
        switch (t)
        {
            case NamedSig n:
                Name(sb, n.Name);
                if (n.Args.Length > 0)
                {
                    sb.Append('<');
                    for (int i = 0; i < n.Args.Length; i++) { if (i > 0) sb.Append(','); Write(sb, n.Args[i]); }
                    sb.Append('>');
                }
                break;
            case ArraySig a: Write(sb, a.Element); sb.Append('[').Append(',', Math.Max(0, a.Rank - 1)).Append(']'); break;
            case PointerSig p: Write(sb, p.Element); sb.Append('*'); break;
            case ByRefSig r: Write(sb, r.Element); sb.Append(r.ReadOnly ? "&in" : "&"); break;
            case GenericParamSig g: sb.Append(g.OfMethod ? "!!" : "!").Append(g.Index); break;
            case FunctionPointerSig: sb.Append("fnptr"); break;
        }
    }

    public static string Name(string name)
    {
        var sb = new StringBuilder();
        Name(sb, name);
        return sb.ToString();
    }

    private static void Name(StringBuilder sb, string name)
    {
        if (name.IndexOfAny(['<', '>', ',', '[', ']', '*', '&', '\'', '!', ' ']) < 0) { sb.Append(name); return; }
        sb.Append('\'').Append(name.Replace("'", "''")).Append('\'');
    }
}
