using System.Text;

/// <summary>The survey report: remaining stubs per project, by member kind and by the shape of Mathcad's body.</summary>
static class Survey
{
    public static string Report(List<(string Project, Plan Plan, string? RenderFailure)> rows)
    {
        string Category((string Project, Plan Plan, string? RenderFailure) r) =>
            !r.Plan.Mechanical ? r.Plan.Category : r.RenderFailure != null ? "mechanical, not renderable: " + r.RenderFailure : r.Plan.Category;
        bool Candidate((string Project, Plan Plan, string? RenderFailure) r) => r.Plan.Mechanical && r.RenderFailure == null;
        var sb = new StringBuilder("# MemberGen survey: remaining [MirrorStub] members\n\n");
        var members = rows.Where(r => r.Plan.Stub.Kind != "constant").ToList();
        sb.Append($"{rows.Count} stub members, {rows.Count - members.Count} of them constants (they stay). Of the other {members.Count}: " +
                  $"{members.Count(r => r.Plan.Stub.Kind == "field")} fields (no body; they move with the members that use them), " +
                  $"{members.Count(r => r.Plan.Stub.Kind != "field")} with bodies, of which **{members.Count(Candidate)} are mechanical candidates** " +
                  "(a branch-free, local-free body the tool can write as C#; acceptance still needs an exact IL match and a passing test).\n\n");
        sb.Append("## Per project\n\n| Project | Stubs | Fields | With bodies | Mechanical candidates | % of bodies |\n|---|--:|--:|--:|--:|--:|\n");
        foreach (var g in members.GroupBy(r => r.Project).OrderByDescending(g => g.Count()))
        {
            int bodies = g.Count(r => r.Plan.Stub.Kind != "field"), mechanical = g.Count(Candidate);
            sb.Append($"| {g.Key} | {g.Count()} | {g.Count(r => r.Plan.Stub.Kind == "field")} | {bodies} | {mechanical} | {Percent(mechanical, bodies)} |\n");
        }
        sb.Append("\n## Mechanical candidates by pattern and member kind\n\n");
        Table(sb, members.Where(Candidate).ToList(), r => r.Plan.Category);
        sb.Append("\n## Not mechanical, by first obstacle\n\n");
        Table(sb, members.Where(r => r.Plan.Stub.Kind != "field" && !Candidate(r)).ToList(), Category);
        sb.Append("\n## Mechanical candidates per project and pattern\n\n| Project | Pattern | Members |\n|---|---|--:|\n");
        foreach (var g in members.Where(Candidate).GroupBy(r => (r.Project, r.Plan.Category)).OrderBy(g => g.Key.Project).ThenByDescending(g => g.Count()))
            sb.Append($"| {g.Key.Project} | {g.Key.Category} | {g.Count()} |\n");
        return sb.ToString();
    }

    private static void Table(StringBuilder sb, List<(string Project, Plan Plan, string? RenderFailure)> rows,
        Func<(string Project, Plan Plan, string? RenderFailure), string> category)
    {
        string[] kinds = ["method", "constructor", "property", "event"];
        sb.Append("| Category | Total | " + string.Join(" | ", kinds) + " |\n|---|--:|" + string.Concat(kinds.Select(_ => "--:|")) + "\n");
        foreach (var g in rows.GroupBy(category).OrderByDescending(g => g.Count()))
            sb.Append($"| {g.Key} | {g.Count()} | " + string.Join(" | ", kinds.Select(k => g.Count(r => r.Plan.Stub.Kind == k))) + " |\n");
    }

    private static string Percent(int part, int whole) => whole == 0 ? "–" : $"{100.0 * part / whole:0.0}%";
}
