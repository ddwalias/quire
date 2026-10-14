using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Mpl.Tests.Mcd;

/// <summary>
/// Behaviour of <see cref="RuntimeOptions"/> and <see cref="UnitSystemGroup"/> against
/// docs/mirror/evidence/Mcd.RuntimeOptions.md; RuntimeOptionsDifferentialTests compares with Mathcad.
/// </summary>
public class RuntimeOptionsTests
{
    public RuntimeOptionsTests() => RuntimeThread.Ensure();

    private static T OnNewThread<T>(Func<T> f)
    {
        T result = default!;
        var t = new Thread(() => result = f());
        t.Start();
        t.Join();
        return result;
    }

    [Fact]
    public void Defaults_of_a_new_instance()
    {
        var o = new RuntimeOptions();
        Assert.True(o.m_strict_singularity_checking);
        Assert.True(o.m_strings_indexed_from_origin);
        Assert.False(o.m_strict_comparisons);
        Assert.Same(RuntimeOptions.s_default_unit_system, o.m_unit_system);
        Assert.Equal("SI", o.m_unit_system_group.Name);
        Assert.Same(RuntimeOptions.s_default_unit_system, o.m_unit_system_group.findMember(0));
        Assert.True(RuntimeOptions.s_ro_counter >= 1);   // the type initialiser counts its own instance
    }

    [Fact]
    public void Only_synced_threads_have_options()
    {
        Assert.Null(OnNewThread(() => RuntimeOptions.the_unit_system));
        // Dim's type initialiser needs a synced thread; run it here rather than on whichever thread touches Dim first.
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Dim).TypeHandle);
        Assert.Null(OnNewThread(() => Dim.s_vec));
        var o = new RuntimeOptions { m_strict_comparisons = true, m_ro_number = 42 };
        var seen = OnNewThread(() =>
        {
            o.SyncToThread();
            return (RuntimeOptions.strict_comparisons, RuntimeOptions.the_ro_number, RuntimeOptions.the_unit_system, Dim.s_vec != null);
        });
        Assert.Equal((true, 42, RuntimeOptions.s_default_unit_system, true), seen);
    }

    [Fact]
    public void Saving_copies_the_thread_fields()
    {
        var saved = OnNewThread(() =>
        {
            new RuntimeOptions { m_zero_over_zero_is_zero = true, m_ro_number = 7 }.SyncToThread();
            return RuntimeOptions.SaveThreadOptions();
        });
        Assert.True(saved.m_zero_over_zero_is_zero);
        Assert.Equal(7, saved.m_ro_number);
        Assert.Same(RuntimeOptions.s_default_unit_system, saved.m_unit_system);
    }

    [Fact]
    public void Group_members_are_keyed_by_options_number()
    {
        var si = RuntimeOptions.s_default_unit_system;
        var g = new UnitSystemGroup(new UnitSystem("SI", 0, "g", si.Basis, si.Defaults));
        var copy = g.addCopyWithRONumber(3);
        Assert.Equal(3, copy.RO_number);
        Assert.Same(copy, g.addCopyWithRONumber(3));
        Assert.Same(copy, g.findMember(3));
        g.findMember(0).Defaults = [];
        g.resetMember(3);
        Assert.Empty(copy.Defaults);
        Assert.Same(copy, g.Reset(3));
        Assert.NotEmpty(copy.Defaults);    // back to its own construction units
        g.removeMember(0);
        Assert.Null(g.addCopyWithRONumber(4));
        Assert.Null(g.Reset(9));
        g.changeMember(9, [], []);         // no member: nothing happens
        var o = new RuntimeOptions { m_unit_system_group = g, m_ro_number = 3 };
        o.RemoveUnitSystem();
        Assert.Null(g.findMember(3));
    }
}
