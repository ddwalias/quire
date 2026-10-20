using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace Quire.Tools.MirrorGen;

/// <summary>Original assembly identities and reference edges, read as metadata only.</summary>
public static class AssemblyMap
{
    public static void Write(string repo, string install, Rules rules)
    {
        var assemblies = new List<object>();
        foreach (var group in rules.Scope.Values.GroupBy(s => s.Assembly, StringComparer.OrdinalIgnoreCase).OrderBy(g => g.Key))
        {
            string? project;
            try { project = Rules.ProjectFor(group.Key); }
            catch (ArgumentException) { project = null; }
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
                    name = md.GetString(definition.Name), file = Path.GetFileName(path), project,
                    target_exists = project != null && File.Exists(Path.Combine(repo, "src", project, project + ".csproj")),
                    mvid = md.GetGuid(md.GetModuleDefinition().Mvid).ToString(),
                    references = md.AssemblyReferences.Select(h => md.GetString(md.GetAssemblyReference(h).Name)).Order().ToArray(),
                    mapped_types = types.Count, expected_methods = methods, error = (string?)null
                });
            }
            catch (Exception error) when (error is IOException or BadImageFormatException)
            {
                assemblies.Add(new { name = group.Key, file = Path.GetFileName(path), project,
                    target_exists = project != null && File.Exists(Path.Combine(repo, "src", project, project + ".csproj")),
                    error = error.Message });
            }
        }
        Console.WriteLine(JsonSerializer.Serialize(assemblies, new JsonSerializerOptions { WriteIndented = true }));
    }
}
