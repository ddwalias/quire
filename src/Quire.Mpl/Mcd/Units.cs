#nullable disable
using System.Collections;
using System.Text;
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

/// <summary>A named unit: <see cref="Magnitude"/> base units of the dimension its signature gives
/// (docs/mirror/evidence/Mcd.Units.md).</summary>
[Serializable]
public partial class Unit
{
    [MirrorVerified("UnitSystemDifferentialTests")]
    public readonly string Name;
    [MirrorVerified("UnitSystemDifferentialTests")]
    public readonly string Symbol;
    [MirrorVerified("UnitSystemDifferentialTests")]
    public readonly string Quantity;
    [MirrorVerified("UnitSystemDifferentialTests")]
    public readonly double Magnitude;
    [MirrorVerified("UnitSystemDifferentialTests")]
    public readonly Type Type;

    [MirrorVerified("UnitSystemDifferentialTests")]
    public Unit(string name, string symbol, string quantity, double magnitude, string signature)
    {
        Name = name;
        Symbol = symbol;
        Quantity = quantity;
        Magnitude = magnitude;
        Type = Type.Parse(signature);
    }

    [MirrorVerified("UnitSystemDifferentialTests")]
    public Type.Product Dimensions => Type.GetDimensionTypeProduct();

    /// <summary>All five fields; the magnitude in the current culture.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public override string ToString() =>
        string.Format("Unit{{name=\"{0}\",symbol=\"{1}\",quantity=\"{2}\",magnitude={3},type=\"{4}\"}}", Name, Symbol, Quantity, Magnitude, Type);
}

/// <summary>A product of unit powers such as <c>kg·m·s^-2</c>, and its magnitude in base units.</summary>
[Serializable]
public partial class Units
{
    [MirrorVerified("UnitSystemDifferentialTests")]
    public readonly Power[] Powers;
    [MirrorVerified("UnitSystemDifferentialTests")]
    public readonly double Magnitude;

    /// <summary>The magnitude must be finite and nonzero ("floating point error" otherwise).</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units(Power[] powers)
    {
        Powers = powers;
        Magnitude = 1.0;
        foreach (var p in powers) Magnitude *= Math.Pow(p.Base.Magnitude, p.Exponent);
        Utilities.AssertFinite(Magnitude);
        Utilities.AssertFinite(1.0 / Magnitude);
    }

    /// <summary>One power; a zero magnitude is an "illegal factor", an infinite one a "floating point error".</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units(Unit unit, Rational exponent)
    {
        Powers = [new Power { Base = unit, Exponent = exponent }];
        Magnitude = Math.Pow(unit.Magnitude, exponent);
        if (Magnitude == 0.0) AppException.Raise("illegal factor");
        Utilities.AssertFinite(Magnitude);
    }

    [MirrorVerified("UnitSystemDifferentialTests")]
    public double magnitude => Magnitude;

    /// <summary><c>{kg m 2 s -3}</c>: symbols, each followed by its exponent unless that is 1.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public override string ToString()
    {
        var sb = new StringBuilder(256);
        sb.Append('{');
        foreach (var p in Powers)
        {
            if (sb.Length > 1) sb.Append(' ');
            sb.Append(p.Base.Symbol);
            if (p.Exponent != 1)
            {
                sb.Append(' ');
                sb.Append(p.Exponent.ToString());
            }
        }
        sb.Append('}');
        return sb.ToString();
    }

    [Serializable]
    public partial struct Power
    {
        [MirrorVerified("UnitSystemDifferentialTests")]
        public Unit Base;
        [MirrorVerified("UnitSystemDifferentialTests")]
        public Rational Exponent;
    }
}

/// <summary>
/// A unit system: a basis of units, one per base dimension, and default units for derived dimensions. Dimension
/// coordinates are indexed by the basis's dimension names in sorted order; <see cref="Simplify(int[])"/> picks a
/// default unit for a dimension when one fits, and <see cref="Normalize(int[])"/> writes it in basis units.
/// </summary>
public partial class UnitSystem : MarshalByRefObject
{
    [MirrorTested("Mcd.UnitSystem")]
    private string m_name;
    [MirrorTested("Mcd.UnitSystem")]
    private uint m_code;
    [MirrorTested("Mcd.UnitSystem")]
    private int m_ro_number;
    [MirrorTested("Mcd.UnitSystem")]
    private string m_guid;
    [MirrorTested("Mcd.UnitSystem")]
    private int m_rank;
    [MirrorTested("Mcd.UnitSystem")]
    private Unit[] m_base;
    [MirrorTested("Mcd.UnitSystem")]
    private Type[] m_dims;
    [MirrorTested("Mcd.UnitSystem")]
    private double[,] m_ltog;
    [MirrorTested("Mcd.UnitSystem")]
    private double[,] m_gtol;
    [MirrorTested("Mcd.UnitSystem")]
    private Hashtable m_defs;
    [MirrorTested("Mcd.UnitSystem")]
    private Unit[] m_basis_orig;
    [MirrorTested("Mcd.UnitSystem")]
    private Unit[] m_defaults_orig;
    [MirrorTested("Mcd.UnitSystem")]
    private Hashtable m_unit_syms = new();

    [MirrorVerified("UnitSystemDifferentialTests")]
    public UnitSystem(string name, uint code, string guid, Unit[] basis, Unit[] defaults)
    {
        m_name = name;
        m_ro_number = 0;
        m_code = code;
        m_guid = guid;
        Basis = basis;
        Defaults = defaults;
        m_basis_orig = basis;
        m_defaults_orig = defaults;
    }

    [MirrorVerified("UnitSystemDifferentialTests")]
    public string Name => m_name;

    [MirrorVerified("UnitSystemDifferentialTests")]
    public uint Code => m_code;

    [MirrorVerified("UnitSystemDifferentialTests")]
    public string Guid => m_guid;

    [MirrorVerified("UnitSystemDifferentialTests")]
    public int Rank => m_rank;

    /// <summary>The base dimensions in coordinate order.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Type[] Dimensions => m_dims;

    /// <summary>
    /// Setting a basis of fewer than eight units pads it to eight with an ampere in the last slot. The dimension
    /// names of all basis units, sorted, become the coordinate axes, and the basis-change matrices are rebuilt; a
    /// unit with an unknown dimension, or a singular basis, throws <see cref="ArgumentException"/> "not a basis"
    /// (after the fields have already changed).
    /// </summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Unit[] Basis
    {
        get => m_base;
        set
        {
            if (value.Length < 8)
            {
                var padded = new Unit[8];
                Array.Copy(value, padded, value.Length);
                padded[7] = new Unit("ampere", "A", "Current", 1.0, "V{Current      60000}");
                value = padded;
            }
            m_rank = value.Length;
            m_base = value;
            m_dims = new Type[m_rank];
            m_ltog = new double[m_rank, m_rank];
            int found = 0;
            try
            {
                foreach (var unit in value)
                {
                    m_unit_syms[unit.Symbol] = true;
                    foreach (var power in unit.Dimensions.Constants)
                        if (Array.IndexOf(m_dims, power.Base) < 0) m_dims[found++] = power.Base;
                }
                Array.Sort(m_dims);
                for (int k = 0; k != m_rank; k++)
                {
                    int[] column = GetCoordinates(m_base[k].Dimensions);
                    for (int l = 0; l != m_rank; l++) m_ltog[l, k] = column[l];
                }
                m_gtol = Inverse(m_ltog);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("not a basis", nameof(value), e);
            }
        }
    }

    /// <summary>The default units, grouped by dimension direction; at most two per direction fit.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Unit[] Defaults
    {
        get
        {
            Unit[] array = new Unit[m_defs.Count * 2];
            int count = 0;
            foreach (IList value in m_defs.Values)
            {
                foreach (Default item in value)
                {
                    array[count] = item.Unit;
                    count++;
                }
            }
            Unit[] result = new Unit[count];
            for (int i = 0; i != count; i++)
            {
                result[i] = array[i];
            }
            return result;
        }
        set
        {
            m_defs = new Hashtable(value.Length);
            foreach (var unit in value)
            {
                m_unit_syms[unit.Symbol] = true;
                var d = new Default(GetCoordinates(unit.Dimensions), unit);
                if (m_defs[d] is not IList group) m_defs[d] = group = new ArrayList(1);
                group.Add(d);
            }
            foreach (ArrayList group in m_defs.Values) group.Sort();
        }
    }

    [MirrorVerified("UnitSystemDifferentialTests")]
    public int RO_number
    {
        get => m_ro_number;
        set => m_ro_number = value;
    }

    [MirrorVerified("UnitSystemDifferentialTests")]
    public override string ToString() => m_name;

    /// <summary>Restores the basis and defaults the system was built with, forgetting other unit symbols; returns
    /// this system.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public UnitSystem Reset()
    {
        m_unit_syms = new Hashtable();
        Basis = m_basis_orig;
        Defaults = m_defaults_orig;
        return this;
    }

    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units Simplify(string signature) => Simplify(GetDimensions(signature));

    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units Simplify(Type.Product product) => Simplify(GetCoordinates(product));

    /// <summary>
    /// A single default unit raised to an integer power when one has the same direction and a scalar dividing the
    /// dimension's; of several, the smallest positive power, or for negative ones the one nearest zero. Otherwise,
    /// or when the chosen unit's symbol is <c>NOSIMPLIFY</c>, <see cref="Normalize(int[])"/>.
    /// </summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units Simplify(int[] global)
    {
        Default target = new Default(global);
        if (m_defs[target] is IList list)
        {
            Units units = null;
            int bestPower = 0;
            Default bestDefault = null;
            foreach (Default item in list)
            {
                if (item.Scalar != 0 && target.Scalar % item.Scalar == 0)
                {
                    int power = target.Scalar / item.Scalar;
                    if (units == null)
                    {
                        bestDefault = item;
                        bestPower = power;
                        units = new Units(item.Unit, power);
                    }
                    else if ((bestPower < 0 && power > bestPower) || (power > 0 && power < bestPower))
                    {
                        bestDefault = item;
                        bestPower = power;
                        units = new Units(item.Unit, power);
                    }
                }
            }
            if (units != null && bestDefault.Unit.Symbol != "NOSIMPLIFY")
            {
                return units;
            }
        }
        return Normalize(global);
    }

    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units Normalize(string signature) => Normalize(GetDimensions(signature));

    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units Normalize(Type.Product product) => Normalize(GetCoordinates(product));

    /// <summary>The dimension as powers of the basis units.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units Normalize(int[] global) => GetUnits(ChangeBasis(global));

    /// <summary>The summed exponents of the product's named dimensions; variables are ignored.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public int[] GetCoordinates(Type.Product product)
    {
        var coordinates = new int[m_rank];
        foreach (var power in product.Constants) coordinates[GetDimensionIndex(power.Base)] += power.Exponent;
        return coordinates;
    }

    /// <summary>The nonzero basis-unit exponents, each converted to a <see cref="Rational"/>.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Units GetUnits(double[] local)
    {
        var powers = new Units.Power[Count(local)];
        for (int i = 0, n = 0; n != powers.Length; i++)
        {
            if (local[i] == 0.0) continue;
            powers[n].Base = m_base[i];
            powers[n].Exponent = local[i];
            n++;
        }
        return new Units(powers);
    }

    /// <summary>A monomorphic product of the nonzero coordinates, in coordinate order.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public Type.Product GetDimensions(int[] global)
    {
        var powers = new Type.Product.Power[Count(global)];
        for (int i = 0, n = 0; n != powers.Length; i++)
        {
            if (global[i] == 0) continue;
            powers[n].Base = m_dims[i];
            powers[n].Exponent = global[i];
            n++;
        }
        return new Type.Product(powers, 0);
    }

    /// <summary>Dimension coordinates to basis-unit exponents.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public double[] ChangeBasis(int[] global)
    {
        var local = new double[m_rank];
        for (int i = 0; i != m_rank; i++)
            for (int j = 0; j != m_rank; j++)
                local[i] += m_gtol[i, j] * global[j];
        return local;
    }

    /// <summary>Basis-unit exponents to dimension coordinates, truncating each product term to int.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public int[] ChangeBasis(double[] local)
    {
        var global = new int[m_rank];
        for (int i = 0; i != m_rank; i++)
            for (int j = 0; j != m_rank; j++)
                global[i] += (int)(m_ltog[i, j] * local[j]);
        return global;
    }

    /// <summary>Whether a basis or default unit (now or since the last reset) has this symbol.</summary>
    [MirrorVerified("UnitSystemDifferentialTests")]
    public bool IsUnitSymbol(string sym) => m_unit_syms.ContainsKey(sym);

    [MirrorTested("Mcd.UnitSystem")]
    private int GetDimensionIndex(Type dimension)
    {
        int i = Array.BinarySearch(m_dims, dimension);
        if (i >= 0) return i;
        throw new ArgumentException("bad dimension", dimension.ToString());
    }

    [MirrorTested("Mcd.UnitSystem")]
    private static Type.Product GetDimensions(string signature) => Type.Parse(signature).GetDimensionTypeProduct();

    /// <summary>
    /// Gauss–Jordan inversion by columns. A pivot within 1e-12 of zero first borrows the first later column whose
    /// entry in the pivot row isn't small; a pivot still small throws "singular matrix".
    /// </summary>
    [MirrorTested("Mcd.UnitSystem")]
    private static double[,] Inverse(double[,] matrix)
    {
        int n = matrix.GetUpperBound(0) + 1;
        var inverse = new double[n, n];
        matrix = (double[,])matrix.Clone();
        for (int i = 0; i != n; i++) inverse[i, i] = 1.0;
        for (int j = 0; j != n; j++)
        {
            if (IsSmall(matrix[j, j]))
            {
                for (int k = j + 1; k != n; k++)
                {
                    if (IsSmall(matrix[j, k])) continue;
                    for (int r = 0; r != n; r++)
                    {
                        matrix[r, j] += matrix[r, k];
                        inverse[r, j] += inverse[r, k];
                    }
                    break;
                }
            }
            double pivot = matrix[j, j];
            if (IsSmall(pivot)) throw new ArgumentException("singular matrix", nameof(matrix));
            for (int r = 0; r != n; r++)
            {
                matrix[r, j] /= pivot;
                inverse[r, j] /= pivot;
            }
            for (int c = 0; c != n; c++)
            {
                if (c == j) continue;
                double factor = matrix[j, c];
                if (factor == 0.0) continue;
                for (int r = 0; r != n; r++)
                {
                    matrix[r, c] -= factor * matrix[r, j];
                    inverse[r, c] -= factor * inverse[r, j];
                }
            }
        }
        return inverse;
    }

    [MirrorTested("Mcd.UnitSystem")]
    private static bool IsSmall(double d) => -1E-12 < d && d < 1E-12;

    [MirrorTested("Mcd.UnitSystem")]
    private static int Count(int[] array)
    {
        int n = 0;
        foreach (int x in array) if (x != 0) n++;
        return n;
    }

    [MirrorTested("Mcd.UnitSystem")]
    private static int Count(double[] array)
    {
        int n = 0;
        foreach (double x in array) if (x != 0.0) n++;
        return n;
    }

    /// <summary>
    /// A dimension split into a direction and a scalar: the gcd of the coordinates, negative when the first nonzero
    /// one is, so the direction's first nonzero coordinate is positive. <see cref="Order"/> is the gcd's magnitude.
    /// Equality and hashing look at the direction only.
    /// </summary>
    private partial class Default : IComparable
    {
        [MirrorTested("Mcd.UnitSystem")]
        public readonly int[] Vector;
        [MirrorTested("Mcd.UnitSystem")]
        public readonly int Scalar;
        [MirrorTested("Mcd.UnitSystem")]
        public readonly int Order;
        [MirrorTested("Mcd.UnitSystem")]
        public readonly Unit Unit;

        [MirrorTested("Mcd.UnitSystem")]
        public Default(int[] global, Unit unit) : this(global) { Unit = unit; }

        [MirrorTested("Mcd.UnitSystem")]
        public Default(int[] global)
        {
            Vector = (int[])global.Clone();
            Scalar = 0;
            int first = 0;
            int[] vector = Vector;
            foreach (int value in vector)
            {
                if (value != 0)
                {
                    if (first == 0)
                    {
                        Scalar = Math.Abs(first = value);
                    }
                    else
                    {
                        Scalar = Rational.GCD(Scalar, value);
                    }
                }
            }
            Order = Scalar;
            if (first < 0)
            {
                Scalar = -Scalar;
            }
            else if (Scalar == 0 || Scalar == 1)
            {
                return;
            }
            for (int j = 0; j != Vector.Length; j++)
            {
                Vector[j] /= Scalar;
            }
        }

        [MirrorTested("Mcd.UnitSystem")]
        public override int GetHashCode()
        {
            int h = 0;
            foreach (int x in Vector) h = ((h << 4) | (h >> 28)) ^ (x % 16);
            return h;
        }

        [MirrorTested("Mcd.UnitSystem")]
        public override bool Equals(object o)
        {
            var other = (Default)o;
            for (int i = 0; i != Vector.Length; i++)
                if (Vector[i] != other.Vector[i]) return false;
            return true;
        }

        /// <summary>Descending <see cref="Order"/>.</summary>
        [MirrorTested("Mcd.UnitSystem")]
        public int CompareTo(object o) => ((Default)o).Order.CompareTo(Order);
    }
}
