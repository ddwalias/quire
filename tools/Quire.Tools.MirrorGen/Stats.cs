namespace Quire.Tools.MirrorGen;

/// <summary>What signatures reference.</summary>
public static class Stats
{
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
