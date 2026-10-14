using static Quire.Differential.Tests.Reflect;
using Q = Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.RuntimeOptions</c> and <c>Mcd.UnitSystemGroup</c> with Quire's: defaults, syncing to a
/// thread, saving a thread's options, and group membership; cases are listed in
/// docs/mirror/evidence/Mcd.RuntimeOptions.md.
/// </summary>
public class RuntimeOptionsDifferentialTests
{
    private static System.Type M => Mathcad.Type("mplrun", "Mcd.RuntimeOptions");
    private static System.Type MGroup => Mathcad.Type("mplrun", "Mcd.UnitSystemGroup");
    private static System.Type MSystem => Mathcad.Type("mplrun", "Mcd.UnitSystem");

    private static readonly string[] Flags =
    [
        "strict_comparisons", "strict_singularity_checking", "strings_indexed_from_origin", "zero_over_zero_is_zero",
        "debugging_enabled", "labels_in_symbolics", "units_in_symbolics", "constants_in_symbolics",
    ];

    private static string SystemText(object? s) => s == null ? "null"
        : $"{Get(s, "Name")}#{Get(s, "RO_number")} [{string.Join(" ", ((Array)Get(s, "Basis")!).Cast<object>().Select(u => Get(u, "Symbol")))}]"
          + $" [{string.Join(" ", ((Array)Get(s, "Defaults")!).Cast<object>().Select(u => Get(u, "Symbol")))}]";

    private static string Instance(object o, object defaultSystem)
    {
        var parts = Flags.Select(f => $"m_{f}={Get(o, "m_" + f)}").ToList();
        parts.Add($"m_mangled_identifiers={Get(o, "m_mangled_identifiers")}");
        parts.Add($"m_unit_system default={ReferenceEquals(Get(o, "m_unit_system"), defaultSystem)} {SystemText(Get(o, "m_unit_system"))}");
        var group = Get(o, "m_unit_system_group");
        parts.Add($"group={(group == null ? "null" : $"{Get(group, "Name")} member0 default={ReferenceEquals(Call(group, "findMember", 0), defaultSystem)}")}");
        parts.Add($"m_ro_number={Get(o, "m_ro_number")}");
        return string.Join(", ", parts);
    }

    private static string Thread(System.Type options)
    {
        var parts = Flags.Select(f => $"{f}={Get(options, f)}").ToList();
        parts.Add($"mangled_identifiers={Get(options, "mangled_identifiers")}");
        parts.Add($"the_ro_number={Get(options, "the_ro_number")}");
        parts.Add($"the_unit_system={SystemText(Get(options, "the_unit_system"))}");
        var group = Get(options, "the_unit_system_group");
        parts.Add($"the_unit_system_group={(group == null ? "null" : Get(group, "Name"))}");
        return string.Join(", ", parts);
    }

    /// <summary>A scenario run on a fresh thread for either side.</summary>
    private static string Scenario(System.Type options, System.Type dim)
    {
        var log = new List<string>();
        var t = new Thread(() =>
        {
            object defaultSystem = Get(options, "s_default_unit_system")!;
            log.Add("statics s_restricted_mode=" + Get(options, "s_restricted_mode") + " s_ro_counter=" + Get(options, "s_ro_counter"));
            log.Add("default system " + SystemText(defaultSystem) + " code " + Get(defaultSystem, "Code") + " guid " + Get(defaultSystem, "Guid"));
            log.Add("fresh thread " + Thread(options));
            log.Add("fresh thread s_vec " + (Get(dim, "s_vec") == null ? "null" : "set"));
            log.Add("saved on fresh thread " + Instance(Call(options, "SaveThreadOptions")!, defaultSystem));
            var o = Activator.CreateInstance(options)!;
            log.Add("new " + Instance(o, defaultSystem));
            foreach (var f in Flags) Set(o, "m_" + f, !(bool)Get(o, "m_" + f)!);
            Set(o, "m_mangled_identifiers", Enum.ToObject(Get(o, "m_mangled_identifiers")!.GetType(), 2));
            Set(o, "m_ro_number", 17);
            Call(o, "SyncToThread");
            log.Add("synced " + Thread(options));
            log.Add("synced s_vec " + (Get(dim, "s_vec") is int[] v ? v.Length.ToString() : "null"));
            var saved = Call(options, "SaveThreadOptions")!;
            log.Add("saved " + Instance(saved, defaultSystem));
            log.Add("saved group is thread's " + ReferenceEquals(Get(saved, "m_unit_system_group"), Get(options, "the_unit_system_group")));
            var group = Get(o, "m_unit_system_group")!;
            var copy = Call(group, "addCopyWithRONumber", 17);
            log.Add("copy " + SystemText(copy));
            Call(o, "RemoveUnitSystem");
            log.Add("after remove " + SystemText(Call(group, "findMember", 17)) + " / member0 " + SystemText(Call(group, "findMember", 0)));
            Set(o, "m_unit_system_group", null);
            log.Add("remove without group " + Outcome(() => Call(o, "RemoveUnitSystem"), _ => "ok"));
        });
        t.Start();
        t.Join();
        return string.Join("\n", log);
    }

    [SkippableFact]
    public void Options_sync_to_threads_and_save()
    {
        Runtime.Sync();
        Assert.Equal(Scenario(M, Mathcad.Type("mplrun", "Mcd.Dim")), Scenario(typeof(Q.RuntimeOptions), typeof(Q.Dim)));
    }

    private static string GroupScript(System.Type groupType, System.Type systemType, Func<string, int, object> makeSystem)
    {
        var log = new List<string>();
        void Step(string what, Func<object?> f, Func<object?, string>? show = null) => log.Add(what + " = " + Outcome(f, show));
        var initial = makeSystem("init", 0);
        var group = New(groupType, initial);
        Step("Name", () => Get(group, "Name"));
        Step("findMember(0) initial", () => ReferenceEquals(Call(group, "findMember", 0), initial));
        Step("findMember(5)", () => Call(group, "findMember", 5), SystemText);
        var five = makeSystem("five", 5);
        Step("addMember(five)", () => { Call(group, "addMember", five); return Call(group, "findMember", 5); }, SystemText);
        Step("addMember(other five)", () => { Call(group, "addMember", makeSystem("other", 5)); return Call(group, "findMember", 5); }, SystemText);
        var initialBasis = Get(initial, "Basis");
        Step("changeMember(5, initial basis, [])", () =>
        {
            Call(group, "changeMember", 5, Get(five, "Basis"), Array.CreateInstance(Get(five, "Basis")!.GetType().GetElementType()!, 0));
            return Call(group, "findMember", 5);
        }, SystemText);
        Step("changeMember(9, ...)", () => { Call(group, "changeMember", 9, initialBasis, initialBasis); return "ok"; });
        Step("resetMember(5)", () => { Call(group, "resetMember", 5); return Call(group, "findMember", 5); }, SystemText);
        Step("resetMember(0)", () => { Call(group, "resetMember", 0); return Call(group, "findMember", 0); }, SystemText);
        Step("Reset(5) same", () => ReferenceEquals(Call(group, "Reset", 5), five));
        Step("Reset(5)", () => Call(group, "findMember", 5), SystemText);
        Step("Reset(42) initial", () => ReferenceEquals(Call(group, "Reset", 42), initial));
        Step("addCopyWithRONumber(5) existing", () => ReferenceEquals(Call(group, "addCopyWithRONumber", 5), five));
        Step("addCopyWithRONumber(8)", () => Call(group, "addCopyWithRONumber", 8), SystemText);
        Step("addCopyWithRONumber(8) again same", () => ReferenceEquals(Call(group, "addCopyWithRONumber", 8), Call(group, "findMember", 8)));
        Step("copy shares basis", () => ReferenceEquals(Get(Call(group, "findMember", 8)!, "Basis"), Get(initial, "Basis")));
        Step("removeMember(77)", () => { Call(group, "removeMember", 77); return "ok"; });
        Step("removeMember(0)", () => { Call(group, "removeMember", 0); return Call(group, "findMember", 0); }, SystemText);
        Step("resetMember(5) without initial", () => { Call(group, "resetMember", 5); return Call(group, "findMember", 5); }, SystemText);
        Step("Reset(42) without initial", () => Call(group, "Reset", 42), SystemText);
        Step("addCopyWithRONumber(9) without initial", () => Call(group, "addCopyWithRONumber", 9), SystemText);
        Step("new group(null)", () => New(groupType, [null]), _ => "ok");
        return string.Join("\n", log);
    }

    [SkippableFact]
    public void Group_membership()
    {
        Runtime.Sync();
        object MSys(string name, int ro)
        {
            var d = Get(M, "s_default_unit_system")!;
            var s = New(MSystem, name, 3u, "g", Get(d, "Basis"), Get(d, "Defaults"));
            Set(s, "RO_number", ro);
            return s;
        }
        object QSys(string name, int ro)
        {
            var d = Q.RuntimeOptions.s_default_unit_system;
            return new Q.UnitSystem(name, 3u, "g", d.Basis, d.Defaults) { RO_number = ro };
        }
        Assert.Equal(GroupScript(MGroup, MSystem, MSys), GroupScript(typeof(Q.UnitSystemGroup), typeof(Q.UnitSystem), QSys));
    }
}
