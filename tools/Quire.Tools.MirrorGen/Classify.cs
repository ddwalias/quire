namespace Quire.Tools.MirrorGen;

/// <summary>
/// Checks each built member's §3c status against Mathcad's metadata: whether the member has a Mathcad counterpart
/// decides which statuses it may carry. Needs the Mathcad install; reads both sides as metadata only.
/// </summary>
public static class Classify
{
    public sealed record Member(string Project, string Type, string Text, MemberStatus Status, bool HasCounterpart);

    /// <summary>Every counted member of every mirrored type in the current build, with whether Mathcad declares it.</summary>
    public static List<Member> Members(string repo, Universe mathcad, Rules rules)
    {
        var mapper = new Mapper(mathcad, rules);
        mapper.Build();
        var byKey = mapper.Types.ToDictionary(t => (t.Project!, t.FullName));
        var result = new List<Member>();
        foreach (var (project, dll) in Shape.BuiltProjects(repo))
            foreach (var t in MetadataLoader.Load(dll)!.Types.Where(t => t.MirrorOf != null))
            {
                var keys = byKey.TryGetValue((project, t.FullName), out var m) ? Keys(m) : [];
                foreach (var (line, member) in Shape.Entries(t))
                    if (member != null)
                        result.Add(new(project, t.FullName, line.TrimStart()[(t.FullName.Length + 1)..], member.Status,
                            CandidateKeys(member).Any(keys.Contains)));
            }
        return result;
    }

    /// <summary>
    /// Statuses that contradict the metadata. Unclassified members aren't problems; they are open work. Stubs are
    /// generated from Mathcad's members by construction (their names can differ only by the writer's C# renames).
    /// </summary>
    public static IEnumerable<string> Problems(IEnumerable<Member> members) =>
        members.Select(m => (m, problem: m.Status switch
        {
            MemberStatus.QuireOnly when m.HasCounterpart => "is [QuireOnly], but Mathcad declares it",
            MemberStatus.Verified or MemberStatus.Documented or MemberStatus.Blocked when !m.HasCounterpart =>
                $"is {m.Status}, but Mathcad has no such member (mark it [QuireOnly])",
            _ => null,
        })).Where(x => x.problem != null).Select(x => $"{x.m.Project}: {x.m.Type} {x.m.Text} {x.problem}");

    public static void Run(string repo, Universe mathcad, Rules rules, bool list)
    {
        var members = Members(repo, mathcad, rules);
        foreach (var p in Problems(members)) Console.WriteLine("PROBLEM " + p);
        var open = members.Where(m => m.Status == MemberStatus.Unclassified).ToList();
        Console.WriteLine($"{members.Count} members; unclassified: {open.Count(m => m.HasCounterpart)} with a Mathcad counterpart, "
            + $"{open.Count(m => !m.HasCounterpart)} without (Quire-only candidates)");
        if (list)
            foreach (var m in open.OrderBy(m => m.Project).ThenBy(m => m.Type, StringComparer.Ordinal))
                Console.WriteLine($"{(m.HasCounterpart ? "mathcad   " : "quire-only")} {m.Project} {m.Type} {m.Text}");
    }

    private static HashSet<string> Keys(TypeModel t) =>
    [
        .. t.Methods.Select(HandWritten.MethodKey),
        .. t.Properties.Select(p => "P " + p.Name),
        .. t.Events.Select(e => "E " + e.Name),
        .. t.Fields.Select(f => "F " + f.Name),
    ];

    /// <summary>A method also matches Mathcad's shorter overloads that its optional parameters stand in for.</summary>
    private static IEnumerable<string> CandidateKeys(IHasStatus member)
    {
        switch (member)
        {
            case MethodModel m:
                yield return HandWritten.MethodKey(m);
                for (int n = m.Params.Length - 1; n >= 0 && m.Params[n].HasDefault; n--)
                    yield return HandWritten.MethodKey(m with { Params = m.Params[..n] });
                break;
            case PropertyModel p: yield return "P " + p.Name; break;
            case EventModel e: yield return "E " + e.Name; break;
            case FieldModel f: yield return "F " + f.Name; break;
        }
    }
}
