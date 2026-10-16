using System.Reflection.Metadata.Ecma335;
using Quire.Tools.MirrorGen;

/// <summary>One method of a stub member with its Mathcad counterpart (null when Mathcad has none).</summary>
sealed record Pair(string Role, MethodModel Method, RawMethod Quire, RawMethod? Mathcad);

/// <summary>A <c>[MirrorStub]</c> member of a mirrored type: a field, method, constructor, property or event.</summary>
sealed record Stub(string Kind, string Name, TypeModel Type, IHasStatus Member, List<Pair> Methods, string MathcadType, Artifact? Original)
{
    /// <summary>A method or constructor by its MethodAudit key (which starts with its name), other members by name.</summary>
    public string Id => $"{Type.FullName}::" + (Methods.Count == 1 && Kind is "method" or "constructor" ? Methods[0].Quire.Key : Name);
}

/// <summary>A stub's reconstruction: the survey category, and the C# for each accessor (or the method) when
/// mechanical; <see cref="Category"/> names the reason otherwise.</summary>
sealed record Plan(Stub Stub, string Category, bool Mechanical, Dictionary<string, (Body Body, bool Constructor)> Bodies);

static class Stubs
{
    /// <summary>The stub members of a built Quire assembly, each with its Mathcad counterpart methods.</summary>
    public static List<Stub> Find(Artifact quire, World world)
    {
        var stubs = new List<Stub>();
        foreach (var t in quire.Model.Types.Where(t => t.MirrorOf != null))
        {
            string[] binding = t.MirrorOf!.Split(',').Select(s => s.Trim()).ToArray();
            var original = world.Mathcad(binding[1]);
            Pair Pair(string role, MethodModel m)
            {
                string key = Key(world, m, false), mapped = Key(world, m, true);
                var q = quire.Methods.Single(r => r.Owner == t.FullName && r.Key == key);
                // C++/CLI overloads can differ only in modifiers the key leaves out: such a counterpart is ambiguous.
                var counterparts = original?.Methods.Where(r => r.Owner == binding[0] && r.Key == mapped).Take(2).ToList();
                return new(role, m, q, counterparts is [var only] ? only : null);
            }
            Stub Make(string kind, string name, IHasStatus member, List<Pair> methods) => new(kind, name, t, member, methods, binding[0], original);
            foreach (var f in t.Fields.Where(f => f.Stub)) stubs.Add(Make(f.Const ? "constant" : "field", f.Name, f, []));
            foreach (var m in t.Methods.Where(m => m.Stub))
                stubs.Add(Make(m.IsConstructor ? "constructor" : "method", m.Name, m, [Pair("method", m)]));
            foreach (var p in t.Properties.Where(p => p.Stub))
                stubs.Add(Make("property", p.Name, p, [.. new[] { ("get", p.Getter), ("set", p.Setter) }.Where(x => x.Item2 != null).Select(x => Pair(x.Item1, x.Item2!))]));
            foreach (var e in t.Events.Where(e => e.Stub))
                stubs.Add(Make("event", e.Name, e, [.. new[] { ("add", e.Adder), ("remove", e.Remover) }.Where(x => x.Item2 != null).Select(x => Pair(x.Item1, x.Item2!))]));
        }
        return stubs;
    }

    /// <summary>MethodAudit's key for a method, in Quire's names or mapped to Mathcad's.</summary>
    public static string Key(World world, MethodModel m, bool map) => Signatures.Method(m.Name, m.GenericParams.Length,
        m.Params.Select(p => map ? world.ToMathcad(p.Type) : p.Type), map ? world.ToMathcad(m.Return) : m.Return, m.Static);

    /// <summary>Classifies a stub's Mathcad bodies. Fields and constants have none: they are reported, not planned.</summary>
    public static Plan Classify(Stub stub)
    {
        var bodies = new Dictionary<string, (Body, bool)>();
        if (stub.Kind is "field" or "constant") return new(stub, stub.Kind, false, bodies);
        if (stub.Original == null) return new(stub, "original assembly unreadable", false, bodies);
        if (stub.Methods.Any(p => p.Mathcad == null)) return new(stub, "no Mathcad counterpart", false, bodies);
        var a = stub.Original;
        bool generated = stub.Methods.All(p => Decompiler.CompilerGenerated(a, Handle(p.Mathcad!)));
        if (stub.Kind == "event" && generated) return new(stub, "field-like event", true, bodies);
        try
        {
            foreach (var p in stub.Methods) bodies[p.Role] = (Decompiler.Decompile(a, Handle(p.Mathcad!)), p.Method.IsConstructor);
        }
        catch (NotMechanical e) { return new(stub, e.Message, false, bodies); }
        static bool Backing(Statement s) => s is Return { Value: Field r } && r.F.Name.EndsWith(">k__BackingField")
            || s is Assign { Target: Field w } && w.F.Name.EndsWith(">k__BackingField");
        bool auto = stub.Kind == "property" && generated && bodies.Values.All(b => b.Item1.Statements.Any(Backing));
        string category = auto ? "auto-property" : string.Join(" + ", bodies.Values.Select(b => b.Item1.Kind(b.Item2)).Distinct());
        return new(stub, category, true, bodies);
    }

    public static System.Reflection.Metadata.MethodDefinitionHandle Handle(RawMethod m) => MetadataTokens.MethodDefinitionHandle(m.Token);
}
