using Quire.Mcd;

namespace Quire.Mpl.Internal;

/// <summary>An affine temperature scale: <c>K = value * Scale + Offset</c>.</summary>
internal sealed record AffineUnit(string Name, double Scale, double Offset);

/// <summary>
/// Quire's built-in unit table, constants and unit systems. Names follow engineering-worksheet conventions
/// (<c>gm</c> is gram because <c>g</c> is standard gravity). Mathcad keeps its tables in compiled Mpl modules
/// that metadata doesn't show, so this table is Quire's own.
/// </summary>
internal static class BuiltinUnits
{
    private static readonly Dictionary<string, Unit> Table = new(StringComparer.Ordinal);
    private static readonly Dictionary<Unit, Dim> Dims = [];
    private static readonly Dictionary<string, AffineUnit> AffineTable = new(StringComparer.Ordinal);

    // Signatures in Mathcad's type syntax: base-dimension exponents scaled by 60000.
    public const string Length = "V{Length 60000}", Mass = "V{Mass 60000}", Time = "V{Time 60000}",
        Current = "V{Current 60000}", Temperature = "V{Temperature 60000}", Substance = "V{Substance 60000}",
        Luminosity = "V{Luminosity 60000}", Money = "V{Money 60000}", Unitless = "V{}",
        Force = "V{Length 60000 Mass 60000 Time-120000}", Pressure = "V{Length-60000 Mass 60000 Time-120000}",
        Energy = "V{Length 120000 Mass 60000 Time-120000}", Power = "V{Length 120000 Mass 60000 Time-180000}",
        Voltage = "V{Current-60000 Length 120000 Mass 60000 Time-180000}", Charge = "V{Current 60000 Time 60000}",
        Resistance = "V{Current-120000 Length 120000 Mass 60000 Time-180000}",
        Capacitance = "V{Current 120000 Length-120000 Mass-60000 Time 240000}",
        Inductance = "V{Current-120000 Length 120000 Mass 60000 Time-120000}",
        Conductance = "V{Current 120000 Length-120000 Mass-60000 Time 180000}",
        MagneticFlux = "V{Current-60000 Length 120000 Mass 60000 Time-120000}",
        FluxDensity = "V{Current-60000 Mass 60000 Time-120000}", Frequency = "V{Time-60000}",
        Area = "V{Length 120000}", Volume = "V{Length 180000}", Velocity = "V{Length 60000 Time-60000}",
        Acceleration = "V{Length 60000 Time-120000}";

    static BuiltinUnits()
    {
        RuntimeThread.Ensure();
        Def("Length", Length, ("m", 1), ("km", 1e3), ("cm", 1e-2), ("mm", 1e-3), ("μm", 1e-6), ("um", 1e-6),
            ("nm", 1e-9), ("Å", 1e-10), ("in", 0.0254), ("ft", 0.3048), ("yd", 0.9144), ("mi", 1609.344),
            ("mil", 2.54e-5), ("nmi", 1852));
        Def("Mass", Mass, ("kg", 1), ("gm", 1e-3), ("mg", 1e-6), ("tonne", 1000), ("lb", 0.45359237),
            ("lbm", 0.45359237), ("oz", 0.028349523125), ("ton", 907.18474), ("slug", 14.593902937206364));
        Def("Time", Time, ("s", 1), ("sec", 1), ("ms", 1e-3), ("μs", 1e-6), ("us", 1e-6), ("ns", 1e-9),
            ("min", 60), ("hr", 3600), ("day", 86400), ("wk", 604800), ("yr", 31556952));
        Def("Current", Current, ("A", 1), ("mA", 1e-3), ("kA", 1e3), ("μA", 1e-6), ("uA", 1e-6));
        Def("Temperature", Temperature, ("K", 1), ("R", 5.0 / 9.0), ("Δ°C", 1), ("Δ°F", 5.0 / 9.0));
        Def("Amount", Substance, ("mol", 1), ("kmol", 1e3));
        Def("Luminosity", Luminosity, ("cd", 1));
        Def("Angle", Unitless, ("rad", 1), ("deg", Math.PI / 180), ("°", Math.PI / 180), ("rev", 2 * Math.PI),
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

        SI = System("si", 1, ["m", "kg", "s", "A", "K", "mol", "cd"], ["N", "Pa", "J", "W", "V", "C", "Ω", "F", "H", "S", "Wb", "T"]);
        USCS = System("uscs", 2, ["ft", "lb", "s", "A", "R", "mol", "cd"], ["lbf", "psi", "BTU", "hp", "V", "C", "Ω", "F", "H", "S", "Wb", "T"]);
        CGS = System("cgs", 3, ["cm", "gm", "s", "A", "K", "mol", "cd"], ["dyne", "erg", "V", "C", "Ω", "F", "H", "S", "Wb", "T"]);
        AllSystems = [SI, USCS, CGS];
        Constants = new Dictionary<string, NumVal>
        {
            ["π"] = NumVal.RealDv(Math.PI), ["pi"] = NumVal.RealDv(Math.PI), ["e"] = NumVal.RealDv(Math.E),
            ["g"] = new(9.80665, 0, Dim.Parse(Acceleration)), ["∞"] = NumVal.RealDv(double.PositiveInfinity),
        };
    }

    private static void Def(string quantity, string signature, params (string Name, double Factor)[] units)
    {
        var dim = Dim.Parse(signature);
        foreach (var (name, factor) in units)
        {
            var u = new Unit(name, name, quantity, factor, signature);
            Table[name] = u;
            Dims[u] = dim;
        }
    }

    /// <summary>Money has no built-in unit, so every basis ends with Mathcad's base currency.</summary>
    private static readonly Unit BaseCurrency = new("base_currency", "¤", "Money", 1.0, Money);

    private static UnitSystem System(string name, uint code, string[] basis, string[] defaults) =>
        new(name, code, "", [.. basis.Select(n => Table[n]), BaseCurrency], defaults.Select(n => Table[n]).ToArray());

    public static UnitSystem SI { get; }
    public static UnitSystem USCS { get; }
    public static UnitSystem CGS { get; }
    public static IReadOnlyList<UnitSystem> AllSystems { get; }

    /// <summary>The system named <paramref name="name"/> (si, uscs or cgs); SI when unknown.</summary>
    public static UnitSystem ByName(string? name) =>
        AllSystems.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)) ?? SI;

    public static IReadOnlyDictionary<string, Unit> All => Table;

    public static IReadOnlyDictionary<string, AffineUnit> Affine => AffineTable;

    public static bool TryGet(string name, out Unit unit) => Table.TryGetValue(name, out unit!);

    public static bool TryGetAffine(string name, out AffineUnit unit) => AffineTable.TryGetValue(name, out unit!);

    public static bool IsUnit(string name) => Table.ContainsKey(name) || AffineTable.ContainsKey(name);

    /// <summary>The dimension of a built-in unit.</summary>
    public static Dim DimOf(Unit unit) => Dims.TryGetValue(unit, out var d) ? d : Dim.New(unit.Dimensions);

    /// <summary>Predefined constants (π, e, g, ∞).</summary>
    public static IReadOnlyDictionary<string, NumVal> Constants { get; }

    /// <summary>The display units of <paramref name="dim"/> in <paramref name="system"/>.</summary>
    public static Units DisplayUnits(UnitSystem system, Dim dim) =>
        Dim.IsDimensioned(dim) ? system.Simplify(dim.Coordinates) : new Units([]);
}
