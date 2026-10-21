using System.Collections.Immutable;

namespace Quire.Tools.MirrorGen;

/// <summary>How a mapped member is to be declared, after the planner's checks.</summary>
public enum Modifier { None, Virtual, Override, SealedOverride, Abstract, AbstractOverride }

/// <summary>A base-constructor call: the argument types to pass as <c>default(T)</c>, or null for none.</summary>
public sealed record BaseCall(ImmutableArray<TypeSig> Args);

/// <summary>
/// Checks each mapped type against the types it builds on and records what the C# declaration needs: override or
/// virtual, base-constructor arguments, explicit implementations for interface members nothing else implements, and
/// which members hand-written code already declares.
/// </summary>
public sealed class Planner(Mapper mapper, Externals externals, HandWritten handWritten, Dictionary<string, HashSet<string>> friends)
{
    /// <summary>True when <paramref name="from"/> can see internal members of <paramref name="owner"/>.</summary>
    public bool SeesInternals(string? owner, string? from) =>
        owner != null && from != null && (owner == from || (friends.TryGetValue(owner, out var f) && f.Contains(from)));

    public Dictionary<MethodModel, Modifier> Modifiers { get; } = new(ReferenceEqualityComparer.Instance);
    public Dictionary<MethodModel, Access> AccessOverrides { get; } = new(ReferenceEqualityComparer.Instance);
    public Dictionary<MethodModel, BaseCall> BaseCalls { get; } = new(ReferenceEqualityComparer.Instance);
    public Dictionary<TypeModel, List<(TypeSig Interface, MethodModel Member, string Kind, string Name)>> ExtraExplicit { get; } = [];
    public HashSet<object> Skip { get; } = new(ReferenceEqualityComparer.Instance);
    /// <summary>Overrides an obfuscator renamed and linked to their base method by MethodImpl: declared under the base name.</summary>
    public Dictionary<MethodModel, string> Renames { get; } = new(ReferenceEqualityComparer.Instance);
    /// <summary>
    /// Overrides of a concrete base member with the same signature. Their stubs call the base, so a stub override of
    /// a framework virtual (OnTemplateChanged, MeasureOverride…) keeps the inherited behaviour instead of throwing.
    /// </summary>
    public HashSet<MethodModel> CallsBase { get; } = new(ReferenceEqualityComparer.Instance);
    public Dictionary<TypeModel, string> TypeSkip { get; } = [];
    public HashSet<TypeModel> DropBase { get; } = [];

    public void Run()
    {
        foreach (var t in mapper.Types) MergeHandWritten(t);
        LowerTypeAccess();
        foreach (var t in mapper.Types)
        {
            if (TypeSkip.ContainsKey(t)) continue;
            foreach (var m in t.Methods) PlanMethod(t, m);
            foreach (var p in t.Properties) foreach (var a in new[] { p.Getter, p.Setter }) if (a != null) PlanMethod(t, a);
            foreach (var e in t.Events) foreach (var a in new[] { e.Adder, e.Remover }) if (a != null) PlanMethod(t, a);
            if (t.Kind is TypeKind.Class or TypeKind.Struct)
            {
                SkipBareAccessorImpls(t);
                CompleteInterfaces(t);
            }
        }
    }

    // ------------------------------------------------------------------ lookups

    /// <summary>The model for a Quire or external type, or null if it isn't known (assume the best then).</summary>
    public TypeModel? Get(TypeSig? sig, string fromProject) => sig is NamedSig n
        ? (mapper.Resolve(n.Name, fromProject) is { } q && q.Project == n.Assembly ? q
            : mapper.Lookup(n.Name).FirstOrDefault(x => x.Project == n.Assembly) ?? mapper.Lookup(n.Name).FirstOrDefault() ?? externals.Find(n.Name))
        : null;

    public static TypeSig Substitute(TypeSig sig, ImmutableArray<TypeSig> typeArgs, ImmutableArray<TypeSig> methodArgs = default) => sig switch
    {
        GenericParamSig { OfMethod: false } g when g.Index < typeArgs.Length => typeArgs[g.Index],
        GenericParamSig { OfMethod: true } g when !methodArgs.IsDefault && g.Index < methodArgs.Length => methodArgs[g.Index],
        NamedSig n when n.Args.Length > 0 => n with { Args = [.. n.Args.Select(a => Substitute(a, typeArgs, methodArgs))] },
        ArraySig a => a with { Element = Substitute(a.Element, typeArgs, methodArgs) },
        PointerSig p => p with { Element = Substitute(p.Element, typeArgs, methodArgs) },
        ByRefSig r => r with { Element = Substitute(r.Element, typeArgs, methodArgs) },
        _ => sig,
    };

    private static string Key(MethodModel m, ImmutableArray<TypeSig> typeArgs) =>
        $"{m.Name}`{m.GenericParams.Length}({string.Join(",", m.Params.Select(p => Substitute(p.Type, typeArgs)))})";

    /// <summary>All methods of a type, including property and event accessors.</summary>
    public static IEnumerable<MethodModel> AllMethods(TypeModel t) =>
        t.Methods.Concat(t.Properties.SelectMany(p => new[] { p.Getter, p.Setter }))
            .Concat(t.Events.SelectMany(e => new[] { e.Adder, e.Remover })).OfType<MethodModel>();

    /// <summary>Walks the base chain from <paramref name="start"/>, yielding each base with its type arguments.</summary>
    private IEnumerable<(TypeModel Type, ImmutableArray<TypeSig> Args, bool Known)> Bases(TypeModel t)
    {
        // Where hand-written code declares a different base, that base is the real one.
        TypeSig? BaseOf(TypeModel x) =>
            DropBase.Contains(x) ? handWritten.Find(x)?.BaseType
            : x.Kind == TypeKind.Struct ? new NamedSig("System.Runtime", "System.ValueType") // ToString/Equals overrides
            : x.BaseType;
        var sig = BaseOf(t);
        string project = t.Project ?? t.Assembly;
        for (int depth = 0; sig is NamedSig n && depth < 40; depth++)
        {
            var b = Get(n, project) ?? handWritten.FindByName(n.Name);
            if (b == null) { yield return (null!, n.Args, false); yield break; }
            yield return (b, n.Args, true);
            if (handWritten.Find(b) is { } hb)
                yield return (hb, n.Args, true); // hand-written members of a mirrored base count too
            sig = BaseOf(b) is { } next ? Substitute(next, n.Args) : null;
            project = b.Project ?? b.Assembly;
        }
    }

    /// <summary>True when the hand-written part of the type is a record, so the generated part must be one too.</summary>
    public bool IsRecord(TypeModel t) => handWritten.Find(t) is { } hw && hw.Methods.Any(m => m.Name == "<Clone>$");

    /// <summary>
    /// A public class can't derive from a non-public class, nor a public interface extend a non-public one (C++/CLI
    /// allows both). Such types are declared
    /// internal, repeated until no public type builds on a non-public one.
    /// </summary>
    private void LowerTypeAccess()
    {
        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var t in mapper.Types.Where(t => t.Access == Access.Public && t.DeclaringType == null && !TypeSkip.ContainsKey(t)))
            {
                // A class needs its base class at least as accessible; an interface needs its base interfaces.
                // A public class may implement an internal interface.
                var sigs = t.Kind == TypeKind.Interface ? t.Interfaces.AsEnumerable()
                    : t.BaseType is null || DropBase.Contains(t) ? [] : [t.BaseType];
                if (!sigs.SelectMany(Stats.Named).Any(n => mapper.Resolve(n.Name, t.Project) is { Project: not null } q
                        && q.Project == n.Assembly && !IsEffectivelyPublic(q))) continue;
                t.Access = Access.Internal;
                t.Notes.Add("declared internal: Mathcad's public type builds on a non-public base");
                changed = true;
            }
        }
    }

    public bool IsEffectivelyPublic(TypeModel t)
    {
        for (var cur = t; cur != null; cur = cur.DeclaringType == null ? null : mapper.Resolve(cur.DeclaringType, cur.Project))
            if (cur.Access is not (Access.Public or Access.Protected or Access.ProtectedInternal)) return false;
        return true;
    }

    public bool IsInterface(TypeSig sig, string project) => Get(sig, project) is { Kind: TypeKind.Interface } or null;

    /// <summary>
    /// Some Mathcad types implement interface property accessors with methods that have no property row (stripped
    /// metadata). C# can only implement those as properties, so the methods are skipped and
    /// <see cref="CompleteInterfaces"/> declares the explicit properties instead.
    /// </summary>
    private void SkipBareAccessorImpls(TypeModel t)
    {
        foreach (var m in t.Methods)
        {
            if (m.Access != Access.Private || m.Implements.IsEmpty) continue;
            bool allAccessors = m.Implements.All(i => Get(i.Interface, t.Project!) is { Kind: TypeKind.Interface } it
                && AccessorOf(it, AllMethods(it).FirstOrDefault(x => x.Name == i.Method) ?? m).Kind != "method");
            if (allAccessors)
            {
                Skip.Add(m);
                t.Notes.Add($"interface accessor {m.Name} has no property metadata; declared as an explicit property");
            }
        }
    }

    // ------------------------------------------------------------------ methods

    private void PlanMethod(TypeModel t, MethodModel m)
    {
        if (t.Kind == TypeKind.Interface)
        {
            Modifiers[m] = m.Abstract ? Modifier.Abstract : m.Virtual && !m.Static ? Modifier.Virtual : Modifier.None;
            return;
        }
        if (m.Access == Access.Private && !m.Implements.IsEmpty && !(m.Virtual && !m.NewSlot)) return; // explicit impl
        if (m.Static || !m.Virtual) { Modifiers[m] = Modifier.None; return; }

        // A MethodImpl naming a base-class method: a C# 9 covariant-return override, or an override an obfuscator
        // renamed. Either way it is declared as an override of that method, under its name and access.
        var classImpl = m.Implements.FirstOrDefault(i => Get(i.Interface, t.Project!) is { Kind: TypeKind.Class });
        if (classImpl.Interface != null)
        {
            Modifiers[m] = m.Abstract ? Modifier.AbstractOverride : m.Final ? Modifier.SealedOverride : Modifier.Override;
            if (classImpl.Method != m.Name) Renames[m] = classImpl.Method;
            var owner = Get(classImpl.Interface, t.Project!)!;
            var baseMember = AllMethods(owner).FirstOrDefault(x => x.Name == classImpl.Method && x.Params.Length == m.Params.Length);
            if (baseMember != null)
            {
                var access = baseMember.Access == Access.ProtectedInternal && owner.Project != t.Project ? Access.Protected : baseMember.Access;
                if (access != m.Access) AccessOverrides[m] = access;
            }
            return;
        }

        if (m.NewSlot)
        {
            Modifiers[m] = m.Abstract ? Modifier.Abstract
                : m.Final || t.Sealed || t.Kind == TypeKind.Struct || m.Access == Access.Private ? Modifier.None // C# has no private virtual
                : Modifier.Virtual;
            return;
        }

        // An override: find what it overrides.
        string key = Key(m, []);
        foreach (var (b, args, known) in Bases(t))
        {
            if (!known) { Modifiers[m] = OverrideKind(m); return; } // unknown base: trust the metadata
            var hit = AllMethods(b).FirstOrDefault(x => !x.Static && x.Virtual && Key(x, args) == key);
            if (hit == null) continue;
            Modifiers[m] = OverrideKind(m);
            if (!hit.Abstract && Substitute(hit.Return, args).ToString() == m.Return.ToString() && m.Return is not ByRefSig)
                CallsBase.Add(m);
            var access = hit.Access == Access.ProtectedInternal && (b.Project ?? b.Assembly) != t.Project ? Access.Protected : hit.Access;
            if (access != m.Access) AccessOverrides[m] = access;
            return;
        }
        Modifiers[m] = m.Abstract ? Modifier.Abstract : m.Final || t.Sealed || t.Kind == TypeKind.Struct ? Modifier.None : Modifier.Virtual;
        t.Notes.Add($"no base member for override {m.Name}; declared {Modifiers[m].ToString().ToLowerInvariant()} instead");
        if (m.Access == Access.Private) AccessOverrides[m] = Access.Protected;
    }

    private static Modifier OverrideKind(MethodModel m) =>
        m.Abstract ? Modifier.AbstractOverride : m.Final ? Modifier.SealedOverride : Modifier.Override;

    /// <summary>The base constructor a stub constructor must call, or null when a parameterless one is accessible.</summary>
    public BaseCall? BaseCallFor(TypeModel t)
    {
        if (t.Kind != TypeKind.Class || t.BaseType == null || DropBase.Contains(t)) return null;
        var b = Get(t.BaseType, t.Project!);
        if (b == null) return null;
        var args = ((NamedSig)t.BaseType).Args;
        bool sameProject = b.Project != null && b.Project == t.Project;
        bool friend = SeesInternals(b.Project, t.Project);
        // A nested type can call its enclosing type's private constructors.
        bool nestedInBase = sameProject && t.FullName.StartsWith(b.FullName + "+", StringComparison.Ordinal);
        bool Accessible(MethodModel c) =>
            c.Access is Access.Public or Access.Protected or Access.ProtectedInternal
            || (sameProject && c.Access is Access.PrivateProtected or Access.Internal)
            || (friend && c.Access == Access.Internal)
            || (nestedInBase && c.Access == Access.Private);
        // The base's constructors as they will exist: Mathcad's (generated, or skipped because hand-written code
        // declares them) plus any only hand-written code has.
        var handCtors = b.Project != null && handWritten.Find(b) is { } hb
            ? hb.Methods.Where(c => c.Name == ".ctor" && !c.Stub) : [];
        var ctors = b.Methods.Where(c => c.Name == ".ctor" && !c.Static).Concat(handCtors)
            .Where(Accessible)
            .Where(c => c.Params.All(p => p.Type is not ByRefSig))
            .OrderBy(c => c.Params.Count(p => !p.HasDefault)).ThenBy(c => c.Params.Length).ToList();
        if (ctors.Count == 0 || ctors[0].Params.All(p => p.HasDefault)) return null;
        return new BaseCall([.. ctors[0].Params.Where(p => !p.HasDefault).Select(p => Substitute(p.Type, args))]);
    }

    /// <summary>
    /// The hand-written constructor a generated constructor must chain to, so the hand-written initialisation runs:
    /// the one with fewest required parameters. Null when the type has no hand-written constructor.
    /// </summary>
    public MethodModel? HandConstructor(TypeModel t) =>
        handWritten.Find(t) is { } hw
            ? hw.Methods.Where(c => c.Name == ".ctor" && !c.Stub && c.Params.All(p => p.Type is not ByRefSig))
                .OrderBy(c => c.Params.Count(p => !p.HasDefault)).FirstOrDefault()
            : null;

    // ------------------------------------------------------------------ interfaces

    /// <summary>Adds explicit stubs for interface members that nothing in the type or its bases implements.</summary>
    private void CompleteInterfaces(TypeModel t)
    {
        var mine = AllMethods(t).Where(m => !Skip.Contains(m)).ToList();
        var explicitImpls = new HashSet<string>(mine.SelectMany(m => m.Implements.Select(i => $"{i.Interface}::{i.Method}")));
        var publicKeys = new HashSet<string>(mine.Where(m => m.Access == Access.Public && !m.Static).Select(m => Key(m, []) + "->" + m.Return));
        foreach (var (b, args, known) in Bases(t))
        {
            if (!known) break; // can't tell what an unknown base provides; explicit stubs are harmless
            foreach (var m in AllMethods(b).Where(m => m.Access == Access.Public && !m.Static))
                publicKeys.Add(Key(m, args) + "->" + Substitute(m.Return, args));
            foreach (var m in AllMethods(b))
                foreach (var i in m.Implements) explicitImpls.Add($"{Substitute(i.Interface, args)}::{i.Method}");
        }
        publicKeys.UnionWith(handWritten.PublicKeys(t));

        // C++/CLI lists only directly implemented interfaces, so walk the closure.
        var closure = new List<TypeSig>();
        var seen = new HashSet<string>();
        var pending = new Queue<TypeSig>(t.Interfaces);
        while (pending.Count > 0)
        {
            var i = pending.Dequeue();
            if (!seen.Add(i.ToString())) continue;
            closure.Add(i);
            if (Get(i, t.Project!) is { } im)
            {
                var iargs = ((NamedSig)i).Args;
                foreach (var b in im.Interfaces) pending.Enqueue(Substitute(b, iargs));
                if (im.Project != null && handWritten.Find(im) is { } hwi)
                    foreach (var b in hwi.Interfaces) pending.Enqueue(Substitute(b, iargs));
            }
        }

        var extra = new List<(TypeSig, MethodModel, string, string)>();
        foreach (var iface in closure)
        {
            var it = Get(iface, t.Project!);
            if (it == null) continue;
            var args = ((NamedSig)iface).Args;
            // Hand-written parts of a mirrored interface can declare members Mathcad's doesn't have.
            var hwIt = it.Project != null ? handWritten.Find(it) : null;
            var members = AllMethods(it).Select(m => (Iface: it, Member: m))
                .Concat(hwIt == null ? [] : AllMethods(hwIt).Where(m => !m.Stub).Select(m => (Iface: hwIt, Member: m)))
                .DistinctBy(x => Key(x.Member, []) + "->" + x.Member.Return);
            foreach (var (owner, im) in members.Where(x => x.Member.Abstract && !x.Member.Static))
            {
                if (explicitImpls.Contains($"{iface}::{im.Name}")) continue;
                if (publicKeys.Contains(Key(im, args) + "->" + Substitute(im.Return, args))) continue;
                var (kind, name) = AccessorOf(owner, im);
                var member = im with
                {
                    Return = Substitute(im.Return, args),
                    Params = [.. im.Params.Select(p => p with { Type = Substitute(p.Type, args) })],
                };
                extra.Add((iface, member, kind, name));
                explicitImpls.Add($"{iface}::{im.Name}");
            }
        }
        if (extra.Count > 0)
        {
            ExtraExplicit[t] = extra;
            t.Notes.Add($"added {extra.Count} explicit interface stub(s) for members no base provides: " +
                string.Join(", ", extra.Select(x => $"{x.Item1}.{x.Item4}").Distinct().Take(8)));
        }
    }

    private static (string Kind, string Name) AccessorOf(TypeModel iface, MethodModel m)
    {
        foreach (var p in iface.Properties)
            if (ReferenceEquals(p.Getter, m) || ReferenceEquals(p.Setter, m)) return ("property", p.Name);
        foreach (var e in iface.Events)
            if (ReferenceEquals(e.Adder, m) || ReferenceEquals(e.Remover, m)) return ("event", e.Name);
        return ("method", m.Name);
    }

    // ------------------------------------------------------------------ hand-written code

    private void MergeHandWritten(TypeModel t)
    {
        var hw = handWritten.Find(t);
        if (hw == null) return;
        if (t.Kind is TypeKind.Enum or TypeKind.Delegate || hw.Kind != t.Kind)
        {
            TypeSkip[t] = hw.Kind != t.Kind ? $"hand-written as {hw.Kind}, Mathcad has {t.Kind}" : $"hand-written {t.Kind}";
            t.Notes.Add("not generated: " + TypeSkip[t]);
            return;
        }
        if (hw.Access != t.Access)
        {
            t.Notes.Add($"hand-written code declares it {hw.Access.ToString().ToLowerInvariant()}; Mathcad's is {t.Access.ToString().ToLowerInvariant()}");
            t.Access = hw.Access;
        }
        if (hw.BaseType is NamedSig hb && hb.Name != "System.Object" && t.BaseType != null && hb.ToString() != t.BaseType.ToString())
        {
            DropBase.Add(t);
            t.Notes.Add($"hand-written base {hb} differs from Mathcad's {t.BaseType}; base not generated");
        }
        var keys = handWritten.MethodKeys(t);
        var nonMethods = handWritten.NonMethodNames(t);
        var methodNames = handWritten.MethodNames(t);
        if (DropBase.Contains(t))
        {
            // Members inherited from the hand-written base would be hidden by generated ones of the same name.
            var inherited = handWritten.InheritedNames(hw);
            var inheritedMethods = handWritten.InheritedMethodKeys(hw);
            foreach (var m in t.Methods.Where(m => !m.IsConstructor && inheritedMethods.Contains(HandWritten.MethodKey(m)))) { Skip.Add(m); t.Notes.Add($"method {m.Name} not generated: inherited from the hand-written base"); }
            foreach (var p in t.Properties.Where(p => inherited.Contains(p.Name))) { Skip.Add(p); t.Notes.Add($"property {p.Name} not generated: inherited from the hand-written base"); }
            foreach (var e in t.Events.Where(e => inherited.Contains(e.Name))) { Skip.Add(e); t.Notes.Add($"event {e.Name} not generated: inherited from the hand-written base"); }
            foreach (var f in t.Fields.Where(f => inherited.Contains(f.Name))) { Skip.Add(f); t.Notes.Add($"field {f.Name} not generated: inherited from the hand-written base"); }
        }
        void Conflict(string what) => t.Notes.Add($"hand-written code uses the name of {what} for a different kind of member; not generated");
        var optional = handWritten.OptionalOverloads(t);
        foreach (var m in t.Methods)
        {
            if (keys.Contains(HandWritten.MethodKey(m))) Skip.Add(m);
            else if (nonMethods.Contains(m.Name)) { Skip.Add(m); Conflict($"method {m.Name}"); }
            else if (optional.Contains(HandWritten.MethodKey(m)))
            {
                // A hand-written overload with optional parameters already accepts these arguments; a generated
                // overload would take its calls.
                Skip.Add(m);
                t.Notes.Add($"{(m.IsConstructor ? "constructor" : "method " + m.Name)}({string.Join(", ", m.Params.Select(p => p.Type))}) not generated: a hand-written overload with optional parameters covers it");
            }
        }
        foreach (var p in t.Properties)
        {
            if (nonMethods.Contains(p.Name)) Skip.Add(p);
            else if (methodNames.Contains(p.Name)) { Skip.Add(p); Conflict($"property {p.Name}"); }
        }
        foreach (var e in t.Events)
        {
            if (nonMethods.Contains(e.Name)) Skip.Add(e);
            else if (methodNames.Contains(e.Name)) { Skip.Add(e); Conflict($"event {e.Name}"); }
        }
        foreach (var f in t.Fields)
        {
            if (nonMethods.Contains(f.Name)) Skip.Add(f);
            else if (methodNames.Contains(f.Name)) { Skip.Add(f); Conflict($"field {f.Name}"); }
        }
    }
}

/// <summary>Quire's own compiled assemblies: what hand-written code already declares.</summary>
public sealed class HandWritten
{
    private readonly Dictionary<(string Project, string Name), TypeModel> _types = [];

    public static HandWritten Load(string repo, IEnumerable<string> projects)
    {
        var hw = new HandWritten();
        foreach (var project in projects)
        {
            var bin = Path.Combine(repo, "src", project, "bin", "Release");
            if (!Directory.Exists(bin)) continue;
            var dll = Directory.GetDirectories(bin).Select(d => Path.Combine(d, project + ".dll")).Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
            if (dll == null) continue;
            var model = MetadataLoader.Load(dll);
            if (model == null) continue;
            // Only a build without the generated files shows what is hand-written (Directory.Build.targets stamps it).
            if (model.Metadata.GetValueOrDefault("Quire.MirrorStubs") != "false")
                throw new InvalidOperationException(
                    $"{dll} is a build with the generated stubs. Run 'dotnet build -p:MirrorStubs=false' before 'emit'.");
            foreach (var t in model.Types) hw._types[(project, t.FullName)] = t;
        }
        return hw;
    }

    /// <summary>Any compiled Quire type by name, mirrored or not.</summary>
    public TypeModel? FindByName(string fullName) => _types.Values.FirstOrDefault(x => x.FullName == fullName);

    private IEnumerable<TypeModel> HandBases(TypeModel t)
    {
        var sig = t.BaseType as NamedSig;
        for (int depth = 0; sig != null && depth < 20; depth++)
        {
            var b = FindByName(sig.Name);
            if (b == null) yield break;
            yield return b;
            sig = b.BaseType as NamedSig;
        }
    }

    /// <summary>Names of the non-method members a hand-written type inherits from its hand-written bases.</summary>
    public HashSet<string> InheritedNames(TypeModel t) =>
        [.. HandBases(t).SelectMany(b => b.Properties.Where(p => !p.Stub).Select(p => p.Name)
            .Concat(b.Events.Where(e => !e.Stub).Select(e => e.Name))
            .Concat(b.Fields.Where(f => !f.Stub && f.Access != Access.Private).Select(f => f.Name)))];

    /// <summary>Signature keys of the methods a hand-written type inherits from its hand-written bases.</summary>
    public HashSet<string> InheritedMethodKeys(TypeModel t) =>
        [.. HandBases(t).SelectMany(b => HandMethodsOf(b).Where(m => m.Access != Access.Private).Select(MethodKey))];

    public TypeModel? Find(TypeModel q)
    {
        if (!_types.TryGetValue((q.Project!, q.FullName), out var t)) return null;
        if (t.MirrorOf != null && t.Kind is TypeKind.Enum or TypeKind.Delegate) return null; // generated whole
        // A type that only ever came from the generator has no hand-written part.
        bool anyHand = t.MirrorOf == null
            || HandMethodsOf(t).Any(m => !(m.Name == ".ctor" && m.Params.Length == 0))
            || t.Fields.Any(f => !f.Stub && !f.Name.StartsWith('<') && t.Kind != TypeKind.Enum);
        return anyHand ? t : null;
    }

    public static string MethodKey(MethodModel m) =>
        $"{Canonical.MethodName(m)}`{m.GenericParams.Length}({string.Join(",", m.Params.Select(p => p.Type))})";

    private IEnumerable<MethodModel> HandMethods(TypeModel q) => Find(q) is { } t ? HandMethodsOf(t) : [];

    /// <summary>Non-stub methods and accessors. [MirrorStub] sits on a property or event, not on its accessors.</summary>
    private static IEnumerable<MethodModel> HandMethodsOf(TypeModel t) =>
        t.Methods.Where(m => !m.Stub)
            .Concat(t.Properties.Where(p => !p.Stub).SelectMany(p => new[] { p.Getter, p.Setter }))
            .Concat(t.Events.Where(e => !e.Stub).SelectMany(e => new[] { e.Adder, e.Remover }))
            .OfType<MethodModel>().Where(m => !m.Name.StartsWith('<'));

    public HashSet<string> MethodKeys(TypeModel q) => [.. HandMethods(q).Select(MethodKey)];

    /// <summary>Keys of the shorter call shapes hand-written methods accept by leaving optional parameters out.</summary>
    public HashSet<string> OptionalOverloads(TypeModel q)
    {
        var keys = new HashSet<string>();
        foreach (var m in HandMethods(q))
            for (int n = m.Params.Length - 1; n >= 0 && m.Params[n].HasDefault; n--)
                keys.Add(MethodKey(m with { Params = m.Params[..n] }));
        return keys;
    }

    /// <summary>Public instance methods, keyed with their return type, as interface implementations are matched.</summary>
    public HashSet<string> PublicKeys(TypeModel q) =>
        [.. HandMethods(q).Where(m => m.Access == Access.Public && !m.Static).Select(m => MethodKey(m) + "->" + m.Return)];

    public HashSet<string> MethodNames(TypeModel q) =>
        Find(q) is { } t ? [.. t.Methods.Where(m => !m.Stub && !m.Name.StartsWith('<')).Select(m => m.Name)] : [];

    public HashSet<string> NonMethodNames(TypeModel q) =>
        Find(q) is { } t
            ? [.. t.Fields.Where(f => !f.Stub && !f.Name.StartsWith('<')).Select(f => f.Name)
                .Concat(t.Properties.Where(p => !p.Stub).Select(p => p.Name))
                .Concat(t.Events.Where(e => !e.Stub).Select(e => e.Name))]
            : [];
}
