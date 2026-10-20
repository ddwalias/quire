using System.Text.RegularExpressions;
using Quire.Tools.MirrorGen;

sealed partial class Emitter
{
    /// <summary>A member's body for its evidence row: the expression, the statements, a constructor's initializer
    /// with its empty body, <c>{ }</c>, or a declaration without a body (a field, an auto-property, an event).</summary>
    internal static string Contract(List<string> lines)
    {
        string line = lines[0];
        int initializer = Math.Max(line.IndexOf(" : base(", StringComparison.Ordinal), line.IndexOf(" : this(", StringComparison.Ordinal));
        string code = lines.Count == 1 && line.Contains(" => ") ? line[(line.IndexOf(" => ", StringComparison.Ordinal) + 4)..].TrimEnd(';')
            : lines.Count == 1 && line.EndsWith(" { }") ? initializer >= 0 ? line[(initializer + 1)..] : "{ }"
            : lines.Count == 1 ? line.Trim() : string.Join(" ", lines.Skip(2).SkipLast(1).Select(l => l.Trim()));
        code = code.Replace("|", "\\|");
        return "`" + (code.Length > 140 ? code[..137] + "..." : code) + "`";
    }

    /// <summary>System namespaces first, then ordinal, as `dotnet format` sorts usings.</summary>
    private static readonly Comparer<string> Usings = Comparer<string>.Create((a, b) =>
        (a == "System" || a.StartsWith("System.")) != (b == "System" || b.StartsWith("System."))
            ? (a == "System" || a.StartsWith("System.") ? -1 : 1) : string.CompareOrdinal(a, b));

    [GeneratedRegex(@"^namespace\s+([\w.]+)\s*;", RegexOptions.Multiline)]
    private static partial Regex NamespaceLine();

    /// <summary>Where a member's lines are in a written file (1-based, inclusive).</summary>
    private sealed class Span(object member, int first, int last)
    {
        public object Member { get; } = member;
        public int First { get; set; } = first;
        public int Last { get; set; } = last;
    }

    /// <summary>The spans written so far, per file: a later insertion into the same file (another type it
    /// declares) moves the spans below it.</summary>
    private readonly Dictionary<string, List<Span>> _spans = [];

    private void Shift(string file, int index, int count)
    {
        foreach (var span in _spans.GetValueOrDefault(file) ?? [])
            if (span.First > index) { span.First += count; span.Last += count; }
    }

    /// <summary>Adds the members to the hand-written file that declares the top-level type, or a new file named
    /// after it, recording where each member's lines end up.</summary>
    private void WriteSource(string top, List<(string[] Path, string[] Kinds, List<string> Lines, object Report)> list, Names names)
    {
        var type = TopType(top);
        string projectDir = Path.Combine(repo, "src", project);
        string? file = Target(top);
        var declaration = Declaration(top);
        var usings = names.Usings.Append("Quire.Mirror").Distinct().Order(Usings).ToList();
        List<string> lines;
        int insert;
        if (file == null)
        {
            string relative = type.Namespace.StartsWith(project + ".") ? type.Namespace[(project.Length + 1)..]
                : type.Namespace.StartsWith("Quire.") ? type.Namespace["Quire.".Length..] : "";
            string directory = Path.Combine(projectDir, relative.Replace('.', Path.DirectorySeparatorChar));
            file = Path.Combine(directory, top + ".cs");
            if (File.Exists(file)) file = Path.Combine(directory, top + ".Members.cs");
            Directory.CreateDirectory(directory);
            string modifiers = Access(type.Access) + (type.IsStatic ? " static" : "");
            string typeArgs = type.GenericParams.Length == 0 ? "" : "<" + string.Join(", ", type.GenericParams.Select(g => g.Name)) + ">";
            lines = ["#nullable disable", .. usings.Select(u => $"using {u};"), "", $"namespace {type.Namespace};", "",
                $"{modifiers} partial {Keyword(type.Kind)} {top}{typeArgs}", "{", "}", ""];
            insert = lines.Count - 2;
        }
        else
        {
            lines = [.. File.ReadAllText(file).Split('\n')];
            foreach (string u in usings.Where(u => !lines.Contains($"using {u};")))
            {
                // Into the using block in order, or as a new block after the leading directives.
                int at = lines.FindIndex(l => l.StartsWith("using ") && Usings.Compare(l["using ".Length..].TrimEnd(';'), u) > 0);
                if (at < 0) at = lines.FindLastIndex(l => l.StartsWith("using ")) + 1;
                if (at == 0)
                {
                    at = lines.FindIndex(l => !l.StartsWith('#'));
                    lines.Insert(at, "");
                    Shift(file, at, 1);
                }
                lines.Insert(at, $"using {u};");
                Shift(file, at, 1);
            }
            int start = lines.FindIndex(l => declaration.IsMatch(l));
            insert = Closing(lines, start);
        }
        var block = new List<string>();
        var spans = new List<Span>();
        foreach (var group in list.GroupBy(m => string.Join("+", m.Path.Skip(1))))
        {
            var head = group.First();
            int nesting = head.Path.Length - 1;
            for (int d = 1; d <= nesting; d++)
            {
                if (d == 1) block.Add("");
                block.Add($"{new string(' ', 4 * d)}partial {head.Kinds[d]} {head.Path[d]}");
                block.Add($"{new string(' ', 4 * d)}{{");
            }
            string indent = new(' ', 4 * (nesting + 1));
            bool first = true;
            foreach (var m in group)
            {
                if (!first || nesting == 0) block.Add("");
                first = false;
                int from = insert + block.Count + 1;
                block.AddRange(m.Lines.Select(l => indent + l));
                spans.Add(new(m.Report, from, insert + block.Count));
            }
            for (int d = nesting; d >= 1; d--) block.Add($"{new string(' ', 4 * d)}}}");
        }
        // A new type body starts without the blank line.
        if (lines[insert - 1].Trim() == "{")
        {
            block.RemoveAt(0);
            foreach (var span in spans) { span.First--; span.Last--; }
        }
        Shift(file, insert, block.Count);
        lines.InsertRange(insert, block);
        File.WriteAllText(file, string.Join('\n', lines));
        if (!_spans.TryGetValue(file, out var all)) _spans[file] = all = [];
        all.AddRange(spans);
    }

    /// <summary>Each written file with its members' spans.</summary>
    private List<object> Written() =>
        [.. _spans.Select(f => (object)new
        {
            file = Path.GetRelativePath(repo, f.Key).Replace('\\', '/'),
            members = f.Value.Select(s => new { member = s.Member, first = s.First, last = s.Last }),
        })];

    private TypeModel TopType(string top) => quire.Model.Types.First(t => t.DeclaringType == null && Strip(t.Name) == top && t.MirrorOf != null);

    private readonly Dictionary<string, string?> _targets = [];

    /// <summary>The hand-written file that declares the top-level type at namespace level; null when none does.</summary>
    private string? Target(string top)
    {
        if (_targets.TryGetValue(top, out var cached)) return cached;
        var type = TopType(top);
        string projectDir = Path.Combine(repo, "src", project);
        var declaration = Declaration(top);
        return _targets[top] = Directory.GetFiles(projectDir, "*.cs", SearchOption.AllDirectories)
            .Where(f => !Regex.IsMatch(Path.GetRelativePath(projectDir, f).Replace('\\', '/'), @"^(Generated|bin|obj)/"))
            .FirstOrDefault(f => File.ReadAllText(f) is var s && declaration.IsMatch(s) && NamespaceLine().Match(s).Groups[1].Value == type.Namespace);
    }

    private static Regex Declaration(string top) => new($@"^(?:(?:public|internal|static|abstract|sealed|unsafe|readonly)\s+)*partial\s+(?:class|struct|interface|record)\s+{Regex.Escape(top)}(?:<[^>]+>)?\b", RegexOptions.Multiline);

    private static string Access(Access a) => a switch { Quire.Tools.MirrorGen.Access.Public => "public", _ => "internal" };
    private static string Keyword(TypeKind k) => k switch { TypeKind.Struct => "struct", TypeKind.Interface => "interface", _ => "class" };

    /// <summary>The index of the line closing the type declared at <paramref name="start"/>: braces counted outside
    /// comments, strings and character literals.</summary>
    private static int Closing(List<string> lines, int start)
    {
        int depth = 0;
        bool opened = false, block = false;
        for (int i = start; i < lines.Count; i++)
        {
            string l = lines[i];
            for (int c = 0; c < l.Length; c++)
            {
                if (block) { if (l[c] == '*' && c + 1 < l.Length && l[c + 1] == '/') { block = false; c++; } continue; }
                if (l[c] == '/' && c + 1 < l.Length && l[c + 1] == '/') break;
                if (l[c] == '/' && c + 1 < l.Length && l[c + 1] == '*') { block = true; c++; continue; }
                if (l[c] is '"' or '\'')
                {
                    char q = l[c];
                    bool verbatim = q == '"' && c > 0 && l[c - 1] == '@';
                    for (c++; c < l.Length && l[c] != q; c++) if (!verbatim && l[c] == '\\') c++;
                    continue;
                }
                if (l[c] == '{') { depth++; opened = true; }
                if (l[c] == '}' && --depth == 0 && opened) return i;
            }
        }
        throw new InvalidOperationException("unbalanced type declaration");
    }
}
