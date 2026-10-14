using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

/// <summary>
/// Behaviour of <see cref="Dim"/> against docs/mirror/evidence/Mcd.Dim.md. Expected values are worked out from that
/// contract; DimDifferentialTests compares with Mathcad directly.
/// </summary>
public class DimTests
{
    public DimTests() => RuntimeThread.Ensure();

    private static Dim D(params int[] c) => Dim.Parse("V{" + string.Join(" ", c.Select((x, i) => (x, i)).Where(p => p.x != 0)
        .Select(p => $"{RuntimeOptions.s_default_unit_system.Dimensions[p.i].Name} {p.x}")) + "}");

    [Fact]
    public void Equal_coordinates_give_the_same_object()
    {
        var a = Dim.Parse("V{Length 60000 Time-60000}");
        var b = Dim.Mult(Dim.Parse("V{Length 60000}"), Dim.Parse("V{Time-60000}"));
        Assert.Same(a, b);
        Assert.Same(Dim.identity, Dim.Parse("V{}"));
        Assert.Same(Dim.identity, Dim.Div(a, a));
        Assert.Equal([0, 60000, 0, 0, 0, 0, 0, -60000], a.Coordinates);
    }

    [Fact]
    public void Interning_hands_the_thread_a_new_scratch_vector()
    {
        var scratch = Dim.s_vec;
        var fresh = Dim.Mult(D(0, 0, 0, 0, 0, 0, 0, 60000 * 7), D(0, 0, 0, 0, 0, 0, 0, 60000 * 4));
        Assert.Same(scratch, fresh.Coordinates);
        Assert.NotSame(scratch, Dim.s_vec);
        var old = Dim.s_vec;
        Assert.Same(fresh, Dim.Mult(D(0, 0, 0, 0, 0, 0, 0, 60000 * 7), D(0, 0, 0, 0, 0, 0, 0, 60000 * 4)));
        Assert.Same(old, Dim.s_vec);   // already interned: the scratch vector stays
    }

    [Fact]
    public void Markers_are_distinct_zero_vectors_equal_to_identity()
    {
        Dim[] markers = [Dim.mcd_zero, Dim.string_dim, Dim.function_dim, Dim.mixed_matrix_dim];
        Assert.Equal(4, markers.Distinct(ReferenceEqualityComparer.Instance).Count());
        foreach (var m in markers)
        {
            Assert.NotSame(Dim.identity, m);
            Assert.True(m.Equals(Dim.identity));
            Assert.Equal(Dim.identity.GetHashCode(), m.GetHashCode());
            Assert.Equal(new int[8], m.Coordinates);
        }
        Assert.Equal("String", Dim.string_dim.Format());
        Assert.Equal("Function", Dim.function_dim.Format());
        Assert.Equal("Unitless", Dim.mixed_matrix_dim.Format());
    }

    [Fact]
    public void Hash_packs_truncated_unit_powers()
    {
        // Length (index 1) power 3 → 3 << 4; Time (index 7) power -1 → -1 << 28; a half power truncates to 0
        Assert.Equal(3 << 4 ^ -1 << 28, D(0, 180000, 0, 0, 0, 0, 0, -60000).GetHashCode());
        Assert.Equal(0, D(0, 30000).GetHashCode());
        Assert.Equal(1 << 4, D(0, 9 * 60000).GetHashCode());   // 9 mod 8
        Assert.Equal(D(0, 60000).GetHashCode(), D(0, 9 * 60000).GetHashCode());
        Assert.NotSame(D(0, 60000), D(0, 9 * 60000));          // equal hashes, unequal vectors
        Assert.False(D(0, 60000).Equals(D(0, 9 * 60000)));
        Assert.False(D(0, 60000).Equals("x"));
    }

    [Fact]
    public void Products_and_quotients_of_special_dimensions()
    {
        var len = D(0, 60000);
        Assert.Same(len, Dim.Mult(Dim.identity, len));
        Assert.Null(Dim.Mult(len, null));
        Assert.Same(Dim.identity, Dim.Mult(Dim.identity, Dim.mcd_zero));
        Assert.Null(Dim.Mult(Dim.mcd_zero, null));
        Assert.Same(len, Dim.Mult(Dim.string_dim, len));        // markers count as zero coordinates
        Assert.Same(len, Dim.Div(len, Dim.identity));
        Assert.Null(Dim.Div(null, len));
        Assert.Null(Dim.Div(len, null));
        Assert.Same(Dim.identity, Dim.Div(Dim.string_dim, Dim.string_dim));
        Assert.Same(D(0, -60000), Dim.Div(Dim.function_dim, len));
    }

    [Fact]
    public void Powers_round_half_to_even_without_complaint()
    {
        var len = D(0, 60000);
        Assert.Same(len, Dim.Pwr(len, 1));
        Assert.Same(Dim.identity, Dim.Pwr(len, 0));
        Assert.Same(D(0, 20000), Dim.Root(len, 3));
        Assert.Same(D(0, 2), Dim.Pwr(D(0, 5), 0.5));    // 2.5 → 2
        Assert.Same(D(0, 4), Dim.Pwr(D(0, 7), 0.5));    // 3.5 → 4
        Assert.Same(D(0, int.MaxValue), Dim.Pwr(len, double.PositiveInfinity));   // 0 × ∞ is NaN, which converts to 0
        Assert.Same(Dim.identity, Dim.Pwr(len, double.NaN));
        Assert.Null(Dim.Pwr(null, 2));
    }

    [Fact]
    public void Predicates_on_special_dimensions()
    {
        Assert.True(Dim.Id_b(null));
        Assert.True(Dim.Id_b(Dim.mcd_zero));
        Assert.False(Dim.Id_b(Dim.string_dim));
        Assert.True(Dim.Eq_b(Dim.mcd_zero, D(0, 60000)));
        Assert.False(Dim.Eq_b1(Dim.mcd_zero, D(0, 60000)));
        Assert.True(Dim.NonScalar_b(Dim.mixed_matrix_dim));
        Assert.True(Dim.IsSpecial(null));
        Assert.False(Dim.IsSpecial(Dim.identity));
        Assert.Same(Dim.identity, Dim.FixDimension(null));
        Assert.Same(Dim.identity, Dim.ValDim(Dim.mcd_zero));
        Assert.Same(Dim.identity, Dim.intern(Dim.mcd_zero));
    }

    [Fact]
    public void Mismatches_raise_tab_separated_errors()
    {
        var e = Assert.Throws<AppException>(() => Dim.Eq(D(0, 60000), D(0, 0, 0, 0, 0, 0, 0, 60000)));
        Assert.Equal("bad_dimensions\tLength\tTime", e.Message);
        Assert.Equal(2u, e.Argument);
        var f = Assert.Throws<AppException>(() => Dim.Id(D(0, 60000), 5));
        Assert.Equal(5u, f.Argument);
        Assert.Equal("bad_dimensions_unitless\tUnitless\tLength", f.Message);
    }

    [Fact]
    public void Polymorphic_and_null_products()
    {
        Assert.Null(Dim.Parse("V{a Length 60000}"));
        Assert.Equal(NonCriticalException.ArgumentIsNull, Assert.Throws<NonCriticalException>(() => Dim.New(null!)).Message);
        Assert.Equal(NonCriticalException.ArgumentIsNull, Assert.Throws<NonCriticalException>(() => Dim.Parse("a->b")).Message);
    }

    [Fact]
    public void Other_threads_need_their_own_scratch_vector()
    {
        Exception? before = null, after = null;
        var t = new Thread(() =>
        {
            new RuntimeOptions().SyncToThread();
            Dim.s_vec = null!;
            before = Record.Exception(() => Dim.Mult(D(0, 60000), D(0, 60000)));
            Dim.ThreadInit();
            after = Record.Exception(() => Dim.Mult(D(0, 60000), D(0, 60000)));
            Dim.ThreadInit();
        });
        t.Start();
        t.Join();
        Assert.IsType<NullReferenceException>(before);
        Assert.Null(after);
    }
}
