using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Quire.Tools.MirrorGen;

/// <summary>
/// Phase 0 step 5 (docs/mirror/PLAN.md §1.3, §3): inventories the Mathcad components outside .NET metadata. Native
/// export tables, Lisp/Prolog definition names and arities, Axiom package and operation names, and the user-function
/// C API are read as data. No method bodies, clause bodies or source text are copied.
/// </summary>
public static class NativeInventory
{
    static readonly string[] MathcadDlls =
    [
        "efistatsci_.dll", "efisymbolics-umgd.dll", "engutils.dll", "mcaduser.dll", "mctranslator.dll",
        "mshex.dll", "msmpl.dll", "mspcons.dll", "msprolog.dll", "utl.dll", "creo3dfit.dll",
    ];
    const uint UndnameComplete = 0x0000, UndnameNameOnly = 0x1000;

    sealed record Export(int Ordinal, string? Name, string? Forwarder, string? Undecorated, string? QualifiedName);
    sealed record DllExports(string Dll, List<Export> Exports, int TotalInImage, string? Note);
    sealed record LispString(string Value);

    public static void Run(string installDir, string outDir)
    {
        Directory.CreateDirectory(outDir);
        var dlls = new List<DllExports>();
        foreach (var dll in MathcadDlls)
        {
            string path = Path.Combine(installDir, dll);
            if (!File.Exists(path)) { dlls.Add(new(dll, [], 0, "file not found")); continue; }
            var all = ReadExports(path);
            dlls.Add(new(dll, all, all.Count, null));
        }

        var summary = new List<string>();
        WriteExports(dlls, Path.Combine(outDir, "exports.txt"));
        WriteClasses(dlls, Path.Combine(outDir, "classes.txt"), summary);
        WriteScripts(installDir, Path.Combine(outDir, "scripts.txt"), summary);
        WriteSummary(dlls, summary, Path.Combine(outDir, "summary.md"));
        Console.WriteLine($"native inventory written to {outDir}");
    }

    // ---------------------------------------------------------------- PE export directory

    static List<Export> ReadExports(string path)
    {
        using var pe = new PEReader(File.OpenRead(path));
        var dir = pe.PEHeaders.PEHeader!.ExportTableDirectory;
        var result = new List<Export>();
        if (dir.Size == 0) return result;

        // IMAGE_EXPORT_DIRECTORY: Characteristics, TimeDateStamp, Major/MinorVersion, Name, Base, NumberOfFunctions,
        // NumberOfNames, AddressOfFunctions, AddressOfNames, AddressOfNameOrdinals.
        var r = At(pe, dir.RelativeVirtualAddress);
        r.Offset = 16;
        int ordinalBase = r.ReadInt32(), functionCount = r.ReadInt32(), nameCount = r.ReadInt32();
        int functionsRva = r.ReadInt32(), namesRva = r.ReadInt32(), ordinalsRva = r.ReadInt32();

        var names = new Dictionary<int, List<string>>();
        var nameReader = At(pe, namesRva);
        var ordinalReader = At(pe, ordinalsRva);
        for (int i = 0; i < nameCount; i++)
        {
            string name = CString(pe, nameReader.ReadInt32());
            int index = ordinalReader.ReadUInt16();
            if (!names.TryGetValue(index, out var list)) names[index] = list = [];
            list.Add(name);
        }

        var functionReader = At(pe, functionsRva);
        for (int i = 0; i < functionCount; i++)
        {
            int rva = functionReader.ReadInt32();
            if (rva == 0) continue;
            bool forwarded = rva >= dir.RelativeVirtualAddress && rva < dir.RelativeVirtualAddress + dir.Size;
            string? forwarder = forwarded ? CString(pe, rva) : null;
            if (names.TryGetValue(i, out var list))
                foreach (var name in list)
                    result.Add(new(ordinalBase + i, name, forwarder,
                        name.StartsWith('?') ? Undecorate(name, UndnameComplete) : null,
                        name.StartsWith('?') ? Undecorate(name, UndnameNameOnly) : null));
            else
                result.Add(new(ordinalBase + i, null, forwarder, null, null));
        }
        return result;
    }

    static BlobReader At(PEReader pe, int rva) => pe.GetSectionData(rva).GetReader();

    static string CString(PEReader pe, int rva)
    {
        var r = At(pe, rva);
        var bytes = new List<byte>();
        while (r.RemainingBytes > 0)
        {
            byte b = r.ReadByte();
            if (b == 0) break;
            bytes.Add(b);
        }
        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    [DllImport("dbghelp.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
    static extern uint UnDecorateSymbolName(string name, byte[] outputString, uint maxStringLength, uint flags);

    static string? Undecorate(string name, uint flags)
    {
        if (!OperatingSystem.IsWindows()) return null;
        var buffer = new byte[8192];
        uint length = UnDecorateSymbolName(name, buffer, (uint)buffer.Length, flags);
        string text = Encoding.ASCII.GetString(buffer, 0, (int)length);
        return length == 0 || text == name ? null : text;
    }

    static IEnumerable<Export> SortedByName(IEnumerable<Export> exports) =>
        exports.OrderBy(e => e.Name == null).ThenBy(e => e.Name, StringComparer.Ordinal).ThenBy(e => e.Ordinal);

    static void WriteExports(List<DllExports> dlls, string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Native export tables of the Mathcad DLLs (docs/mirror/PLAN.md §1.3, §3 step 5).");
        sb.AppendLine("# Generated by `MirrorGen native-inventory` from each PE export directory. Data only: names and ordinals.");
        sb.AppendLine("# Line format: @ordinal  exported-name; MSVC-decorated names are followed by \"=> \" and the undecorated");
        sb.AppendLine("# C++ form (dbghelp UnDecorateSymbolName, UNDNAME_COMPLETE). \"-> \" marks a forwarder.");
        foreach (var d in dlls)
        {
            sb.AppendLine();
            string count = d.Note == null ? $"{d.Exports.Count} exports"
                : $"0 exports ({d.Note})";
            sb.AppendLine($"== {d.Dll}: {count}");
            foreach (var e in SortedByName(d.Exports))
            {
                sb.AppendLine($"  @{e.Ordinal,-5} {e.Name ?? "(exported by ordinal only)"}" + (e.Forwarder != null ? $"  -> {e.Forwarder}" : ""));
                if (e.Undecorated != null) sb.AppendLine($"         => {e.Undecorated}");
                else if (e.Name?.StartsWith('?') == true) sb.AppendLine("         => (could not be undecorated)");
            }
        }
        File.WriteAllText(path, sb.ToString());
    }

    // ---------------------------------------------------------------- C++ classes from undecorated names

    /// <summary>Splits a qualified C++ name at its last top-level "::" (outside template arguments and quoted parts).</summary>
    static (string? Qualifier, string Member) SplitQualified(string name)
    {
        int op = name.IndexOf("::operator", StringComparison.Ordinal);
        int depth = 0, last = -1;
        bool quoted = false;
        for (int i = 0; i < name.Length; i++)
        {
            if (op >= 0 && i == op) { last = i; break; }
            char c = name[i];
            if (c == '`') quoted = true;
            else if (c == '\'') quoted = false;
            else if (quoted) continue;
            else if (c is '<' or '(') depth++;
            else if (c is '>' or ')') depth--;
            else if (c == ':' && depth == 0 && i + 1 < name.Length && name[i + 1] == ':') { last = i; i++; }
        }
        return last < 0 ? (null, name) : (name[..last], name[(last + 2)..]);
    }

    static void WriteClasses(List<DllExports> dlls, string path, List<string> summary)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Exported C++ surface of the Mathcad DLLs, derived from the undecorated export names in exports.txt.");
        sb.AppendLine("# A class is the qualifier before the last \"::\" of an exported member (an export whose undecorated form");
        sb.AppendLine("# carries public:/protected:/private:, or a class-scoped compiler symbol such as `vftable'). Qualified exports");
        sb.AppendLine("# without an access specifier are namespace-scope functions or data. Signatures are as undecorated; no bodies.");
        int totalClasses = 0, stdClasses = 0, totalMembers = 0, totalNamespaceFns = 0, totalGlobalFns = 0, totalPlain = 0;
        foreach (var d in dlls)
        {
            var classes = new SortedDictionary<string, List<Export>>(StringComparer.Ordinal);
            var namespaces = new SortedDictionary<string, List<Export>>(StringComparer.Ordinal);
            var globals = new List<Export>();
            int plain = 0;
            foreach (var e in d.Exports)
            {
                if (e.Name == null || !e.Name.StartsWith('?') || e.Undecorated == null) { if (e.Name != null) plain++; continue; }
                var (qualifier, _) = SplitQualified(e.QualifiedName ?? e.Undecorated);
                bool member = Regex.IsMatch(e.Undecorated, @"^(public|protected|private):") || (e.QualifiedName ?? "").Contains("::`");
                var target = qualifier == null ? null : member ? classes : namespaces;
                if (target == null) { globals.Add(e); continue; }
                if (!target.TryGetValue(qualifier!, out var list)) target[qualifier!] = list = [];
                list.Add(e);
            }
            int members = classes.Values.Sum(l => l.Count);
            totalClasses += classes.Count; stdClasses += classes.Keys.Count(k => k.StartsWith("std::")); totalMembers += members; totalPlain += plain;
            totalNamespaceFns += namespaces.Values.Sum(l => l.Count); totalGlobalFns += globals.Count;

            sb.AppendLine();
            sb.AppendLine($"== {d.Dll}: {classes.Count} classes, {members} member exports, {namespaces.Values.Sum(l => l.Count)} namespace-scope, " +
                          $"{globals.Count} global C++, {plain} undecorated (C) exports" + (d.Note != null ? $" ({d.Note})" : ""));
            foreach (var (name, list) in classes)
            {
                sb.AppendLine($"class {name}  ({list.Count} members)" + (name.StartsWith("std::") ? "  [C++ standard library instantiation]" : ""));
                foreach (var e in list.OrderBy(e => e.QualifiedName, StringComparer.Ordinal).ThenBy(e => e.Undecorated, StringComparer.Ordinal))
                    sb.AppendLine($"    {e.Undecorated}");
            }
            foreach (var (name, list) in namespaces)
            {
                sb.AppendLine($"namespace {name}  ({list.Count} exports)");
                foreach (var e in list.OrderBy(e => e.Undecorated, StringComparer.Ordinal)) sb.AppendLine($"    {e.Undecorated}");
            }
            if (globals.Count > 0)
            {
                sb.AppendLine($"(global scope)  ({globals.Count} exports)");
                foreach (var e in globals.OrderBy(e => e.Undecorated, StringComparer.Ordinal)) sb.AppendLine($"    {e.Undecorated}");
            }
        }
        File.WriteAllText(path, sb.ToString());
        summary.Add($"| classes.txt | {totalClasses} exported classes ({stdClasses} of them C++ standard library instantiations) with {totalMembers} member exports; {totalNamespaceFns} namespace-scope and " +
                    $"{totalGlobalFns} global C++ exports; {totalPlain} undecorated (C) exports |");
    }

    // ---------------------------------------------------------------- S-expression reader (Lisp, Lisp-syntax Prolog, Axiom KAF)

    static bool IsDelimiter(char c, bool lisp) =>
        char.IsWhiteSpace(c) || c is '(' or ')' or '"' or ';' || (lisp ? c is '\'' or '`' or ',' : c == '|');

    static void SkipAtmosphere(string s, ref int i)
    {
        while (i < s.Length)
        {
            if (char.IsWhiteSpace(s[i]) || s[i] == '\uFEFF') i++;
            else if (s[i] == ';') { while (i < s.Length && s[i] != '\n') i++; }
            else if (s[i] == '#' && i + 1 < s.Length && s[i + 1] == '|')
            {
                int depth = 1;
                i += 2;
                while (i < s.Length && depth > 0)
                {
                    if (s[i] == '|' && i + 1 < s.Length && s[i + 1] == '#') { depth--; i += 2; }
                    else if (s[i] == '#' && i + 1 < s.Length && s[i + 1] == '|') { depth++; i += 2; }
                    else i++;
                }
            }
            else break;
        }
    }

    /// <summary>
    /// Reads one datum: a string atom, a <see cref="LispString"/>, or a List&lt;object&gt;. Returns null at end of input or
    /// on a stray ")". With <paramref name="lisp"/>, |...| is a multiple escape and ' ` , are reader prefixes; otherwise
    /// (Mathcad's Lisp-syntax Prolog) "|" is its own token and ' ` , are ordinary atom characters, as in the variable $y'.
    /// </summary>
    static object? ReadDatum(string s, ref int i, bool lisp)
    {
        SkipAtmosphere(s, ref i);
        if (i >= s.Length) return null;
        char c = s[i];
        char next = i + 1 < s.Length ? s[i + 1] : '\0';
        if (lisp && (c is '\'' or '`' or ',' || c == '#' && next == '\'') && ReaderPrefixHasNoOperand(s, i + (c == '#' || next == '@' ? 2 : 1)))
        {
            i++;
            return c.ToString();
        }
        switch (lisp ? c : c is '(' or ')' or '"' or '|' ? c : '\0')
        {
            case '(':
            {
                i++;
                var list = new List<object>();
                while (true)
                {
                    SkipAtmosphere(s, ref i);
                    if (i >= s.Length) return list;
                    if (s[i] == ')') { i++; return list; }
                    if (ReadDatum(s, ref i, lisp) is { } d) list.Add(d);
                }
            }
            case ')':
                i++;
                return null;
            case '"':
            {
                var sb = new StringBuilder();
                for (i++; i < s.Length && s[i] != '"'; i++)
                {
                    if (s[i] == '\\' && i + 1 < s.Length) i++;
                    sb.Append(s[i]);
                }
                i++;
                return new LispString(sb.ToString());
            }
            case '\'': i++; return new List<object> { "quote", ReadDatum(s, ref i, lisp) ?? "" };
            case '`': i++; return new List<object> { "quasiquote", ReadDatum(s, ref i, lisp) ?? "" };
            case ',':
                i += next == '@' ? 2 : 1;
                return new List<object> { "unquote", ReadDatum(s, ref i, lisp) ?? "" };
            case '#' when next == '\'':
                i += 2;
                return new List<object> { "function", ReadDatum(s, ref i, lisp) ?? "" };
            case '#' when next == '(':
                i++;
                return new List<object> { "vector", ReadDatum(s, ref i, lisp) ?? "" };
            case '#' when next == '\\':
            {
                int start = i;
                i += 3; // the character itself may be a delimiter, e.g. #\(
                while (i < s.Length && !IsDelimiter(s[i], lisp)) i++;
                return s[start..Math.Min(i, s.Length)];
            }
            case '|' when !lisp:
                i++;
                return "|";
        }

        var atom = new StringBuilder();
        bool escaped = false;
        while (i < s.Length && (escaped || !IsDelimiter(s[i], lisp)))
        {
            if (lisp && s[i] == '|') { escaped = !escaped; i++; continue; }
            if (lisp && !escaped && s[i] == '\\' && i + 1 < s.Length) i++;
            atom.Append(s[i++]);
        }
        return atom.ToString();
    }

    static bool ReaderPrefixHasNoOperand(string s, int i)
    {
        SkipAtmosphere(s, ref i);
        return i >= s.Length || s[i] == ')';
    }

    static List<object> ReadAll(string s, bool lisp)
    {
        var forms = new List<object>();
        int i = 0;
        while (i < s.Length)
            if (ReadDatum(s, ref i, lisp) is { } d) forms.Add(d);
        return forms;
    }

    static string Render(object? x) => x switch
    {
        List<object> l => "(" + string.Join(" ", l.Select(Render)) + ")",
        LispString => "\"…\"",
        _ => x?.ToString() ?? "",
    };

    // ---------------------------------------------------------------- scripts.txt

    static void WriteScripts(string installDir, string path, List<string> summary)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Mathcad script and SDK structure (docs/mirror/PLAN.md §1.3, §3 step 5).");
        sb.AppendLine("# Names, arities and declared signatures only, read as data. No source text, bodies or comments are copied.");
        WriteLisp(installDir, sb, summary);
        WriteProlog(installDir, sb, summary);
        WriteAxiom(installDir, sb, summary);
        WriteHeader(installDir, sb, summary);
        File.WriteAllText(path, sb.ToString());
    }

    static string LambdaListSummary(object? lambdaList)
    {
        if (lambdaList is string nil && nil.Equals("nil", StringComparison.OrdinalIgnoreCase)) return "required 0";
        if (lambdaList is not List<object> items) return "lambda list not a list";
        int required = 0;
        bool inRequired = true;
        var keywords = new List<string>();
        for (int k = 0; k < items.Count; k++)
        {
            if (items[k] is string s && s.StartsWith('&'))
            {
                string kw = s.ToLowerInvariant();
                if (kw is "&whole" or "&environment") { k++; continue; }
                inRequired = false;
                if (!keywords.Contains(kw)) keywords.Add(kw);
            }
            else if (items[k] is ".") { inRequired = false; keywords.Add("&rest (dotted)"); k++; }
            else if (inRequired) required++;
        }
        return $"required {required}" + (keywords.Count > 0 ? ", " + string.Join(", ", keywords) : "");
    }

    static void WriteLisp(string installDir, StringBuilder sb, List<string> summary)
    {
        string dir = Path.Combine(installDir, "lisp");
        sb.AppendLine();
        sb.AppendLine("==== lisp/*.lisp: depth-0 definition forms (kind, name; required-parameter count and lambda-list keywords");
        sb.AppendLine("==== for defun/defmacro), plus package forms. Definitions nested inside other forms are not recorded.");
        var kinds = new SortedDictionary<string, int>(StringComparer.Ordinal);
        var files = Directory.Exists(dir) ? Directory.GetFiles(dir, "*.lisp").OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToArray() : [];
        foreach (var file in files)
        {
            var lines = new List<string>();
            foreach (var form in ReadAll(File.ReadAllText(file), lisp: true))
            {
                if (form is not List<object> { Count: > 0 } l || l[0] is not string head) continue;
                string h = head.ToLowerInvariant();
                if (h is "in-package" or "use-package" or "export")
                {
                    var symbols = l.Skip(1).SelectMany(Atoms).Where(a => a != "quote").ToList();
                    lines.Add($"  {h,-18} {string.Join(" ", symbols)}");
                    kinds[h] = kinds.GetValueOrDefault(h) + 1;
                    continue;
                }
                if (!h.StartsWith("def") || l.Count < 2) continue;
                string name = Render(l[1]);
                string detail = h is "defun" or "defmacro" && l.Count > 2 ? "  " + LambdaListSummary(l[2]) : "";
                lines.Add($"  {h,-18} {name}{detail}");
                kinds[h] = kinds.GetValueOrDefault(h) + 1;
            }
            sb.AppendLine();
            sb.AppendLine($"== lisp/{Path.GetFileName(file)}: {lines.Count} entries");
            foreach (var line in lines) sb.AppendLine(line);
        }
        summary.Add($"| scripts.txt: lisp | {files.Length} files; " + string.Join(", ", kinds.Select(k => $"{k.Value} {k.Key}")) + " |");
    }

    static IEnumerable<string> Atoms(object x) => x switch
    {
        List<object> l => l.SelectMany(Atoms),
        string s => [s],
        LispString str => [$"\"{str.Value}\""],
        _ => [],
    };

    static void WriteProlog(string installDir, StringBuilder sb, List<string> summary)
    {
        string dir = Path.Combine(installDir, "prolog");
        sb.AppendLine();
        sb.AppendLine("==== prolog/*.prolog: Mathcad's Prolog uses Lisp syntax, not Edinburgh syntax. A clause is ((name arg...) goal...),");
        sb.AppendLine("==== variables are $-prefixed, \";\" starts a comment, and directives are (!name arg...). So there are no \":-\"");
        sb.AppendLine("==== module/dynamic directives; predicates are recorded as name/arity (clause count) per file, deduplicated,");
        sb.AppendLine("==== with \"+tail\" when the head has a \"|\" tail. Directives are recorded by name with their literal file arguments.");
        int predicates = 0, clauses = 0, directives = 0, unrecognised = 0;
        var files = Directory.Exists(dir) ? Directory.GetFiles(dir, "*.prolog").OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToArray() : [];
        foreach (var file in files)
        {
            var preds = new SortedDictionary<string, int>(StringComparer.Ordinal);
            var dirs = new List<string>();
            int other = 0;
            foreach (var form in ReadAll(File.ReadAllText(file), lisp: false))
            {
                if (form is List<object> { Count: > 0 } l && l[0] is List<object> { Count: > 0 } headList && headList[0] is string name)
                {
                    int bar = headList.IndexOf("|");
                    string key = bar < 0 ? $"{name}/{headList.Count - 1}" : $"{name}/{bar - 1}+tail";
                    preds[key] = preds.GetValueOrDefault(key) + 1;
                    clauses++;
                }
                else if (form is List<object> { Count: > 0 } d && d[0] is string directive)
                    dirs.Add($"  directive {directive}/{d.Count - 1}" + string.Concat(d.Skip(1).OfType<LispString>().Select(a => $" \"{a.Value}\"")));
                else
                    other++;
            }
            predicates += preds.Count; directives += dirs.Count; unrecognised += other;
            sb.AppendLine();
            sb.AppendLine($"== prolog/{Path.GetFileName(file)}: {preds.Count} predicates, {preds.Values.Sum()} clauses, {dirs.Count} directives" +
                          (other > 0 ? $", {other} unrecognised top-level data" : ""));
            foreach (var line in dirs) sb.AppendLine(line);
            foreach (var (key, count) in preds) sb.AppendLine($"  {key}  ({count})");
        }
        summary.Add($"| scripts.txt: prolog | {files.Length} files; {predicates} predicates (name/arity per file), {clauses} clauses, " +
                    $"{directives} directives, {unrecognised} unrecognised top-level data |");
    }

    static string RenderAxiomType(object? x) => x switch
    {
        List<object> { Count: 3 } l when l[0] is ":" => $"{RenderAxiomType(l[1])}: {RenderAxiomType(l[2])}",
        List<object> { Count: > 0 } l => RenderAxiomType(l[0]) + (l.Count > 1 ? "(" + string.Join(", ", l.Skip(1).Select(RenderAxiomType)) + ")" : ""),
        List<object> => "()",
        LispString => "\"…\"",
        _ => x?.ToString() ?? "",
    };

    static void WriteAxiom(string installDir, StringBuilder sb, List<string> summary)
    {
        string dir = Path.Combine(installDir, "axiom");
        sb.AppendLine();
        sb.AppendLine("==== axiom/: *.NRLIB compiled packages. index.KAF is a Lisp random-access file (first line: offset of an index");
        sb.AppendLine("==== alist of (key 0 offset)); constructorForm, constructorKind, abbreviation and operationAlist are read from it.");
        sb.AppendLine("==== Operation lines are name: (argument types) -> result, one per signature; #1, #2 are the constructor parameters.");
        sb.AppendLine("==== Documentation strings and compiled code (.fasl) are not read.");
        if (!Directory.Exists(dir)) { summary.Add("| scripts.txt: axiom | directory not found |"); return; }
        var nrlibs = Directory.GetDirectories(dir, "*.NRLIB").OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToArray();
        int mc = 0, ops = 0, unreadable = 0;
        foreach (var lib in nrlibs)
        {
            string name = Path.GetFileNameWithoutExtension(lib);
            bool isMc = name.StartsWith("MC", StringComparison.Ordinal);
            if (isMc) mc++;
            sb.AppendLine();
            string kaf = Path.Combine(lib, "index.KAF");
            string origin = isMc ? "MC: Mathcad-authored" : "not MC: standard Axiom name";
            if (!File.Exists(kaf)) { sb.AppendLine($"== {name}  [{origin}]  (no index.KAF; name only)"); unreadable++; continue; }
            try
            {
                string text = File.ReadAllText(kaf, Encoding.Latin1);
                int p = int.Parse(Regex.Match(text, @"^\s*(\d+)").Groups[1].Value);
                var offsets = new Dictionary<string, int>();
                if (ReadDatum(text, ref p, lisp: true) is List<object> index)
                    foreach (var entry in index.OfType<List<object>>())
                        if (entry is [LispString key, _, string off] && int.TryParse(off, out int o)) offsets[key.Value] = o;
                object? Item(string key)
                {
                    if (!offsets.TryGetValue(key, out int o)) return null;
                    return ReadDatum(text, ref o, lisp: true);
                }
                var form = Item("constructorForm");
                var kind = Item("constructorKind");
                var abbreviation = Item("abbreviation");
                var operations = (Item("operationAlist") as List<object> ?? []).OfType<List<object>>()
                    .Where(op => op.Count > 0 && op[0] is string).OrderBy(op => (string)op[0], StringComparer.Ordinal).ToList();
                if (form == null && operations.Count == 0) throw new FormatException("no constructorForm or operationAlist");
                var opLines = new List<string>();
                foreach (var op in operations)
                    foreach (var sig in op.Skip(1).OfType<List<object>>())
                        if (sig is [List<object> { Count: > 0 } types, ..])
                            opLines.Add($"    {op[0]}: ({string.Join(", ", types.Skip(1).Select(RenderAxiomType))}) -> {RenderAxiomType(types[0])}");
                ops += operations.Count;
                sb.AppendLine($"== {name}  [{origin}]  {Render(kind)} {RenderAxiomType(form)}, abbreviation {Render(abbreviation)}, " +
                              $"{operations.Count} operations, {opLines.Count} signatures");
                foreach (var line in opLines) sb.AppendLine(line);
            }
            catch (Exception ex) when (ex is FormatException or ArgumentException or InvalidCastException or IndexOutOfRangeException)
            {
                sb.AppendLine($"== {name}  [{origin}]  (index.KAF not readable as data: {ex.Message}; name only)");
                unreadable++;
            }
        }
        var others = Directory.GetFileSystemEntries(dir).Where(e => !e.EndsWith(".NRLIB", StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e, StringComparer.OrdinalIgnoreCase).ToArray();
        sb.AppendLine();
        sb.AppendLine($"== axiom/ entries that are not NRLIB directories (names only): {others.Length}");
        foreach (var e in others) sb.AppendLine($"  {Path.GetFileName(e)}" + (Directory.Exists(e) ? "  (directory)" : ""));
        summary.Add($"| scripts.txt: axiom | {nrlibs.Length} NRLIB packages ({mc} MC, {nrlibs.Length - mc} other), {ops} operation names, " +
                    $"{unreadable} without a readable index; {others.Length} other entries |");
    }

    // ---------------------------------------------------------------- MCADINCL.H

    static string StripCComments(string s)
    {
        var sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (c is '"' or '\'')
            {
                int start = i;
                for (i++; i < s.Length && s[i] != c && s[i] != '\n'; i++) if (s[i] == '\\') i++;
                sb.Append(s, start, Math.Min(i + 1, s.Length) - start);
            }
            else if (c == '/' && i + 1 < s.Length && s[i + 1] == '/') { while (i + 1 < s.Length && s[i + 1] != '\n') i++; }
            else if (c == '/' && i + 1 < s.Length && s[i + 1] == '*')
            {
                for (i += 2; i + 1 < s.Length && !(s[i] == '*' && s[i + 1] == '/'); i++) if (s[i] == '\n') sb.Append('\n');
                i++;
                sb.Append(' ');
            }
            else sb.Append(c);
        }
        return sb.ToString();
    }

    static string JoinTokens(IEnumerable<string> tokens)
    {
        var sb = new StringBuilder();
        string prev = "";
        foreach (var t in tokens)
        {
            if (sb.Length > 0 && t is not ("," or ")" or "[" or "]") && prev is not ("(" or "[") && !(t == "*" && prev == "*")) sb.Append(' ');
            sb.Append(t);
            prev = t;
        }
        return sb.ToString();
    }

    static List<List<string>> SplitTopLevel(List<string> tokens, string separator)
    {
        var parts = new List<List<string>> { new() };
        int depth = 0;
        foreach (var t in tokens)
        {
            if (t is "(" or "[" or "{") depth++;
            else if (t is ")" or "]" or "}") depth--;
            if (t == separator && depth == 0) { parts.Add([]); continue; }
            parts[^1].Add(t);
        }
        return parts.Where(p => p.Count > 0).ToList();
    }

    static (string Type, string Name) Declarator(List<string> tokens)
    {
        int bracket = tokens.IndexOf("[");
        int nameAt = bracket > 0 ? bracket - 1 : tokens.Count - 1;
        bool named = nameAt > 0 && Regex.IsMatch(tokens[nameAt], @"^[A-Za-z_]\w*$") && tokens[nameAt] is not ("void" or "const");
        if (!named) return (JoinTokens(tokens), "");
        string suffix = bracket > 0 ? JoinTokens(tokens.Skip(bracket)) : "";
        return (JoinTokens(tokens.Take(nameAt)), tokens[nameAt] + suffix);
    }

    static void WriteHeader(string installDir, StringBuilder sb, List<string> summary)
    {
        string path = Path.Combine(installDir, "Custom Functions", "MCADINCL.H");
        sb.AppendLine();
        sb.AppendLine("==== Custom Functions/MCADINCL.H: the user-function C API. Comments are dropped; #define values are kept");
        sb.AppendLine("==== only when they are plain numeric literals.");
        if (!File.Exists(path)) { summary.Add("| scripts.txt: MCADINCL.H | file not found |"); return; }

        var defines = new List<string>();
        var code = new StringBuilder();
        var lines = StripCComments(File.ReadAllText(path)).Replace("\r\n", "\n").Split('\n');
        for (int k = 0; k < lines.Length; k++)
        {
            string line = lines[k];
            while (line.EndsWith('\\') && k + 1 < lines.Length) line = line[..^1] + " " + lines[++k];
            string t = line.Trim();
            if (!t.StartsWith('#')) { code.AppendLine(line); continue; }
            var m = Regex.Match(t, @"^#\s*define\s+([A-Za-z_]\w*)(\([^)]*\))?\s*(.*)$");
            if (!m.Success) continue;
            string value = m.Groups[3].Value.Trim();
            string shown = Regex.IsMatch(value, @"^[-+]?(0[xX][0-9A-Fa-f]+|\d+(\.\d+)?([eE][-+]?\d+)?)[uUlLfF]*$") ? $" = {value}"
                : value.Length == 0 ? "  (no value)" : "  (non-numeric value omitted)";
            defines.Add($"  {m.Groups[1].Value}{(m.Groups[2].Success ? "(macro)" : "")}{shown}");
        }

        var tokens = Regex.Matches(code.ToString(), @"\.\.\.|[A-Za-z_]\w*|0[xX][0-9A-Fa-f]+\w*|\d[\w.]*|""(?:\\.|[^""\\])*""|'(?:\\.|[^'\\])*'|\S")
            .Select(m => m.Value).ToList();
        var declarations = new List<List<string>>();
        var current = new List<string>();
        int depth = 0, transparent = 0;
        for (int i = 0; i < tokens.Count; i++)
        {
            string t = tokens[i];
            if (t == "extern" && i + 2 < tokens.Count && tokens[i + 1].StartsWith('"') && tokens[i + 2] == "{") { transparent++; i += 2; continue; }
            if (t == "}" && depth == 0 && transparent > 0) { transparent--; continue; }
            if (t == "{") depth++;
            if (t == "}") depth--;
            if (t == ";" && depth == 0) { if (current.Count > 0) declarations.Add(current); current = []; continue; }
            current.Add(t);
        }

        var structs = new List<string>();
        var typedefs = new List<string>();
        var functions = new List<string>();
        var otherDecls = new List<string>();
        foreach (var d in declarations)
        {
            int open = d.IndexOf("{");
            if (open >= 0)
            {
                int close = d.LastIndexOf("}");
                string tag = open > 0 && Regex.IsMatch(d[open - 1], @"^[A-Za-z_]\w*$") && d[open - 1] is not ("struct" or "union" or "enum") ? d[open - 1] : "";
                string keyword = d.FirstOrDefault(x => x is "struct" or "union" or "enum") ?? "struct";
                var names = SplitTopLevel(d.Skip(close + 1).ToList(), ",").Select(JoinTokens).ToList();
                structs.Add($"  {keyword} {(names.Count > 0 ? string.Join(", ", names) : tag)}" + (tag.Length > 0 ? $"  (tag {tag})" : ""));
                foreach (var field in SplitTopLevel(d.Skip(open + 1).Take(close - open - 1).ToList(), ";"))
                {
                    var (type, name) = Declarator(field);
                    structs.Add($"      {type} {name}".TrimEnd());
                }
            }
            else if (d[0] == "typedef" && d.Contains("("))
            {
                int star = d.IndexOf("*");
                string name = star > 0 && star + 1 < d.Count ? d[star + 1] : "?";
                typedefs.Add($"  {name} = {JoinTokens(d.Skip(1).Where((_, k) => k + 1 != star + 1))}  (function pointer)");
            }
            else if (d[0] == "typedef")
            {
                var (type, name) = Declarator(d.Skip(1).ToList());
                typedefs.Add($"  {name} = {type}");
            }
            else if (d.Contains("("))
            {
                int p = d.IndexOf("(");
                int end = d.LastIndexOf(")");
                var parameters = SplitTopLevel(d.Skip(p + 1).Take(end - p - 1).ToList(), ",").Select(JoinTokens);
                functions.Add($"  {JoinTokens(d.Take(p - 1))} {d[p - 1]}({string.Join(", ", parameters)})");
            }
            else otherDecls.Add($"  {JoinTokens(d)}");
        }

        int structCount = declarations.Count(d => d.Contains("{"));
        sb.AppendLine();
        sb.AppendLine($"== MCADINCL.H: {functions.Count} functions, {structCount} structs, {typedefs.Count} other typedefs, {defines.Count} #defines");
        sb.AppendLine("-- functions");
        foreach (var f in functions) sb.AppendLine(f);
        sb.AppendLine("-- structs (fields as type name)");
        foreach (var s in structs) sb.AppendLine(s);
        sb.AppendLine("-- typedefs");
        foreach (var t in typedefs) sb.AppendLine(t);
        sb.AppendLine("-- #define names");
        foreach (var d in defines) sb.AppendLine(d);
        if (otherDecls.Count > 0)
        {
            sb.AppendLine("-- other declarations");
            foreach (var o in otherDecls) sb.AppendLine(o);
        }
        summary.Add($"| scripts.txt: MCADINCL.H | {functions.Count} functions, {structCount} structs, {typedefs.Count} other typedefs, {defines.Count} #defines |");
    }

    // ---------------------------------------------------------------- summary.md

    static void WriteSummary(List<DllExports> dlls, List<string> rows, string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Native and script inventory");
        sb.AppendLine();
        sb.AppendLine("Generated by `MirrorGen native-inventory` (docs/mirror/PLAN.md §1.3 and §3 step 5). It covers the Mathcad");
        sb.AppendLine("components outside .NET metadata.");
        sb.AppendLine();
        sb.AppendLine("| Output | Contents |");
        sb.AppendLine("|---|---|");
        int listed = dlls.Sum(d => d.Exports.Count);
        sb.AppendLine($"| exports.txt | {listed} exports listed across {dlls.Count} DLLs |");
        foreach (var row in rows) sb.AppendLine(row);
        sb.AppendLine();
        sb.AppendLine("## Native DLLs");
        sb.AppendLine();
        sb.AppendLine("| DLL | Exports listed | Exports in image | Decorated (C++) | Note |");
        sb.AppendLine("|---|---:|---:|---:|---|");
        foreach (var d in dlls)
            sb.AppendLine($"| {d.Dll} | {d.Exports.Count} | {d.TotalInImage} | {d.Exports.Count(e => e.Name?.StartsWith('?') == true)} | {d.Note} |");
        sb.AppendLine();
        sb.AppendLine("## Method");
        sb.AppendLine();
        sb.AppendLine("Everything here is read as data; no method bodies, clause bodies, comments or source text are copied.");
        sb.AppendLine();
        sb.AppendLine("- **Exports:** the PE export directory (names, ordinals, forwarders) is parsed directly, using");
        sb.AppendLine("  `System.Reflection.PortableExecutable` for headers and sections. MSVC-decorated names are undecorated with");
        sb.AppendLine("  dbghelp `UnDecorateSymbolName` (`UNDNAME_COMPLETE` for the signature, `UNDNAME_NAME_ONLY` for the class).");
        sb.AppendLine("- **Lisp:** an S-expression reader that skips strings and comments records depth-0 definition forms with their");
        sb.AppendLine("  required-parameter counts and lambda-list keywords.");
        sb.AppendLine("- **Prolog:** Mathcad's Prolog files use Lisp syntax; the same reader records clause heads as name/arity.");
        sb.AppendLine("- **Axiom:** package names come from the `*.NRLIB` directory names; constructor, abbreviation and operation");
        sb.AppendLine("  signatures come from the Lisp data in each `index.KAF`.");
        sb.AppendLine("- **MCADINCL.H:** a small C tokenizer records prototypes, struct fields, typedefs and `#define` names");
        sb.AppendLine("  (values only when they are plain numeric literals).");
        File.WriteAllText(path, sb.ToString());
    }
}
