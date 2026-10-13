namespace Quire.Values;

/// <summary>A named unit: <c>value_in_SI = magnitude * Factor</c>.</summary>
public sealed record Unit(string Name, double Factor, Dim Dim, string Quantity = "");

/// <summary>An affine temperature scale: <c>K = value * Scale + Offset</c>.</summary>
public sealed record AffineUnit(string Name, double Scale, double Offset);

/// <summary>The built-in unit table. Names follow engineering-worksheet conventions
/// (<c>gm</c> is gram because <c>g</c> is standard gravity).</summary>
public static class Units
{
    private static readonly Dictionary<string, Unit> Table = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, AffineUnit> AffineTable = new(StringComparer.Ordinal);

    public static readonly Dim Force = new(1, 1, -2), Pressure = new(-1, 1, -2), Energy = new(2, 1, -2),
        Power = new(2, 1, -3), Voltage = new(2, 1, -3, -1), Charge = new(0, 0, 1, 1), Resistance = new(2, 1, -3, -2),
        Capacitance = new(-2, -1, 4, 2), Inductance = new(2, 1, -2, -2), Conductance = new(-2, -1, 3, 2),
        MagneticFlux = new(2, 1, -2, -1), FluxDensity = new(0, 1, -2, -1), Frequency = new(time: -1),
        Area = new(2), Volume = new(3), Velocity = new(1, 0, -1), Acceleration = new(1, 0, -2);

    static Units()
    {
        Def("Length", Dim.Length, ("m", 1), ("km", 1e3), ("cm", 1e-2), ("mm", 1e-3), ("μm", 1e-6), ("um", 1e-6),
            ("nm", 1e-9), ("Å", 1e-10), ("in", 0.0254), ("ft", 0.3048), ("yd", 0.9144), ("mi", 1609.344),
            ("mil", 2.54e-5), ("nmi", 1852));
        Def("Mass", Dim.Mass, ("kg", 1), ("gm", 1e-3), ("mg", 1e-6), ("tonne", 1000), ("lb", 0.45359237),
            ("lbm", 0.45359237), ("oz", 0.028349523125), ("ton", 907.18474), ("slug", 14.593902937206364));
        Def("Time", Dim.Time, ("s", 1), ("sec", 1), ("ms", 1e-3), ("μs", 1e-6), ("us", 1e-6), ("ns", 1e-9),
            ("min", 60), ("hr", 3600), ("day", 86400), ("wk", 604800), ("yr", 31556952));
        Def("Current", Dim.Current, ("A", 1), ("mA", 1e-3), ("kA", 1e3), ("μA", 1e-6), ("uA", 1e-6));
        Def("Temperature", Dim.Temperature, ("K", 1), ("R", 5.0 / 9.0), ("Δ°C", 1), ("Δ°F", 5.0 / 9.0));
        Def("Amount", Dim.Amount, ("mol", 1), ("kmol", 1e3));
        Def("Luminosity", Dim.Luminosity, ("cd", 1));
        Def("Angle", Dim.None, ("rad", 1), ("deg", Math.PI / 180), ("°", Math.PI / 180), ("rev", 2 * Math.PI),
            ("sr", 1));
        Def("Force", Force, ("N", 1), ("kN", 1e3), ("MN", 1e6), ("dyne", 1e-5), ("lbf", 4.4482216152605),
            ("kip", 4448.2216152605), ("kgf", 9.80665));
        Def("Pressure", Pressure, ("Pa", 1), ("kPa", 1e3), ("MPa", 1e6), ("GPa", 1e9), ("bar", 1e5),
            ("atm", 101325), ("psi", 6894.757293168361), ("ksi", 6894757.293168361), ("torr", 101325.0 / 760),
            ("mmHg", 133.322387415), ("mm_Hg", 133.322387415), ("inHg", 3386.389), ("in_Hg", 3386.389));
        Def("Energy", Energy, ("J", 1), ("kJ", 1e3), ("MJ", 1e6), ("cal", 4.184), ("kcal", 4184),
            ("BTU", 1055.05585262), ("kWh", 3.6e6), ("ft·lbf", 1.3558179483314004),
            ("in·lbf", 0.1129848290276167), ("eV", 1.602176634e-19), ("erg", 1e-7));
        Def("Power", Power, ("W", 1), ("kW", 1e3), ("MW", 1e6), ("mW", 1e-3), ("hp", 745.69987158227022));
        Def("Voltage", Voltage, ("V", 1), ("kV", 1e3), ("mV", 1e-3));
        Def("Charge", Charge, ("C", 1));
        Def("Resistance", Resistance, ("Ω", 1), ("ohm", 1), ("kΩ", 1e3), ("MΩ", 1e6));
        Def("Capacitance", Capacitance, ("F", 1), ("μF", 1e-6), ("uF", 1e-6), ("nF", 1e-9), ("pF", 1e-12));
        Def("Inductance", Inductance, ("H", 1), ("mH", 1e-3));
        Def("Conductance", Conductance, ("S", 1));
        Def("Magnetic flux", MagneticFlux, ("Wb", 1));
        Def("Flux density", FluxDensity, ("T", 1));
        Def("Frequency", Frequency, ("Hz", 1), ("kHz", 1e3), ("MHz", 1e6), ("GHz", 1e9), ("rpm", 2 * Math.PI / 60));
        Def("Volume", Volume, ("L", 1e-3), ("mL", 1e-6), ("gal", 0.003785411784));
        Def("Area", Area, ("hectare", 1e4), ("acre", 4046.8564224));
        Def("Velocity", Velocity, ("mph", 0.44704), ("kph", 1000.0 / 3600), ("knot", 1852.0 / 3600));

        AffineTable["°C"] = new AffineUnit("°C", 1, 273.15);
        AffineTable["degC"] = new AffineUnit("°C", 1, 273.15);
        AffineTable["°F"] = new AffineUnit("°F", 5.0 / 9.0, 459.67 * 5.0 / 9.0);
        AffineTable["degF"] = new AffineUnit("°F", 5.0 / 9.0, 459.67 * 5.0 / 9.0);
    }

    private static void Def(string quantity, Dim dim, params (string Name, double Factor)[] units)
    {
        foreach (var (name, factor) in units) Table[name] = new Unit(name, factor, dim, quantity);
    }

    public static IReadOnlyDictionary<string, Unit> All => Table;
    public static IReadOnlyDictionary<string, AffineUnit> Affine => AffineTable;

    public static bool TryGet(string name, out Unit unit) => Table.TryGetValue(name, out unit!);
    public static bool TryGetAffine(string name, out AffineUnit unit) => AffineTable.TryGetValue(name, out unit!);
    public static bool IsUnit(string name) => Table.ContainsKey(name) || AffineTable.ContainsKey(name);

    /// <summary>Predefined constants (π, e, g, ∞).</summary>
    public static IReadOnlyDictionary<string, NumVal> Constants { get; } = new Dictionary<string, NumVal>
    {
        ["π"] = new(Math.PI), ["pi"] = new(Math.PI), ["e"] = new(Math.E),
        ["g"] = new(9.80665, Acceleration), ["∞"] = new(double.PositiveInfinity),
    };
}
