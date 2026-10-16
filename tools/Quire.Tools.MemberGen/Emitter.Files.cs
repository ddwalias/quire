using System.Text.RegularExpressions;
using Quire.Tools.MirrorGen;

sealed partial class Emitter
{
    /// <summary>The direct-comparison test class for a Mathcad type's mechanical members.</summary>
    public static string TestClass(string mathcadType) =>
        Regex.Replace(mathcadType.Split('+')[0], @"\W", "") + "MechanicalDifferentialTests";

    private static string TestName(Stub stub, Pair p) =>
        $"{Regex.Replace(stub.Name, @"\W", "")}{(p.Role is "method" ? "" : "_" + p.Role)}_{p.Mathcad!.Token:x8}";

    private static string Display(Stub stub, Pair p)
    {
        string nested = stub.Type.DeclaringType == null ? "" : stub.Type.Name.Split('`')[0] + ": ";
        static string Short(TypeSig t) => t switch
        {
            NamedSig n => Names.Keyword(n.Name).Split('.', '+')[^1].Split('`')[0],
            ArraySig a => Short(a.Element) + "[]",
            ByRefSig b => Short(b.Element) + "&",
            _ => t.ToString(),
        };
        return nested + (stub.Kind switch
        {
            "property" or "event" => $"{stub.Name} {p.Role}",
            _ => $"{stub.Name}({string.Join(", ", p.Method.Params.Select(x => Short(x.Type)))})",
        });
    }

    private static string Contract(List<string> lines)
    {
        string code = lines.Count == 1 && lines[0].Contains(" => ") ? lines[0][(lines[0].IndexOf(" => ", StringComparison.Ordinal) + 4)..].TrimEnd(';')
            : lines.Count == 1 ? lines[0].Trim() : string.Join(" ", lines.Skip(2).SkipLast(1).Select(l => l.Trim()));
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
            lines = ["#nullable disable", .. usings.Select(u => $"using {u};"), "", $"namespace {type.Namespace};", "",
                $"{modifiers} partial {Keyword(type.Kind)} {top}", "{", "}", ""];
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

    private static Regex Declaration(string top) => new($@"^(?:(?:public|internal|static|abstract|sealed|unsafe|readonly)\s+)*partial\s+(?:class|struct|interface|record)\s+{Regex.Escape(top)}\b", RegexOptions.Multiline);

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

    private void WriteTests(string outer, List<(string Test, int Token, string Member)> facts)
    {
        string cls = TestClass(outer);
        string path = Path.Combine(repo, "tests", "Quire.Differential.Tests", cls + ".cs");
        var type = quire.Model.Types.First(t => t.MirrorOf != null && t.MirrorOf.Split(',')[0].Trim() == outer);
        string typeName = "global::" + type.FullName.Split('`')[0];
        var methods = facts.Select(f => $"    [SkippableFact]\n    public void {f.Test}() => Mechanical.Compare(typeof({typeName}), 0x{f.Token:x8});").ToList();
        if (File.Exists(path))
        {
            var text = File.ReadAllText(path).TrimEnd();
            File.WriteAllText(path, text[..^1].TrimEnd() + "\n\n" + string.Join("\n\n", methods) + "\n}\n");
            return;
        }
        File.WriteAllText(path, $$"""
            namespace Quire.Differential.Tests;

            /// <summary>
            /// Direct comparisons of the <c>{{outer}}</c> members that MemberGen reconstructed mechanically
            /// (docs/mirror/evidence/{{outer}}.md): each compares one member on generated inputs (Mechanical).
            /// </summary>
            public class {{cls}}
            {
            {{string.Join("\n\n", methods)}}
            }

            """);
    }

    /// <summary>A table row's cells, split at unescaped pipes (index 0 is the empty text before the first).</summary>
    private static string[] Cells(string row) => Regex.Split(row, @"(?<!\\)\|");

    private void WriteEvidence(string outer, List<string> rows)
    {
        string path = Path.Combine(repo, "docs", "mirror", "evidence", outer + ".md");
        string cls = TestClass(outer);
        var original = world.Mathcad(quire.Model.Types.First(t => t.MirrorOf != null && t.MirrorOf.Split(',')[0].Trim() == outer).MirrorOf!.Split(',')[1].Trim())!;
        if (!File.Exists(path))
        {
            File.WriteAllText(path, $"""
                # {outer}

                - **Original:** `{original.Name}.dll` {original.Version}, SHA-256 `{original.Hash}`, MVID `{original.Mvid}`
                - **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
                - **Quire:** `src/{project}`; tests: `Quire.Differential.Tests.{cls}`

                | Member | Token | Contract | Confidence / limits | Status |
                |---|---|---|---|---|
                {string.Join("\n", rows)}

                ## Test cases

                **Observed** (`{cls}`): each member on generated inputs (Mechanical: zero, null and distinct values per
                parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

                """);
            return;
        }
        var lines = File.ReadAllText(path).Split('\n').ToList();
        int header = lines.FindIndex(l => l.StartsWith("| Member | Token"));
        if (header < 0)
        {
            lines.AddRange(["", "## MemberGen members", "", "| Member | Token | Contract | Confidence / limits | Status |", "|---|---|---|---|---|", .. rows]);
            header = lines.Count;
        }
        else
        {
            // Older records carry a normalized-IL column: a new row leaves it empty, a row for the same token is
            // replaced but keeps its value.
            bool normalized = lines[header].Count(c => c == '|') == 7;
            foreach (string row in rows)
            {
                string token = Cells(row)[2].Trim();
                int end = header + 1;
                while (end < lines.Count && lines[end].StartsWith('|')) end++;
                int same = lines.FindIndex(header, end - header, l => Cells(l).Skip(2).FirstOrDefault()?.Trim() == token);
                string il = same >= 0 && normalized ? Cells(lines[same])[3].Trim() : "—";
                string written = normalized ? row.Insert(row.IndexOf(" | MemberGen", StringComparison.Ordinal), $" | {il}") : row;
                if (same >= 0) lines[same] = written;
                else lines.Insert(end, written);
            }
        }
        string note = $"MemberGen members are compared by `Quire.Differential.Tests.{cls}`.";
        while (lines.Count > 0 && lines[^1].Trim() == "") lines.RemoveAt(lines.Count - 1);
        if (!lines.Contains(note)) lines.AddRange(["", note]);
        File.WriteAllText(path, string.Join('\n', lines).TrimEnd() + "\n");
    }
}
