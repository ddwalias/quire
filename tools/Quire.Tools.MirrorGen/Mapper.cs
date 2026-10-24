using System.Collections.Immutable;

namespace Quire.Tools.MirrorGen;

/// <summary>
/// Produces the Quire-side models: every in-scope Mathcad type with its names mapped, its signatures rewritten to
/// Quire or substitute types, and its members filtered to the ones a C# mirror can and should declare. Members that
/// can't be mapped are dropped with a note saying why.
/// </summary>
public sealed class Mapper
{
    private readonly Universe _mathcad;
    private readonly Rules _rules;
    private readonly Dictionary<string, ScopeEntry> _primary = [];             // Mathcad full name → its Mirror entry
    private readonly Dictionary<(string Project, string Name), TypeModel> _byProject = [];
    private readonly Dictionary<string, List<TypeModel>> _byName = [];

    public List<TypeModel> Types { get; } = [];

    public Mapper(Universe mathcad, Rules rules)
    {
        _mathcad = mathcad;
        _rules = rules;
        foreach (var e in rules.Scope.Values)
            if (e.Disposition == Disposition.Mirror) _primary.TryAdd(e.MathcadType, e);
    }

    public IReadOnlyList<TypeModel> Lookup(string quireName) => _byName.TryGetValue(quireName, out var l) ? l : [];

    /// <summary>The model a type reference from <paramref name="fromProject"/> binds to: its own copy first.</summary>
    public TypeModel? Resolve(string quireName, string? fromProject)
    {
        if (fromProject != null && _byProject.TryGetValue((fromProject, quireName), out var own)) return own;
        var all = Lookup(quireName);
        return all.FirstOrDefault(t => t.Access == Access.Public) ?? all.FirstOrDefault();
    }

    public void Build()
    {
        // 1. Decide which (assembly, type) pairs produce a Quire type, and where.
        var sources = new List<(AssemblyModel Asm, TypeModel Type, string Project)>();
        foreach (var asm in _mathcad.Assemblies.Values)
            foreach (var t in asm.Types)
            {
                if (!_rules.TryFind(asm.Name, t.FullName, out var e)) continue;
                string project = _rules.HostFor(e.Assembly);
                switch (e.Disposition)
                {
                    case Disposition.Mirror:
                    case Disposition.SharedSource:
                        sources.Add((asm, t, project));
                        break;
                    case Disposition.Duplicate when !IsPublicEverywhere(t):
                        sources.Add((asm, t, project)); // internal: each project keeps its own copy, as in Mathcad
                        break;
                }
            }

        // Primary copies first, so a project's copy of a duplicate is the primary when it has one.
        sources = [.. sources.OrderByDescending(x => IsPrimary(x.Asm.Name, x.Type))]; // stable
        var shells = new List<(TypeModel Src, TypeModel Shell, string Asm)>();
        foreach (var (asm, t, project) in sources)
        {
            string name = QuireName(t.FullName, PrimaryAssembly(asm.Name, t.FullName));
            if (_byProject.TryGetValue((project, name), out var existing))
            {
                if (existing.MirrorOf != $"{t.FullName}, {asm.Name}" && !existing.MirrorOf!.StartsWith(t.FullName + ", "))
                    existing.Notes.Add($"{t.FullName} ({asm.Name}) maps to the same Quire name and is not generated");
                continue;
            }
            var shell = Shell(t, asm.Name, name, project);
            _byProject[(project, name)] = shell;
            if (!_byName.TryGetValue(name, out var list)) _byName[name] = list = [];
            list.Add(shell);
            shells.Add((t, shell, asm.Name));
        }

        // 2. Fill in signatures, now that every Quire type exists to be referenced.
        foreach (var (src, shell, asm) in shells)
        {
            Fill(src, shell, asm);
            Types.Add(shell);
        }
    }

    /// <summary>The Quire name a type-map row maps to (duplicates share their primary's name).</summary>
    public string QuireNameFor(ScopeEntry e) => e.Disposition == Disposition.OwnEquivalent
        ? "Quire.ComponentModel." + e.MathcadType[(e.MathcadType.Split('+')[0].LastIndexOf('.') + 1)..]
        : QuireName(e.MathcadType, PrimaryAssembly(e.Assembly, e.MathcadType));

    private bool IsPrimary(string asm, TypeModel t) =>
        _rules.TryFind(asm, t.FullName, out var e) && e.Disposition is Disposition.Mirror;

    private string PrimaryAssembly(string asm, string fullName) =>
        _rules.TryFind(asm, fullName, out var e) && e.Disposition == Disposition.Duplicate && _primary.TryGetValue(fullName, out var p)
            ? p.Assembly : asm;

    private static bool IsPublicEverywhere(TypeModel t) => t.Access == Access.Public && t.DeclaringType == null;

    // ------------------------------------------------------------------ names

    private readonly Dictionary<(string Asm, string Type), HashSet<string>> _memberNames = [];

    /// <summary>Names of a Mathcad type's members, which its nested types must not reuse in C#.</summary>
    private HashSet<string> MemberNames(string asm, string fullName)
    {
        if (_memberNames.TryGetValue((asm, fullName), out var set)) return set;
        set = [];
        var a = _mathcad.Assemblies.Values.FirstOrDefault(x =>
            string.Equals(Path.GetFileNameWithoutExtension(x.Path), asm, StringComparison.OrdinalIgnoreCase));
        if (a != null)
            if (a.Types.FirstOrDefault(t => t.FullName == fullName) is { } type)
                set = [.. type.Methods.Select(m => m.Name).Concat(type.Properties.Select(p => p.Name))
                    .Concat(type.Events.Select(e => e.Name)).Concat(type.Fields.Select(f => f.Name)).Select(Identifier.Encode)];
        return _memberNames[(asm, fullName)] = set;
    }

    /// <summary>Quire metadata name for a Mathcad type: namespace mapped, each simple name encoded.</summary>
    public string QuireName(string fullName, string asm)
    {
        if (_mathcad.Assemblies.TryGetValue(asm, out var model)) asm = Path.GetFileNameWithoutExtension(model.Path); // file stem

        var segments = fullName.Split('+');
        string outer = segments[0];
        int dot = outer.LastIndexOf('.');
        string ns = dot < 0 ? "" : outer[..dot];
        segments[0] = dot < 0 ? outer : outer[(dot + 1)..];
        var encoded = segments.Select(Identifier.Encode).ToArray();
        // C# forbids a nested type named like its enclosing type or one of its members; obfuscated Mathcad types do
        // both. Members keep their names (they may override or implement), so the nested type gets a suffix.
        var raw = fullName.Split('+');
        for (int i = 1; i < encoded.Length; i++)
        {
            string Plain(string x) => x.Split('`')[0];
            string parent = string.Join("+", raw.Take(i));
            while (Plain(encoded[i]) == Plain(encoded[i - 1]) || MemberNames(asm, parent).Contains(Plain(encoded[i])))
            {
                int tick = encoded[i].IndexOf('`');
                encoded[i] = tick < 0 ? encoded[i] + "_" : encoded[i][..tick] + "_" + encoded[i][tick..];
            }
        }
        return Rules.MapNamespace(ns, asm) + "." + string.Join("+", encoded);
    }

    private static readonly Dictionary<string, (string Asm, string Name)> Substitutes = new()
    {
        // D6: DevComponents → Fluent.Ribbon / AvalonDock
        ["DevComponents.WpfRibbon.RibbonBar"] = ("Fluent", "Fluent.RibbonGroupBox"),
        ["DevComponents.WpfRibbon.ButtonDropDown"] = ("Fluent", "Fluent.DropDownButton"),
        ["DevComponents.WpfRibbon.ButtonEx"] = ("Fluent", "Fluent.Button"),
        ["DevComponents.WpfRibbon.ApplicationMenu"] = ("Fluent", "Fluent.ApplicationMenu"),
        ["DevComponents.WpfRibbon.RibbonTab"] = ("Fluent", "Fluent.RibbonTabItem"),
        ["DevComponents.WpfRibbon.Qat"] = ("Fluent", "Fluent.QuickAccessToolBar"),
        ["DevComponents.WpfRibbon.RibbonWindow"] = ("Fluent", "Fluent.RibbonWindow"),
        ["DevComponents.WpfRibbon.Gallery"] = ("Fluent", "Fluent.Gallery"),
        ["DevComponents.WpfRibbon.GroupPanel"] = ("PresentationFramework", "System.Windows.Controls.Panel"),
        ["DevComponents.WpfRibbon.RibbonBarPanel"] = ("PresentationFramework", "System.Windows.Controls.Panel"),
        ["DevComponents.WpfDock.DockWindow"] = ("AvalonDock", "AvalonDock.Layout.LayoutAnchorable"),
        ["DevComponents.WpfDock.DockSite"] = ("AvalonDock", "AvalonDock.DockingManager"),
        ["DevComponents.WpfDock.DockRoutedEventArgs"] = ("PresentationCore", "System.Windows.RoutedEventArgs"),
    };

    /// <summary>Maps a Mathcad-side signature type to Quire. Returns null with a reason when it has no counterpart.</summary>
    public TypeSig? Map(TypeSig sig, string fromProject, out string? reason)
    {
        reason = null;
        switch (sig)
        {
            case NamedSig n:
            {
                var named = MapNamed(n, fromProject, out reason);
                if (named == null) return null;
                var args = ImmutableArray.CreateBuilder<TypeSig>(n.Args.Length);
                foreach (var a in n.Args)
                {
                    var m = Map(a, fromProject, out reason);
                    if (m == null) return null;
                    args.Add(m);
                }
                return named with { Args = args.MoveToImmutable() };
            }
            case ArraySig a: return Map(a.Element, fromProject, out reason) is { } e1 ? a with { Element = e1 } : null;
            case PointerSig p:
                if (Map(p.Element, fromProject, out reason) is { } e2) return p with { Element = e2 };
                // A pointer to a native toolchain type (a Windows handle such as HWND__*, an STL object) is an
                // address to managed code: nint, as .NET spells native handles.
                if (reason != null && reason.StartsWith("toolchain")) { reason = null; return new NamedSig("System.Runtime", "System.IntPtr"); }
                return null;
            case ByRefSig r: return Map(r.Element, fromProject, out reason) is { } e3 ? r with { Element = e3 } : null;
            case FunctionPointerSig: reason = "function pointer"; return null;
            default: return sig;
        }
    }

    private NamedSig? MapNamed(NamedSig n, string fromProject, out string? reason)
    {
        reason = null;
        if (_mathcad.IsMathcad(n.Assembly))
        {
            if (!_rules.TryFind(n.Assembly, n.Name, out var e)) { reason = $"unlisted Mathcad type {n.Name}"; return null; }
            switch (e.Disposition)
            {
                case Disposition.Mirror:
                case Disposition.SharedSource:
                case Disposition.Duplicate:
                {
                    string name = QuireName(n.Name, PrimaryAssembly(n.Assembly, n.Name));
                    var target = Resolve(name, fromProject);
                    if (target == null) { reason = $"no Quire type for {n.Name}"; return null; }
                    return new NamedSig(target.Project!, name);
                }
                case Disposition.Dependency:
                    return new NamedSig("JetBrains.Annotations", n.Name);
                case Disposition.OwnEquivalent:
                    return new NamedSig("Quire.Core", "Quire.ComponentModel." + n.Name[(n.Name.Split('+')[0].LastIndexOf('.') + 1)..]);
                default:
                    reason = $"{e.Disposition.ToString().ToLowerInvariant()} type {n.Name}";
                    return null;
            }
        }
        if (n.Assembly == "nrtftree-library") { reason = $"pending D7: NRtfTree ({n.Name})"; return null; }
        if (n.Assembly.StartsWith("DevComponents."))
        {
            if (Substitutes.TryGetValue(n.Name, out var s)) return new NamedSig(s.Asm, s.Name);
            reason = $"no substitute for {n.Name}";
            return null;
        }
        if (n.Assembly == "System.Windows.Interactivity")
            return new NamedSig("Microsoft.Xaml.Behaviors", "Microsoft.Xaml.Behaviors." + n.Name["System.Windows.Interactivity.".Length..]);
        return n;
    }

    // ------------------------------------------------------------------ types

    private static TypeModel Shell(TypeModel t, string asm, string quireName, string project) => new()
    {
        Assembly = project,
        Project = project,
        FullName = quireName,
        Namespace = quireName.Split('+')[0][..quireName.Split('+')[0].LastIndexOf('.')],
        Name = SimpleName(quireName),
        DeclaringType = t.DeclaringType == null ? null : quireName[..quireName.LastIndexOf('+')],
        Kind = t.Kind,
        Access = t.Access,
        Abstract = t.Abstract,
        Sealed = t.Sealed,
        GenericParams = t.GenericParams,
        IsFlags = t.IsFlags,
        IsReadOnly = t.IsReadOnly,
        IsByRefLike = t.IsByRefLike,
        DefaultMember = t.DefaultMember,
        NativeSize = t.Kind == TypeKind.Struct && t.Fields.All(f => f.Static) ? t.NativeSize : null,
        EnumUnderlying = t.EnumUnderlying,
        MirrorOf = $"{t.FullName}, {asm}",
    };

    private static string SimpleName(string fullName)
    {
        string last = fullName.Split('+')[^1];
        return fullName.Contains('+') ? last : last[(last.LastIndexOf('.') + 1)..];
    }

    private void Fill(TypeModel src, TypeModel q, string asm)
    {
        string p = q.Project!;
        TypeSig? M(TypeSig s, string what)
        {
            var m = Map(s, p, out var reason);
            if (m == null) q.Notes.Add($"dropped {what}: {reason}");
            return m;
        }

        var baseType = src.BaseType == null ? null : M(src.BaseType, "base type");
        var interfaces = src.Interfaces.Select(i => M(i, $"interface {i}")).OfType<TypeSig>().ToImmutableArray();
        var generics = MapGenerics(src.GenericParams, p, q, "type");
        q.BaseType = baseType;
        q.Interfaces = interfaces;
        q.GenericParams = generics;

        // A field-like event's private backing field shares the event's name; the event declaration recreates it.
        var eventNames = src.Events.Select(e => e.Name).ToHashSet();
        foreach (var f in src.Fields)
        {
            if (f.Name.StartsWith('<') || f.Name.StartsWith('$')) continue;
            if (f.Access == Access.Private && eventNames.Contains(f.Name)) continue;
            if (M(f.Type, $"field {f.Name}") is { } ft)
                q.Fields.Add(f with { Name = Identifier.Encode(f.Name), Type = ft, Value = MapConstant(f.Value) });
        }

        if (src.Kind == TypeKind.Delegate)
        {
            if (src.Methods.FirstOrDefault(m => m.Name == "Invoke") is { } invoke && MapMethod(invoke, p, q) is { } mi)
                q.Methods.Add(mi);
            return;
        }

        foreach (var m in src.Methods)
        {
            if (!Wanted(m) || m.Name == ".cctor") continue;
            if (MapMethod(m, p, q) is { } mm) q.Methods.Add(mm);
        }
        if (src.Kind == TypeKind.Class && !src.IsStatic && !q.Methods.Any(m => m.Name == ".ctor"))
            q.Methods.Add(new MethodModel(".ctor", Access.Private, false, false, false, false, false, true,
                new NamedSig("System.Runtime", "System.Void"), [], [], [], false, false, false));

        foreach (var prop in src.Properties)
        {
            if (prop.Name.StartsWith('<')) continue;
            if (!Wanted(prop.Getter) && !Wanted(prop.Setter)) continue;
            var type = M(prop.Type, $"property {prop.Name}");
            var getter = prop.Getter == null ? null : MapMethod(prop.Getter, p, q);
            var setter = prop.Setter == null ? null : MapMethod(prop.Setter, p, q);
            if (type == null || (prop.Getter != null && getter == null) || (prop.Setter != null && setter == null)) continue;
            q.Properties.Add(new PropertyModel(ExplicitAware(prop.Name, getter ?? setter!), type, getter, setter,
                (getter?.Params ?? setter!.Params.RemoveAt(setter.Params.Length - 1))));
        }

        foreach (var ev in src.Events)
        {
            if (!Wanted(ev.Adder) && !Wanted(ev.Remover)) continue;
            var type = M(ev.Type, $"event {ev.Name}");
            var add = ev.Adder == null ? null : MapMethod(ev.Adder, p, q);
            var remove = ev.Remover == null ? null : MapMethod(ev.Remover, p, q);
            if (type == null || add == null || remove == null) continue;
            q.Events.Add(new EventModel(ExplicitAware(ev.Name, add), type, add, remove));
        }
    }

    /// <summary>Explicit implementations keep their dotted metadata name; ordinary members are encoded.</summary>
    private static string ExplicitAware(string name, MethodModel accessor) =>
        accessor.Access == Access.Private && !accessor.Implements.IsEmpty ? name : Identifier.Encode(name);

    /// <summary>
    /// Every member Mathcad declared, private ones included. Compiler-generated members (lambdas, local functions,
    /// iterator and async machinery, the Mpl compiler's gensyms) are left out: their names come from a compiler.
    /// </summary>
    private static bool Wanted(MethodModel? m) =>
        m != null && !m.Name.StartsWith('<') && !m.Name.StartsWith('$');

    private MethodModel? MapMethod(MethodModel m, string project, TypeModel q)
    {
        string what = $"method {m.Name}";
        var ret = Map(m.Return, project, out var reason);
        if (ret == null) { q.Notes.Add($"dropped {what}: {reason}"); return null; }
        var ps = ImmutableArray.CreateBuilder<ParamModel>(m.Params.Length);
        foreach (var prm in m.Params)
        {
            var t = Map(prm.Type, project, out reason);
            if (t == null) { q.Notes.Add($"dropped {what}: {reason}"); return null; }
            ps.Add(prm with { Type = t, Name = Identifier.Encode(prm.Name), Default = MapConstant(prm.Default) });
        }
        var impls = ImmutableArray.CreateBuilder<(TypeSig, string)>();
        foreach (var (iface, name) in m.Implements)
        {
            var mi = Map(iface, project, out reason);
            if (mi == null) { q.Notes.Add($"dropped {what}: implemented interface {reason}"); return null; }
            impls.Add((mi, name));
        }
        var generics = MapGenerics(m.GenericParams, project, q, what);
        bool isExplicit = m.Access == Access.Private && impls.Count > 0;
        return m with
        {
            Name = isExplicit || m.SpecialName ? m.Name : Identifier.Encode(m.Name),
            Return = ret, Params = ps.MoveToImmutable(), Implements = impls.ToImmutable(), GenericParams = generics,
        };
    }

    private ImmutableArray<GenericParam> MapGenerics(ImmutableArray<GenericParam> gps, string project, TypeModel q, string what) =>
        [.. gps.Select(g => g with
        {
            Name = Identifier.Encode(g.Name),
            Constraints = [.. g.Constraints.Select(c =>
            {
                var m = Map(c, project, out var reason);
                if (m == null) q.Notes.Add($"dropped constraint on {what}: {reason}");
                return m;
            }).OfType<TypeSig>()],
        })];

    /// <summary>String constants keep their value with the app name changed.</summary>
    private static object? MapConstant(object? value) => value is string s
        ? s.Replace("Mathcad Prime", "Quire").Replace("MathcadPrime", "Quire").Replace("Mathcad", "Quire").Replace("PTC", "Quire").Replace("Ptc", "Quire")
        : value;
}
