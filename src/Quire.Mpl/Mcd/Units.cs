#nullable disable
using System.Text;

namespace Quire.Mcd;

/// <summary>A named unit: a quantity of <see cref="Magnitude"/> SI units of the dimension its signature gives.</summary>
public partial class Unit
{
    public readonly string Name;
    public readonly string Symbol;
    public readonly string Quantity;
    public readonly double Magnitude;
    public readonly Type Type;

    public Unit(string name, string symbol, string quantity, double magnitude, string signature)
    {
        Name = name;
        Symbol = symbol;
        Quantity = quantity;
        Magnitude = magnitude;
        Type = Type.Parse(signature ?? "1");
    }

    public Type.Product Dimensions => (Type.Product)Type;

    public override string ToString() => Symbol;
}

/// <summary>A product of unit powers, such as <c>kg·m·s^-2</c>, used to display a dimension.</summary>
public partial class Units
{
    public readonly Power[] Powers;
    public readonly double Magnitude;

    public Units(Power[] powers)
    {
        Powers = powers ?? [];
        double m = 1;
        foreach (var p in Powers) m *= Math.Pow(p.Base.Magnitude, p.Exponent);
        Magnitude = m;
    }

    public Units(Unit unit, Rational exponent) : this([new Power { Base = unit, Exponent = exponent }]) { }

    public double magnitude => Magnitude;

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var p in Powers)
        {
            if (sb.Length > 0) sb.Append('·');
            sb.Append(p.Base.Symbol);
            if (p.Exponent != 1) sb.Append('^').Append(p.Exponent.ToString());
        }
        return sb.Length == 0 ? "1" : sb.ToString();
    }

    public partial struct Power
    {
        public Unit Base;
        public Rational Exponent;
    }
}

/// <summary>
/// A unit system decides how a dimension is displayed: by one of its <see cref="Defaults"/> (derived units such as
/// N or Pa) when one has exactly that dimension, otherwise as a product of <see cref="Basis"/> units, one per
/// base dimension.
/// </summary>
public partial class UnitSystem : MarshalByRefObject
{
    private string m_name;
    private uint m_code;
    private int m_ro_number;
    private string m_guid;
    private int m_rank;
    private Unit[] m_base;
    private Unit[] m_basis_orig;
    private Unit[] m_defaults_orig;
    private Unit[] m_defaults;

    public UnitSystem(string name, uint code, string guid, Unit[] basis, Unit[] defaults)
    {
        m_name = name;
        m_code = code;
        m_guid = guid;
        m_rank = basis?.Length ?? 0;
        m_base = basis ?? [];
        m_defaults = defaults ?? [];
        m_basis_orig = m_base;
        m_defaults_orig = m_defaults;
    }

    public string Name => m_name;
    public uint Code => m_code;
    public string Guid => m_guid;
    public int Rank => m_rank;

    public Unit[] Basis
    {
        get => m_base;
        set { m_base = value ?? []; m_rank = m_base.Length; }
    }

    public Unit[] Defaults
    {
        get => m_defaults;
        set => m_defaults = value ?? [];
    }

    public int RO_number
    {
        get => m_ro_number;
        set => m_ro_number = value;
    }

    /// <summary>A copy with the original basis and defaults restored.</summary>
    public UnitSystem Reset() => new(m_name, m_code, m_guid, m_basis_orig, m_defaults_orig) { RO_number = m_ro_number };

    /// <summary>The display units for a dimension given by its <see cref="Dim.Coordinates"/>.</summary>
    public Units Simplify(int[] global)
    {
        if (global == null || global.All(c => c == 0)) return new Units([]);
        foreach (var u in m_defaults)
            if (GetCoordinates(u.Dimensions).AsSpan().SequenceEqual(global)) return new Units(u, 1);
        var powers = new List<Units.Power>();
        for (int i = 0; i < global.Length && i < m_rank; i++)
            if (global[i] != 0)
                powers.Add(new Units.Power { Base = m_base[i], Exponent = new Rational(global[i], Dim.Denominator) });
        return new Units([.. powers]);
    }

    public Units Simplify(Type.Product product) => Simplify(GetCoordinates(product));

    public Units Simplify(string signature) => Simplify((Type.Product)Type.Parse(signature));

    public int[] GetCoordinates(Type.Product product) => Dim.New(product).Coordinates;

    public override string ToString() => m_name;
}
