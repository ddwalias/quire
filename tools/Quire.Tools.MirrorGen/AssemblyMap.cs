using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;

namespace Quire.Tools.MirrorGen;

/// <summary>Original assembly identities and reference edges, read as metadata and PE imports only.</summary>
public static class AssemblyMap
{
    public static void Write(string repo, string install, Rules rules)
    {
        var native = Rules.AssembliesIn(Language.Native).ToDictionary(n => n, StringComparer.OrdinalIgnoreCase);
        // Native import edges between Mathcad's own DLLs, in either direction (msmpl imports msclr and msgen).
        var known = rules.Scope.Values.Select(s => s.Assembly).Concat(native.Values)
            .DistinctBy(n => n, StringComparer.OrdinalIgnoreCase).ToDictionary(n => n, StringComparer.OrdinalIgnoreCase);
        var assemblies = new List<object>();
        foreach (var group in rules.Scope.Values.GroupBy(s => s.Assembly, StringComparer.OrdinalIgnoreCase).OrderBy(g => g.Key))
        {
            var (project, host, language, exists) = Target(rules, group.Key);
            string path = Path.Combine(install, group.Key + ".dll");
            try
            {
                using var pe = new PEReader(File.OpenRead(path));
                var md = pe.GetMetadataReader();
                var definition = md.GetAssemblyDefinition();
                var types = group.Where(s => s.Disposition is Disposition.Mirror or Disposition.SharedSource)
                    .Select(s => s.MathcadType).ToHashSet();
                var methods = md.TypeDefinitions.Where(h => types.Contains(SigProvider.FullName(md, h)))
                    .SelectMany(h => md.GetTypeDefinition(h).GetMethods())
                    .Select(h => $"0x{MetadataTokens.GetToken(h):x8}").ToArray();
                assemblies.Add(new
                {
                    name = md.GetString(definition.Name), file = Path.GetFileName(path), project, host, language,
                    target_exists = exists, deferred = Rules.IsDeferredAssembly(group.Key),
                    mvid = md.GetGuid(md.GetModuleDefinition().Mvid).ToString(),
                    references = md.AssemblyReferences.Select(h => md.GetString(md.GetAssemblyReference(h).Name)).Order().ToArray(),
                    native_imports = Imports(pe).Where(known.ContainsKey).Select(n => known[n]).ToArray(),
                    mapped_types = types.Count, expected_methods = methods, error = (string?)null
                });
            }
            catch (Exception error) when (error is IOException or BadImageFormatException or ArgumentOutOfRangeException)
            {
                assemblies.Add(new { name = group.Key, file = Path.GetFileName(path), project, host, language,
                    target_exists = exists, deferred = Rules.IsDeferredAssembly(group.Key), error = error.Message });
            }
        }
        // D20: native DLLs have no types in the type map, but the C++/CLI mirrors link against them.
        foreach (var name in native.Values)
        {
            var (project, _, language, exists) = Target(rules, name);
            string path = Path.Combine(install, name + ".dll");
            try
            {
                using var pe = new PEReader(File.OpenRead(path));
                assemblies.Add(new { name, file = Path.GetFileName(path), project, host = (string?)null, language,
                    target_exists = exists, references = Array.Empty<string>(),
                    native_imports = Imports(pe).Where(known.ContainsKey).Select(n => known[n]).ToArray(), error = (string?)null });
            }
            catch (Exception error) when (error is IOException or BadImageFormatException or ArgumentOutOfRangeException)
            {
                assemblies.Add(new { name, file = Path.GetFileName(path), project, host = (string?)null, language,
                    target_exists = exists, error = error.Message });
            }
        }
        Console.WriteLine(JsonSerializer.Serialize(assemblies, new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>The D20 target, the current host, the language and whether the target project exists in that language.</summary>
    private static (string? Project, string? Host, string? Language, bool Exists) Target(Rules rules, string asm)
    {
        try
        {
            string project = Rules.ProjectFor(asm);
            var language = Rules.LanguageOf(asm);
            string extension = language is Language.CSharp ? ".csproj" : ".vcxproj";
            string? host = language is Language.Native ? null : rules.HostFor(asm);
            return (project, host, language.ToString(), File.Exists(Path.Combine(rules.Repo, "src", project, project + extension))
                && language is not Language.Mpl);
        }
        catch (ArgumentException) { return (null, null, null, false); }
    }

    /// <summary>The DLL names in the PE import directory, without extension. Delay-load imports are not read.</summary>
    private static IEnumerable<string> Imports(PEReader pe)
    {
        var directory = pe.PEHeaders.PEHeader!.ImportTableDirectory;
        if (directory.Size == 0) yield break;
        for (int offset = 0; ; offset += 20)
        {
            var descriptor = pe.GetSectionData(directory.RelativeVirtualAddress + offset).GetReader(0, 20);
            descriptor.Offset = 12;
            int nameRva = descriptor.ReadInt32();
            if (nameRva == 0) yield break;
            var name = pe.GetSectionData(nameRva).GetReader();
            var bytes = new List<byte>();
            for (byte b; (b = name.ReadByte()) != 0;) bytes.Add(b);
            yield return Path.GetFileNameWithoutExtension(Encoding.ASCII.GetString([.. bytes]));
        }
    }
}
