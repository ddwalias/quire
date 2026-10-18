using System.Reflection;
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
    private const string Contract = "Mcd.RuntimeOptions";
    private static System.Type M => Mathcad.Type("mplrun", "Mcd.RuntimeOptions");
    private static System.Type MGroup => Mathcad.Type("mplrun", "Mcd.UnitSystemGroup");
    private static System.Type MSystem => Mathcad.Type("mplrun", "Mcd.UnitSystem");

    private static readonly string[] Flags =
    [
        "strict_comparisons", "strict_singularity_checking", "strings_indexed_from_origin", "zero_over_zero_is_zero",
        "debugging_enabled", "labels_in_symbolics", "units_in_symbolics", "constants_in_symbolics",
    ];

    private static string SystemText(object? s) => s == null ? "null"
        : $"{Get(s, "Name")}#{Get(s, "RO_number")} [{string.Join(" ", ((Array)Get(s, "Basis")!).Cast<object?>().Select(u => u == null ? "null" : Get(u, "Symbol")))}]"
          + $" [{string.Join(" ", ((Array)Get(s, "Defaults")!).Cast<object?>().Select(u => u == null ? "null" : Get(u, "Symbol")))}]";

    private static string GroupState(object g)
    {
        string name = (string)Get(g, "Name")!;
        var list = (System.Collections.IDictionary)Get(g, "m_unitsys_list")!;
        var members = new List<string>();
        foreach (System.Collections.DictionaryEntry entry in list)
            members.Add($"{entry.Key}={SystemText(entry.Value)}");
        members.Sort();
        return $"Group({name}, [{string.Join(", ", members)}])";
    }

    private static string OptionsState(object o)
    {
        var parts = Flags.Select(f => $"m_{f}={Get(o, "m_" + f)}").ToList();
        parts.Add($"m_mangled_identifiers={Get(o, "m_mangled_identifiers")}");
        parts.Add($"m_unit_system={SystemText(Get(o, "m_unit_system"))}");
        var group = Get(o, "m_unit_system_group");
        parts.Add($"group={(group == null ? "null" : GroupState(group))}");
        parts.Add($"m_ro_number={Get(o, "m_ro_number")}");
        return string.Join(", ", parts);
    }

    private static string Show(object? o)
    {
        if (o == null) return "null";
        if (o.GetType().Name == "RuntimeOptions") return OptionsState(o);
        if (o.GetType().Name == "UnitSystemGroup") return GroupState(o);
        if (o.GetType().Name == "UnitSystem") return SystemText(o);
        if (o.GetType().Name == "RuntimeOptionsInitializer") return "RuntimeOptionsInitializer";
        if (o is Exception ex) return Thrown(ex);
        return Unmap(Reflect.Text(o));
    }

    private static string Unmap(string s) => System.Text.RegularExpressions.Regex.Replace(s, @"\bQuire\.(?=(Mcd|Mpl)\.)", "");

    private static object MakeSystem(bool mathcad, string name, int roNumber)
    {
        var defaultSystem = mathcad
            ? Get(Mathcad.Type("mplrun", "Mcd.RuntimeOptions"), "s_default_unit_system")!
            : Q.RuntimeOptions.s_default_unit_system;
        var basis = (Array)Get(defaultSystem, "Basis")!;
        var defaults = (Array)Get(defaultSystem, "Defaults")!;
        var sys = mathcad
            ? New(MSystem, name, 7u, "guid", basis, defaults)
            : new Q.UnitSystem(name, 7u, "guid", (Q.Unit[])basis, (Q.Unit[])defaults);
        Set(sys, "RO_number", roNumber);
        return sys;
    }

    [SkippableFact]
    public void UnitSystemGroup_methods_and_state()
    {
        Runtime.Sync();

        var qGroupCtor = typeof(Q.UnitSystemGroup).GetConstructor([typeof(Q.UnitSystem)])!;
        var mGroupCtor = MGroup.GetConstructor([MSystem])!;
        var qGetName = typeof(Q.UnitSystemGroup).GetMethod("get_Name")!;
        var mGetName = MGroup.GetMethod("get_Name")!;
        var qFindMember = typeof(Q.UnitSystemGroup).GetMethod("findMember", [typeof(int)])!;
        var mFindMember = MGroup.GetMethod("findMember", [typeof(int)])!;
        var qAddMember = typeof(Q.UnitSystemGroup).GetMethod("addMember", [typeof(Q.UnitSystem)])!;
        var mAddMember = MGroup.GetMethod("addMember", [MSystem])!;
        var qChangeMember = typeof(Q.UnitSystemGroup).GetMethod("changeMember", [typeof(int), typeof(Q.Unit[]), typeof(Q.Unit[])])!;
        var mChangeMember = MGroup.GetMethod("changeMember", [typeof(int), Array.CreateInstance(Mathcad.Type("mplrun", "Mcd.Unit"), 0).GetType(), Array.CreateInstance(Mathcad.Type("mplrun", "Mcd.Unit"), 0).GetType()])!;
        var qRemoveMember = typeof(Q.UnitSystemGroup).GetMethod("removeMember", [typeof(int)])!;
        var mRemoveMember = MGroup.GetMethod("removeMember", [typeof(int)])!;
        var qResetMember = typeof(Q.UnitSystemGroup).GetMethod("resetMember", [typeof(int)])!;
        var mResetMember = MGroup.GetMethod("resetMember", [typeof(int)])!;
        var qReset = typeof(Q.UnitSystemGroup).GetMethod("Reset", [typeof(int)])!;
        var mReset = MGroup.GetMethod("Reset", [typeof(int)])!;
        var qAddCopy = typeof(Q.UnitSystemGroup).GetMethod("addCopyWithRONumber", [typeof(int)])!;
        var mAddCopy = MGroup.GetMethod("addCopyWithRONumber", [typeof(int)])!;

        // Constructor with system
        var qInitial = MakeSystem(false, "init", 0);
        var mInitial = MakeSystem(true, "init", 0);
        MethodComparison.Compare("UnitSystemGroup.ctor(init)", Contract, qGroupCtor, null, [qInitial], mGroupCtor, null, [mInitial], Show, Show);

        // Constructor with null throws NullReferenceException
        MethodComparison.Compare("UnitSystemGroup.ctor(null)", Contract, qGroupCtor, null, [null], mGroupCtor, null, [null], Show, Show);

        var qGroup = new Q.UnitSystemGroup((Q.UnitSystem)qInitial);
        var mGroup = New(MGroup, mInitial);

        // Name
        MethodComparison.Compare("UnitSystemGroup.Name", Contract, qGetName, qGroup, [], mGetName, mGroup, [], Show, Show, GroupState, GroupState);

        // findMember
        MethodComparison.Compare("findMember(0)", Contract, qFindMember, qGroup, [0], mFindMember, mGroup, [0], Show, Show, GroupState, GroupState);
        MethodComparison.Compare("findMember(5) missing", Contract, qFindMember, qGroup, [5], mFindMember, mGroup, [5], Show, Show, GroupState, GroupState);

        // addMember
        var qSys3 = MakeSystem(false, "three", 3);
        var mSys3 = MakeSystem(true, "three", 3);
        MethodComparison.Compare("addMember(3)", Contract, qAddMember, qGroup, [qSys3], mAddMember, mGroup, [mSys3], Show, Show, GroupState, GroupState);

        // addMember duplicate does not overwrite
        var qSys3Dup = MakeSystem(false, "duplicate", 3);
        var mSys3Dup = MakeSystem(true, "duplicate", 3);
        MethodComparison.Compare("addMember(3 duplicate)", Contract, qAddMember, qGroup, [qSys3Dup], mAddMember, mGroup, [mSys3Dup], Show, Show, GroupState, GroupState);

        // changeMember on existing
        var qUnitType = typeof(Q.Unit);
        var mUnitType = Mathcad.Type("mplrun", "Mcd.Unit");
        var qNewBasis = Array.CreateInstance(qUnitType, 1);
        qNewBasis.SetValue(new Q.Unit("second", "s", "Time", 1.0, "V{Time 60000}"), 0);
        var mNewBasis = Array.CreateInstance(mUnitType, 1);
        mNewBasis.SetValue(New(mUnitType, "second", "s", "Time", 1.0, "V{Time 60000}"), 0);
        var qNewDefaults = Array.CreateInstance(qUnitType, 0);
        var mNewDefaults = Array.CreateInstance(mUnitType, 0);

        MethodComparison.Compare("changeMember(3)", Contract, qChangeMember, qGroup, [3, qNewBasis, qNewDefaults], mChangeMember, mGroup, [3, mNewBasis, mNewDefaults], Show, Show, GroupState, GroupState);

        // changeMember on existing with a full basis replaces both basis and defaults
        var qFull = MakeSystem(false, "full", 0);
        var mFull = MakeSystem(true, "full", 0);
        var qFullBasis = (Array)Get(qFull, "Basis")!;
        var mFullBasis = (Array)Get(mFull, "Basis")!;
        var qFewDefaults = Array.CreateInstance(qUnitType, 1);
        qFewDefaults.SetValue(new Q.Unit("hertz", "Hz", "Frequency", 1.0, "V{Time-60000}"), 0);
        var mFewDefaults = Array.CreateInstance(mUnitType, 1);
        mFewDefaults.SetValue(New(mUnitType, "hertz", "Hz", "Frequency", 1.0, "V{Time-60000}"), 0);
        MethodComparison.Compare("changeMember(3 full basis)", Contract, qChangeMember, qGroup, [3, qFullBasis, qFewDefaults], mChangeMember, mGroup, [3, mFullBasis, mFewDefaults], Show, Show, GroupState, GroupState);

        // changeMember on missing
        MethodComparison.Compare("changeMember(99 missing)", Contract, qChangeMember, qGroup, [99, qNewBasis, qNewDefaults], mChangeMember, mGroup, [99, mNewBasis, mNewDefaults], Show, Show, GroupState, GroupState);

        // resetMember on existing key 3 (copies member 0's basis and defaults)
        MethodComparison.Compare("resetMember(3)", Contract, qResetMember, qGroup, [3], mResetMember, mGroup, [3], Show, Show, GroupState, GroupState);

        // resetMember on key 0 does nothing
        MethodComparison.Compare("resetMember(0)", Contract, qResetMember, qGroup, [0], mResetMember, mGroup, [0], Show, Show, GroupState, GroupState);

        // resetMember on missing key does nothing
        MethodComparison.Compare("resetMember(99)", Contract, qResetMember, qGroup, [99], mResetMember, mGroup, [99], Show, Show, GroupState, GroupState);

        // Reset(3) calls UnitSystem.Reset
        MethodComparison.Compare("Reset(3)", Contract, qReset, qGroup, [3], mReset, mGroup, [3], Show, Show, GroupState, GroupState);

        // Reset(99) falls back to member 0
        MethodComparison.Compare("Reset(99 fallback)", Contract, qReset, qGroup, [99], mReset, mGroup, [99], Show, Show, GroupState, GroupState);

        // addCopyWithRONumber on existing key returns existing member
        MethodComparison.Compare("addCopyWithRONumber(3 existing)", Contract, qAddCopy, qGroup, [3], mAddCopy, mGroup, [3], Show, Show, GroupState, GroupState);

        // addCopyWithRONumber on new key creates a copy
        MethodComparison.Compare("addCopyWithRONumber(7 new)", Contract, qAddCopy, qGroup, [7], mAddCopy, mGroup, [7], Show, Show, GroupState, GroupState);

        // removeMember on key 3
        MethodComparison.Compare("removeMember(3)", Contract, qRemoveMember, qGroup, [3], mRemoveMember, mGroup, [3], Show, Show, GroupState, GroupState);

        // removeMember on missing key
        MethodComparison.Compare("removeMember(99)", Contract, qRemoveMember, qGroup, [99], mRemoveMember, mGroup, [99], Show, Show, GroupState, GroupState);

        // removeMember on key 0
        MethodComparison.Compare("removeMember(0)", Contract, qRemoveMember, qGroup, [0], mRemoveMember, mGroup, [0], Show, Show, GroupState, GroupState);

        // resetMember, Reset, addCopyWithRONumber when member 0 is gone
        MethodComparison.Compare("resetMember(7 without 0)", Contract, qResetMember, qGroup, [7], mResetMember, mGroup, [7], Show, Show, GroupState, GroupState);
        MethodComparison.Compare("Reset(99 without 0)", Contract, qReset, qGroup, [99], mReset, mGroup, [99], Show, Show, GroupState, GroupState);
        MethodComparison.Compare("addCopyWithRONumber(12 without 0)", Contract, qAddCopy, qGroup, [12], mAddCopy, mGroup, [12], Show, Show, GroupState, GroupState);
    }

    [SkippableFact]
    public void RuntimeOptions_methods_and_sync()
    {
        Runtime.Sync();

        var qOptsCtor = typeof(Q.RuntimeOptions).GetConstructor([])!;
        var mOptsCtor = M.GetConstructor([])!;
        var qSync = typeof(Q.RuntimeOptions).GetMethod("SyncToThread")!;
        var mSync = M.GetMethod("SyncToThread")!;
        var qRemove = typeof(Q.RuntimeOptions).GetMethod("RemoveUnitSystem")!;
        var mRemove = M.GetMethod("RemoveUnitSystem")!;
        var qSave = typeof(Q.RuntimeOptions).GetMethod("SaveThreadOptions")!;
        var mSave = M.GetMethod("SaveThreadOptions")!;

        // Constructor
        MethodComparison.Compare("RuntimeOptions.ctor()", Contract, qOptsCtor, null, [], mOptsCtor, null, [], Show, Show);

        var qOpts = new Q.RuntimeOptions();
        var mOpts = New(M);

        // SyncToThread default
        MethodComparison.Compare("SyncToThread default", Contract, qSync, qOpts, [], mSync, mOpts, [], Show, Show, OptionsState, OptionsState);

        // SaveThreadOptions
        MethodComparison.Compare("SaveThreadOptions synced", Contract, qSave, null, [], mSave, null, [], Show, Show);

        // Modify options and SyncToThread again
        foreach (var f in Flags)
        {
            Set(qOpts, "m_" + f, !(bool)Get(qOpts, "m_" + f)!);
            Set(mOpts, "m_" + f, !(bool)Get(mOpts, "m_" + f)!);
        }
        Set(qOpts, "m_mangled_identifiers", Enum.ToObject(Get(qOpts, "m_mangled_identifiers")!.GetType(), 2));
        Set(mOpts, "m_mangled_identifiers", Enum.ToObject(Get(mOpts, "m_mangled_identifiers")!.GetType(), 2));
        Set(qOpts, "m_ro_number", 17);
        Set(mOpts, "m_ro_number", 17);

        MethodComparison.Compare("SyncToThread modified", Contract, qSync, qOpts, [], mSync, mOpts, [], Show, Show, OptionsState, OptionsState);

        // Add copy with RO number 17 so RemoveUnitSystem has something to remove
        var qGrp = Get(qOpts, "m_unit_system_group")!;
        var mGrp = Get(mOpts, "m_unit_system_group")!;
        Call(qGrp, "addCopyWithRONumber", 17);
        Call(mGrp, "addCopyWithRONumber", 17);

        // RemoveUnitSystem
        MethodComparison.Compare("RemoveUnitSystem()", Contract, qRemove, qOpts, [], mRemove, mOpts, [], Show, Show, OptionsState, OptionsState);

        // RemoveUnitSystem when group is null throws NullReferenceException
        Set(qOpts, "m_unit_system_group", null);
        Set(mOpts, "m_unit_system_group", null);
        MethodComparison.Compare("RemoveUnitSystem() null group", Contract, qRemove, qOpts, [], mRemove, mOpts, [], Show, Show, OptionsState, OptionsState);

        // Initializer constructor
        var qInitType = typeof(Q.RuntimeOptions).GetNestedType("RuntimeOptionsInitializer", BindingFlags.NonPublic)!;
        var mInitType = M.GetNestedType("RuntimeOptionsInitializer", BindingFlags.NonPublic)!;
        var qInitCtor = qInitType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!;
        var mInitCtor = mInitType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!;
        Assert.Equal(StaticState(M), StaticState(typeof(Q.RuntimeOptions)));
        MethodComparison.Compare("RuntimeOptionsInitializer.ctor()", Contract, qInitCtor, null, [], mInitCtor, null, [], Show, Show);
        Assert.Equal(StaticState(M), StaticState(typeof(Q.RuntimeOptions)));
    }

    private static string UnitDetail(object? u) => u == null ? "null"
        : $"{Get(u, "Name")}/{Get(u, "Symbol")}/{Get(u, "Quantity")}/{Text(Get(u, "Magnitude"))}/{Get(u, "Type")}";

    private static string DefaultSystemText(object s) =>
        $"{Get(s, "Name")} code={Get(s, "Code")} guid={Get(s, "Guid")} ro={Get(s, "RO_number")} rank={Get(s, "Rank")}"
        + $" basis=[{string.Join(" ", ((Array)Get(s, "Basis")!).Cast<object?>().Select(UnitDetail))}]"
        + $" defaults=[{string.Join(" ", ((Array)Get(s, "Defaults")!).Cast<object?>().Select(UnitDetail))}]";

    private static string StaticState(System.Type t)
    {
        var parts = new List<string>
        {
            $"s_restricted_mode={Get(t, "s_restricted_mode")}",
            $"s_ro_counter={Get(t, "s_ro_counter")}",
            $"s_default_unit_system={DefaultSystemText(Get(t, "s_default_unit_system")!)}",
            $"s_init={Get(t, "s_init")?.GetType().Name ?? "null"}",
        };
        parts.AddRange(Flags.Select(f => $"{f}={Get(t, f)}"));
        parts.Add($"mangled_identifiers={Get(t, "mangled_identifiers")}");
        parts.Add($"the_ro_number={Get(t, "the_ro_number")}");
        parts.Add($"the_unit_system={SystemText(Get(t, "the_unit_system"))}");
        var group = Get(t, "the_unit_system_group");
        parts.Add($"the_unit_system_group={(group == null ? "null" : GroupState(group))}");
        System.Type dim = t.Assembly == typeof(Q.RuntimeOptions).Assembly ? typeof(Q.Dim) : Mathcad.Type("mplrun", "Mcd.Dim");
        var scratch = (int[]?)Get(dim, "s_vec");
        parts.Add($"Dim.s_vec={(scratch == null ? "null" : "[" + string.Join(", ", scratch) + "]")}");
        return string.Join(", ", parts);
    }

    [SkippableFact]
    public void RuntimeOptions_type_initializer()
    {
        // When this test runs on its own, as mutant runs of the type initializer do, the first touch of either type
        // happens inside this comparison, so a failing initializer is a compared TypeInitializationException rather
        // than an error in setup. In a full run another test may initialize the types first.
        var qOptsCtor = typeof(Q.RuntimeOptions).GetConstructor([])!;
        var mOptsCtor = M.GetConstructor([])!;
        Func<object?, string> options = o => o == null ? "null" : $"{OptionsState(o)}, system={DefaultSystemText(Get(o, "m_unit_system")!)}";
        MethodComparison.Compare("RuntimeOptions.ctor() first in this test", Contract, qOptsCtor, null, [], mOptsCtor, null, [], options, options);
        Runtime.Sync();
        var qCctor = typeof(Q.RuntimeOptions).TypeInitializer!;
        var mCctor = M.TypeInitializer!;
        // The runtime runs a type initializer once, so invoking it again leaves the statics it set up; the outcome is
        // those statics in full (the default system's every unit), as no test changes them.
        MethodComparison.Compare(".cctor()", Contract, qCctor, null, [], mCctor, null, [],
            _ => StaticState(typeof(Q.RuntimeOptions)), _ => StaticState(M));
        Runtime.Sync();
    }
}
