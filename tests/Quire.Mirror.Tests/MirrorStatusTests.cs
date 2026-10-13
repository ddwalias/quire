using System.IO;
using System.Text.RegularExpressions;
using Quire.Tools.MirrorGen;

namespace Quire.Mirror.Tests;

/// <summary>
/// Checks that every per-member status (PLAN.md §3c rule 6) points at a record that exists: an evidence file for
/// analyzed and tested members, a differential test class for verified ones, and an accepted ledger entry for
/// deviations.
/// </summary>
public class MirrorStatusTests
{
    private static readonly string Repo = FindRepo();

    private static string FindRepo()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "Quire.slnx"))) return dir.FullName;
        throw new DirectoryNotFoundException("Quire.slnx not found above the test output directory");
    }

    private static IEnumerable<(string Member, MemberStatus Status, string? Evidence)> Members()
    {
        foreach (var dll in Directory.GetFiles(AppContext.BaseDirectory, "Quire*.dll"))
        {
            if (Path.GetFileName(dll).StartsWith("Quire.Tools", StringComparison.Ordinal)) continue;
            if (MetadataLoader.Load(dll) is not { } model) continue;
            foreach (var t in model.Types.Where(t => t.MirrorOf != null))
                foreach (var (line, m) in Shape.Entries(t))
                    if (m != null) yield return (line.Trim(), m.Status, m.Evidence);
        }
    }

    [Fact]
    public void Every_status_reference_resolves()
    {
        string evidenceDir = Path.Combine(Repo, "docs", "mirror", "evidence");
        string ledger = File.ReadAllText(Path.Combine(Repo, "docs", "mirror", "deviations.md"));
        string diffDir = Path.Combine(Repo, "tests", "Quire.Differential.Tests");
        var diffClasses = Directory.Exists(diffDir)
            ? Directory.GetFiles(diffDir, "*.cs", SearchOption.AllDirectories)
                .SelectMany(f => Regex.Matches(File.ReadAllText(f), @"\bclass\s+(\w+)").Select(m => m.Groups[1].Value)).ToHashSet()
            : [];

        var problems = new List<string>();
        foreach (var (member, status, evidence) in Members())
        {
            switch (status)
            {
                case MemberStatus.Analyzed or MemberStatus.Tested:
                    if (string.IsNullOrWhiteSpace(evidence) || !File.Exists(Path.Combine(evidenceDir, evidence + ".md")))
                        problems.Add($"{member}: evidence record '{evidence}' not found in docs/mirror/evidence");
                    break;
                case MemberStatus.Verified:
                    if (evidence == null || !diffClasses.Contains(evidence))
                        problems.Add($"{member}: differential test class '{evidence}' not found in Quire.Differential.Tests");
                    break;
                case MemberStatus.Deviation:
                    var entry = evidence == null ? null
                        : Regex.Match(ledger, @"^### " + Regex.Escape(evidence) + @"\b.*?(?=^### |\z)", RegexOptions.Multiline | RegexOptions.Singleline);
                    if (entry is not { Success: true } || !Regex.IsMatch(entry.Value, @"\*\*Status:\*\*\s*accepted-(temporary|permanent)"))
                        problems.Add($"{member}: '{evidence}' is not an accepted entry in deviations.md");
                    break;
                case MemberStatus.Blocked or MemberStatus.Documented:
                    if (string.IsNullOrWhiteSpace(evidence)) problems.Add($"{member}: {status} without a reason or source");
                    break;
            }
        }
        Assert.True(problems.Count == 0, $"{problems.Count} unresolved status references:\n" + string.Join("\n", problems.Take(40)));
    }
}
