namespace Quire.Values;

/// <summary>A unit system decides how a dimensioned result is displayed by default.</summary>
public sealed class UnitSystem
{
    private readonly Unit[] _base;
    private readonly Unit[] _derived;

    private UnitSystem(string name, string[] baseUnits, string[] derived)
    {
        Name = name;
        _base = baseUnits.Select(n => Units.All[n]).ToArray();
        _derived = derived.Select(n => Units.All[n]).ToArray();
    }

    public string Name { get; }

    public static readonly UnitSystem SI = new("si", ["m", "kg", "s", "A", "K", "mol", "cd"],
        ["N", "Pa", "J", "W", "V", "C", "Ω", "F", "H", "S", "Wb", "T"]);

    public static readonly UnitSystem USCS = new("uscs", ["ft", "lb", "s", "A", "R", "mol", "cd"],
        ["lbf", "psi", "BTU", "hp", "V", "C", "Ω", "F", "H", "S", "Wb", "T"]);

    public static readonly UnitSystem CGS = new("cgs", ["cm", "gm", "s", "A", "K", "mol", "cd"],
        ["dyne", "erg", "V", "C", "Ω", "F", "H", "S", "Wb", "T"]);

    public static IReadOnlyList<UnitSystem> All { get; } = [SI, USCS, CGS];

    public static UnitSystem ByName(string? name) =>
        All.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)) ?? SI;

    /// <summary>Unit factors (unit, exponent) used to display a quantity of dimension <paramref name="dim"/>.</summary>
    public IReadOnlyList<(Unit Unit, double Exponent)> DisplayFactors(Dim dim)
    {
        if (dim.IsNone) return [];
        foreach (var u in _derived)
            if (u.Dim == dim) return [(u, 1)];
        var list = new List<(Unit, double)>();
        for (int i = 0; i < Dim.Count; i++)
            if (dim[i] != 0) list.Add((_base[i], dim[i]));
        return list;
    }

    /// <summary>The SI factor of the display unit for <paramref name="dim"/>; divide an SI value by it.</summary>
    public double DisplayScale(Dim dim) =>
        DisplayFactors(dim).Aggregate(1.0, (acc, f) => acc * Math.Pow(f.Unit.Factor, f.Exponent));
}
