namespace Quire.Tools.MirrorGen;

/// <summary>Counts in-scope members and what their signatures reference, to size the emitter's problems.</summary>
public static class Stats
{
    public static void Run(Universe u, Rules rules)
    {
        int types = 0, unlisted = 0;
        var members = new Dictionary<string, int>();
        var refs = new Dictionary<string, int>();
        var refExamples = new Dictionary<string, SortedSet<string>>();
        var devc = new Dictionary<string, int>();
        var badNames = new List<string>();
        void Count(string key) => members[key] = members.GetValueOrDefault(key) + 1;

        foreach (var asm in u.Assemblies.Values)
            foreach (var t in asm.Types)
            {
                if (!rules.Scope.TryGetValue((asm.Name, t.FullName), out var e)) { if (!t.CompilerGenerated && !t.FullName.Contains('<')) unlisted++; continue; }
                if (e.Disposition != Disposition.Mirror) continue;
                types++;
                if (!System.Text.RegularExpressions.Regex.IsMatch(t.Name, @"^[A-Za-z_][A-Za-z0-9_]*(`\d+)?$")) badNames.Add("type " + t.FullName);
                foreach (var m in t.Methods.Where(m => m.Access != Access.Private && !m.IsConstructor && !m.SpecialName && !m.Name.StartsWith('<') && !m.Name.StartsWith('$')))
                    if (!System.Text.RegularExpressions.Regex.IsMatch(m.Name, @"^[A-Za-z_][A-Za-z0-9_]*$")) badNames.Add("method " + t.FullName + "::" + m.Name);
                var sigs = new List<TypeSig>();
                if (t.BaseType != null) sigs.Add(t.BaseType);
                sigs.AddRange(t.Interfaces);
                foreach (var f in t.Fields.Where(f => f.Access != Access.Private)) { Count("field"); sigs.Add(f.Type); }
                foreach (var m in t.Methods)
                {
                    if (m.Name.StartsWith('<')) continue;
                    if (m.Access == Access.Private && m.Implements.IsEmpty) continue;
                    Count(m.Access == Access.Private ? "explicit-impl" : m.IsConstructor ? "ctor" : "method");
                    sigs.Add(m.Return); sigs.AddRange(m.Params.Select(p => p.Type));
                }
                foreach (var p in t.Properties)
                {
                    var acc = new[] { p.Getter, p.Setter }.Where(a => a != null).ToList();
                    if (acc.All(a => a!.Access == Access.Private && a.Implements.IsEmpty)) continue;
                    Count("property"); sigs.Add(p.Type);
                }
                foreach (var ev in t.Events)
                {
                    if (ev.Adder is { Access: Access.Private, Implements.IsEmpty: true }) continue;
                    Count("event"); sigs.Add(ev.Type);
                }
                foreach (var s in sigs)
                    foreach (var n in Named(s))
                    {
                        string key;
                        if (u.IsMathcad(n.Assembly))
                            key = rules.Scope.TryGetValue((n.Assembly, n.Name), out var r) ? "mathcad:" + r.Disposition : "mathcad:unlisted";
                        else key = "ext:" + n.Assembly;
                        refs[key] = refs.GetValueOrDefault(key) + 1;
                        if (key.StartsWith("ext:DevComponents")) devc[n.Name] = devc.GetValueOrDefault(n.Name) + (s == t.BaseType ? 1000 : 1);
                        if (!refExamples.TryGetValue(key, out var ex)) refExamples[key] = ex = [];
                        if (ex.Count < 6) ex.Add(n.Name);
                    }
            }

        Console.WriteLine($"in-scope types loaded: {types} (unlisted non-generated types in metadata: {unlisted})");
        foreach (var (k, v) in members.OrderBy(k => k.Key)) Console.WriteLine($"  {k,-14} {v,8}");
        Console.WriteLine("DevComponents types (base-type uses x1000): " + string.Join(", ", devc.OrderByDescending(d => d.Value).Select(d => $"{d.Key}={d.Value}")));
        Console.WriteLine($"invalid C# names: {badNames.Count}: " + string.Join(" | ", badNames.Take(60)));
        Console.WriteLine("signature references:");
        foreach (var (k, v) in refs.OrderByDescending(k => k.Value))
            Console.WriteLine($"  {k,-50} {v,8}   e.g. {string.Join(" | ", refExamples[k].Take(3))}");
    }

    public static IEnumerable<NamedSig> Named(TypeSig s)
    {
        switch (s)
        {
            case NamedSig n:
                yield return n;
                foreach (var a in n.Args) foreach (var x in Named(a)) yield return x;
                break;
            case ArraySig a: foreach (var x in Named(a.Element)) yield return x; break;
            case PointerSig p: foreach (var x in Named(p.Element)) yield return x; break;
            case ByRefSig r: foreach (var x in Named(r.Element)) yield return x; break;
        }
    }
}

public static class ScopeDiff
{
    public static void Run(Universe u, Rules rules)
    {
        var loaded = new HashSet<(string, string)>(rules.Scope.Comparer);
        foreach (var a in u.Assemblies.Values) foreach (var t in a.Types) loaded.Add((a.Name, t.FullName));
        var fileNames = rules.Scope.Keys.Select(k => k.Asm).Distinct().ToList();
        Console.WriteLine("assembly name vs file name: " + string.Join(", ", u.Assemblies.Values.Where(a => !fileNames.Contains(a.Name)).Select(a => a.Name)));
        var missing = rules.Scope.Where(kv => kv.Value.Disposition == Disposition.Mirror && !loaded.Contains(kv.Key)).Select(kv => kv.Key).ToList();
        Console.WriteLine($"in type map but not matched: {missing.Count}");
        foreach (var g in missing.GroupBy(m => m.Item1).OrderByDescending(g => g.Count()).Take(12))
            Console.WriteLine($"  {g.Key}: {g.Count()}  e.g. {string.Join(" | ", g.Take(3).Select(x => x.Item2))}");
        var extra = loaded.Where(k => !rules.Scope.ContainsKey(k) && !k.Item2.Contains('<')).ToList();
        Console.WriteLine($"in metadata but not in type map: {extra.Count}");
        if (Environment.GetEnvironmentVariable("LIST_UNLISTED") is { } path)
            File.WriteAllLines(path, extra.Select(k => u.Assemblies[k.Item1].Types.Single(t => t.FullName == k.Item2))
                .Select(t => $"{t.Assembly}|{t.FullName}|{t.Kind}|{t.Access}"));
        foreach (var g in extra.GroupBy(m => m.Item1).OrderByDescending(g => g.Count()))
            Console.WriteLine($"  {g.Key}: {g.Count()}  e.g. {string.Join(" | ", g.Take(4).Select(x => x.Item2))}");
    }
}

public static class Dump
{
    public static void Run(Universe u, string asm, string type)
    {
        var t = u.Assemblies[asm].Types.Single(t => t.FullName == type);
        Console.WriteLine($"{t.Access} {t.Kind} {t.FullName} abstract={t.Abstract} sealed={t.Sealed} base={t.BaseType} : {string.Join(", ", t.Interfaces)}");
        foreach (var f in t.Fields) Console.WriteLine($"  field {f.Access} {(f.Static ? "static " : "")}{(f.Const ? "const " : "")}{f.Type} {f.Name} = {f.Value}");
        foreach (var p in t.Properties) Console.WriteLine($"  prop {p.Getter?.Access}/{p.Setter?.Access} {p.Type} {p.Name}");
        foreach (var e in t.Events) Console.WriteLine($"  event {e.Adder?.Access} {e.Type} {e.Name}");
        foreach (var m in t.Methods) Console.WriteLine($"  method {m.Access} {(m.Static ? "static " : "")}{(m.Virtual ? (m.NewSlot ? "virtual " : "override ") : "")}{m.Return} {m.Name}({string.Join(", ", m.Params.Select(p => p.Type + " " + p.Name))}){(m.Implements.IsEmpty ? "" : " implements " + string.Join(",", m.Implements.Select(i => i.Interface + "::" + i.Method)))}");
    }
}
