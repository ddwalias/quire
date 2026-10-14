using Quire.Mcd;
using Quire.Mpl.Internal;
using Quire.Mpl.Run;
using Type = Quire.Mcd.Type;

namespace Quire.Mpl.Tests.Mcd;

/// <summary>
/// Behaviour of <see cref="Unit"/>, <see cref="Units"/> and <see cref="UnitSystem"/> against
/// docs/mirror/evidence/Mcd.Units.md and Mcd.UnitSystem.md; UnitSystemDifferentialTests compares with Mathcad.
/// </summary>
public class UnitSystemTests
{
    public UnitSystemTests() => RuntimeThread.Ensure();

    private static UnitSystem Si => RuntimeOptions.s_default_unit_system;

    private static Unit U(string symbol, string quantity, double magnitude, string signature) => new(symbol, symbol, quantity, magnitude, signature);

    [Fact]
    public void Coordinates_follow_sorted_dimension_names()
    {
        Assert.Equal(["Current", "Length", "Luminosity", "Mass", "Money", "Substance", "Temperature", "Time"], Si.Dimensions.Select(d => d.Name));
        Assert.Equal([0, 60000, 0, 60000, 0, 0, 0, -120000], Si.GetCoordinates((Type.Product)Type.Parse("{Time-120000 Mass 60000 Length 60000}")));
        Assert.Equal("{Length 60000 Time -60000}", Si.GetDimensions([0, 60000, 0, 0, 0, 0, 0, -60000]).ToString());
        var e = Assert.Throws<ArgumentException>(() => Si.GetCoordinates((Type.Product)Type.Parse("{Foo 60000}")));
        Assert.Equal("Foo", e.ParamName);
    }

    [Fact]
    public void Simplify_prefers_a_dividing_default()
    {
        Assert.Equal("{N}", Si.Simplify("V{Length 60000 Mass 60000 Time-120000}").ToString());
        Assert.Equal("{N 2}", Si.Simplify("V{Length 120000 Mass 120000 Time-240000}").ToString());
        Assert.Equal("{L}", Si.Simplify("V{Length 180000}").ToString());
        Assert.Equal("{m 2}", Si.Simplify("V{Length 120000}").ToString());   // 120000 isn't a multiple of the liter's 180000
        Assert.Equal("{m 1/2}", Si.Simplify("V{Length 30000}").ToString());
        Assert.Equal("{}", Si.Simplify(new int[8]).ToString());
    }

    [Fact]
    public void Of_several_defaults_the_smallest_power_wins()
    {
        var force = "V{Length 60000 Mass 60000 Time-120000}";
        var basis = RuntimeOptions.s_default_unit_system.Basis;
        var s = new UnitSystem("s", 1, "g", basis, [U("kN", "Force", 1000, force), U("N2", "Force squared", 1, "V{Length 120000 Mass 120000 Time-240000}")]);
        Assert.Equal("{kN}", s.Simplify(force).ToString());
        Assert.Equal("{N2}", s.Simplify("V{Length 120000 Mass 120000 Time-240000}").ToString());
        Assert.Equal("{N2 -1}", s.Simplify("V{Length-120000 Mass-120000 Time 240000}").ToString());
        var quiet = new UnitSystem("q", 1, "g", basis, [U("NOSIMPLIFY", "Velocity", 1, "V{Length 60000 Time-60000}")]);
        Assert.Equal("{m s -1}", quiet.Simplify("V{Length 60000 Time-60000}").ToString());
        var crowded = new UnitSystem("c", 1, "g", basis, [U("a", "F", 1, force), U("b", "F", 2, force), U("c", "F", 3, force)]);
        Assert.Throws<IndexOutOfRangeException>(() => crowded.Defaults);
    }

    [Fact]
    public void Basis_change_inverts_a_composite_basis()
    {
        var basis = Si.Basis.ToArray();
        int seconds = Array.FindIndex(basis, u => u.Symbol == "s");
        basis[seconds] = U("Hz", "Frequency", 1, "V{Time-60000}");
        var s = new UnitSystem("hz", 1, "g", basis, []);
        Assert.Equal("{Hz -1}", s.Normalize("V{Time 60000}").ToString());
        // global coordinates follow the sorted dimension names; local ones follow the basis units
        var local = new double[8];
        local[seconds] = -1;
        Assert.Equal(local, s.ChangeBasis([0, 0, 0, 0, 0, 0, 0, 60000]));
        Assert.Equal([0, 0, 0, 0, 0, 0, 0, 60000], s.ChangeBasis(local));
        basis[seconds] = basis[0];
        var e = Assert.Throws<ArgumentException>(() => new UnitSystem("bad", 1, "g", basis, []));
        Assert.Equal("value", e.ParamName);
        Assert.Contains("singular matrix", e.InnerException!.Message);
    }

    [Fact]
    public void Short_bases_are_padded_with_an_ampere()
    {
        var s = new UnitSystem("seven", 1, "g", [.. Si.Basis.Where(u => u.Symbol != "A")], []);
        Assert.Equal(8, s.Rank);
        Assert.Equal("A", s.Basis[7].Symbol);
        Assert.True(s.IsUnitSymbol("A"));
        Assert.Throws<NullReferenceException>(() => new UnitSystem("empty", 1, "g", [], []));
    }

    [Fact]
    public void Reset_restores_construction_units_in_place()
    {
        var s = new UnitSystem("s", 2, "g", Si.Basis, Si.Defaults);
        s.Defaults = [];
        Assert.Equal("{m s -1}", s.Simplify("V{Length 60000 Time-60000}").ToString());
        Assert.Same(s, s.Reset());
        Assert.Equal("{N}", s.Simplify("V{Length 60000 Mass 60000 Time-120000}").ToString());
        Assert.Equal(("s", 2u, "g", "s"), (s.Name, s.Code, s.Guid, s.ToString()));
        s.RO_number = 4;
        Assert.Equal(4, s.RO_number);
    }

    [Fact]
    public void Units_magnitude_and_text()
    {
        var ft = U("ft", "Length", 0.3048, "V{Length 60000}");
        var u = new Units([new Units.Power { Base = ft, Exponent = 2 }, new Units.Power { Base = U("s", "Time", 1, "V{Time 60000}"), Exponent = new Rational(-1, 2) }]);
        Assert.Equal(0.3048 * 0.3048, u.Magnitude, 15);
        Assert.Equal(u.Magnitude, u.magnitude);
        Assert.Equal("{ft 2 s -1/2}", u.ToString());
        Assert.Equal("floating point error", Assert.Throws<AppException>(() => new Units([new Units.Power { Base = U("z", "Q", 0, "V{}"), Exponent = 1 }])).Message);
        Assert.Equal("illegal factor", Assert.Throws<AppException>(() => new Units(U("z", "Q", 0, "V{}"), 1)).Message);
        Assert.Equal("floating point error", Assert.Throws<AppException>(() => new Units(U("i", "Q", double.PositiveInfinity, "V{}"), 1)).Message);
    }

    [Fact]
    public void Unit_text_and_dimensions()
    {
        var u = U("N", "Force", 1.5, "V{Length 60000}");
        Assert.Equal("Unit{name=\"N\",symbol=\"N\",quantity=\"Force\",magnitude=1.5,type=\"(V {Length 60000})\"}", u.ToString());
        Assert.Equal("{Length 60000}", u.Dimensions.ToString());
        Assert.Null(U("f", "Q", 1, "a->b").Dimensions);
    }

    [Fact]
    public void Units_of_products()
    {
        Assert.Null(Utilities.GetUnits((Type.Product)Type.Parse("{}")));
        Assert.Null(Utilities.GetUnits((Type.Product)Type.Parse("{a Length 60000}")));
        Assert.Equal("{kg m s -2}", Utilities.GetUnits((Type.Product)Type.Parse("{Length 60000 Mass 60000 Time-120000}"))!.ToString());
    }
}
