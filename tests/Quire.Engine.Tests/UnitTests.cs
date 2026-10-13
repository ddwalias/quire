using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Engine.Tests;

public class UnitTests : EngineTestBase
{
    [Fact]
    public void Quantities_are_stored_in_SI() =>
        AssertNum(Last("(eval (op_mult 5 km))"), 5000, Dim.Parse("length"));

    [Fact]
    public void Derived_dimensions_combine() =>
        AssertNum(Last("(eval (op_mult (op_mult 2 kg) (op_mult 3 (op_div m (op_power s 2)))))"), 6, Dim.Parse(BuiltinUnits.Force));

    [Fact]
    public void Adding_incompatible_units_is_an_error() =>
        AssertError(Last("(eval (op_add (op_mult 1 m) (op_mult 1 s)))"), "Units don't match");

    [Fact]
    public void Compatible_units_add() =>
        AssertNum(Last("(eval (op_add (op_mult 1 ft) (op_mult 12 in)))"), 0.6096, Dim.Parse("length"));

    [Fact]
    public void Unit_override_returns_display_scale()
    {
        var r = Last("(define v (op_mult 100 (op_div km hr)))", "(eval v (op_div km hr))");
        Assert.Equal(1000.0 / 3600, r.Display!.Scale, 12);
        Assert.Equal(100, Num(r).Real / r.Display.Scale, 9);
    }

    [Fact]
    public void Unit_override_must_match_dimensions() =>
        AssertError(Last("(eval (op_mult 3 m) s)"), "doesn't match");

    [Fact]
    public void Affine_temperature_literals_convert_to_kelvin() =>
        AssertNum(Last("(eval (op_mult 20 °C))"), 293.15, Dim.Parse("temperature"));

    [Fact]
    public void Affine_temperature_override_converts_back()
    {
        var r = Last("(eval (op_mult 300 K) °F)");
        double shown = (Num(r).Real - r.Display!.Offset) / r.Display.Scale;
        Assert.Equal(80.33, shown, 2);
    }

    [Fact]
    public void Square_root_halves_dimensions() =>
        AssertNum(Last("(eval (op_sqrt (op_mult 16 (op_power m 2))))"), 4, Dim.Parse("length"));

    [Fact]
    public void Trig_requires_unitless_angles_and_deg_is_unitless()
    {
        AssertNum(Last("(eval (op_apply cos (op_mult 60 deg)))"), 0.5);
        AssertError(Last("(eval (op_apply cos (op_mult 1 m)))"), "unitless");
    }

    [Fact]
    public void Gravity_constant_has_acceleration_units() =>
        AssertNum(Last("(eval (op_mult (op_mult 10 kg) g))"), 98.0665, Dim.Parse(BuiltinUnits.Force));

    [Fact]
    public void Unit_systems_choose_display_units()
    {
        var force = Dim.Parse(BuiltinUnits.Force);
        Assert.Equal("N", Assert.Single(BuiltinUnits.SI.Simplify(force.Coordinates).Powers).Base.Symbol);
        Assert.Equal("lbf", Assert.Single(BuiltinUnits.USCS.Simplify(force.Coordinates).Powers).Base.Symbol);
        Assert.Equal(0.3048, BuiltinUnits.USCS.Simplify(Dim.Parse("length").Coordinates).Magnitude, 12);
    }
}
