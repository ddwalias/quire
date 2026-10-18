using Quire.Tools.MirrorGen;

/// <summary>
/// C# names for Quire types inside one file's namespace: keywords for the built-in types, simple names where the
/// namespace is enclosing or imported and nothing nearer or already imported shares the name, <c>global::</c>
/// otherwise. Collects the <c>using</c> directives the file needs.
/// </summary>
sealed class Names(string ns, World world)
{
    // The SDK's implicit global usings for a console or library project (ImplicitUsings in Directory.Build.props).
    private static readonly HashSet<string> Implicit = ["System", "System.Collections.Generic", "System.Linq",
        "System.Threading", "System.Threading.Tasks"];
    private static readonly Dictionary<string, string> Keywords = new()
    {
        ["System.Object"] = "object", ["System.String"] = "string", ["System.Boolean"] = "bool", ["System.Char"] = "char",
        ["System.SByte"] = "sbyte", ["System.Byte"] = "byte", ["System.Int16"] = "short", ["System.UInt16"] = "ushort",
        ["System.Int32"] = "int", ["System.UInt32"] = "uint", ["System.Int64"] = "long", ["System.UInt64"] = "ulong",
        ["System.Single"] = "float", ["System.Double"] = "double", ["System.Decimal"] = "decimal", ["System.Void"] = "void",
        ["System.IntPtr"] = "nint", ["System.UIntPtr"] = "nuint",
    };
    public SortedSet<string> Usings { get; } = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _simple = [];   // simple name → the imported full name it denotes

    /// <summary>Generic parameter names of the member being written, type's first.</summary>
    public string[] TypeParameters { get; set; } = [];
    public string[] MethodParameters { get; set; } = [];
    /// <summary>The type being written and its declaring types: their nested types go by their own names.</summary>
    public string[] Enclosing { get; set; } = [];

    public static string Keyword(string name) => Keywords.GetValueOrDefault(name) ?? name;

    public string Type(TypeSig sig) => sig switch
    {
        NamedSig n when Keywords.TryGetValue(n.Name, out var k) => k,
        NamedSig { Name: "System.Nullable`1", Args: [var inner] } => Type(inner) + "?",
        NamedSig n => Named(n),
        ArraySig a => Type(a.Element) + "[" + new string(',', Math.Max(0, a.Rank - 1)) + "]",
        GenericParamSig g => (g.OfMethod ? MethodParameters : TypeParameters).ElementAtOrDefault(g.Index)
            ?? throw new NotSupportedException("unknown generic parameter"),
        _ => throw new NotSupportedException($"type {sig}"),
    };

    private string Named(NamedSig n)
    {
        // Outer`1+Inner: generic arguments go to the segments in order of their arity.
        var segments = n.Name.Split('+');
        int lastDot = segments[0].LastIndexOf('.');
        string typeNs = lastDot < 0 ? "" : segments[0][..lastDot];
        segments[0] = segments[0][(lastDot + 1)..];
        var args = new Queue<TypeSig>(n.Args);
        var parts = segments.Select(s =>
        {
            int tick = s.IndexOf('`');
            if (tick < 0) return Identifier(s);
            int arity = int.Parse(s[(tick + 1)..]);
            return Identifier(s[..tick]) + "<" + string.Join(", ", Enumerable.Range(0, arity).Select(_ => Type(args.Dequeue()))) + ">";
        }).ToList();
        string outer = segments[0].Split('`')[0];
        string full = typeNs.Length == 0 ? outer : typeNs + "." + outer;
        foreach (string enclosing in Enclosing.Where(e => n.Name.StartsWith(e + "+", StringComparison.Ordinal)).OrderByDescending(e => e.Length).Take(1))
            return string.Join(".", parts.Skip(enclosing.Split('+').Length));
        string rest = string.Join(".", parts.Skip(1).Prepend(""));
        if (Simple(typeNs, outer, full)) return parts[0] + rest;
        return "global::" + (typeNs.Length == 0 ? "" : typeNs + ".") + parts[0] + rest;
    }

    /// <summary>Whether the outermost type can go by its simple name here, importing its namespace if needed.</summary>
    private bool Simple(string typeNs, string name, string full)
    {
        while (typeNs.Length > 0 && world.KnownTypes.Contains(typeNs))
            typeNs = typeNs.Contains('.') ? typeNs[..typeNs.LastIndexOf('.')] : "";
        // A nearer namespace's type of the same name would win over an enclosing or imported one.
        for (string scope = ns; scope.Length > 0; scope = scope.Contains('.') ? scope[..scope.LastIndexOf('.')] : "")
        {
            if (scope == typeNs) return true;
            if (Declares(scope, name)) return false;
        }
        if (_simple.TryGetValue(name, out var taken)) return taken == full;
        // Another imported namespace declaring the name would make it ambiguous; so would importing one that
        // declares a name already in use.
        bool imported = typeNs.Length == 0 || Implicit.Contains(typeNs) || Usings.Contains(typeNs);
        if (Implicit.Concat(Usings).Any(u => u != typeNs && Declares(u, name))) return false;
        if (!imported && _simple.Keys.Any(s => Declares(typeNs, s) && _simple[s] != typeNs + "." + s)) return false;
        _simple[name] = full;
        if (!imported) Usings.Add(typeNs);
        return true;
    }

    private bool Declares(string scope, string name) => world.KnownTypes.Contains(scope + "." + name);

    /// <summary>Namespaces the file already imports.</summary>
    public void Import(IEnumerable<string> namespaces)
    {
        foreach (string u in namespaces) Usings.Add(u);
    }

    /// <summary>An identifier as C# source spells it: keywords escaped.</summary>
    public static string Identifier(string name) => Reserved.Contains(name) ? "@" + name : name;

    private static readonly HashSet<string> Reserved = [.. """
        abstract as base bool break byte case catch char checked class const continue decimal default delegate do double
        else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal
        is lock long namespace new null object operator out override params private protected public readonly ref return
        sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked
        unsafe ushort using virtual void volatile while
        """.Split((char[])[' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)];
}
