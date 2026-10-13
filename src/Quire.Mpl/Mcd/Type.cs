#nullable disable
using System.Globalization;
using System.Text;

namespace Quire.Mcd;

/// <summary>
/// A dimension type. Unit signatures are products of named base dimensions with integer exponents, written
/// <c>length*mass/time^2</c>; <c>1</c> is dimensionless.
/// </summary>
public abstract partial class Type
{
    protected Type() { }

    public abstract string Name { get; }

    public override string ToString() => Name;

    /// <summary>Parses a unit signature into a <see cref="Product"/>.</summary>
    public static Type Parse(string signature)
    {
        var powers = new Dictionary<string, int>(StringComparer.Ordinal);
        int pos = 0, sign = 1;
        string s = signature.Replace(" ", "");
        if (s is "" or "1") return new Product([], 0);
        if (s.StartsWith("1/", StringComparison.Ordinal)) { pos = 2; sign = -1; }
        while (pos < s.Length)
        {
            int start = pos;
            while (pos < s.Length && (char.IsLetter(s[pos]) || s[pos] == '_')) pos++;
            if (pos == start) throw new FormatException($"Bad dimension signature '{signature}' at {start}");
            string name = s[start..pos];
            int exponent = 1;
            if (pos < s.Length && s[pos] == '^')
            {
                int e = ++pos;
                if (pos < s.Length && s[pos] == '-') pos++;
                while (pos < s.Length && char.IsDigit(s[pos])) pos++;
                exponent = int.Parse(s[e..pos], CultureInfo.InvariantCulture);
            }
            powers[name] = powers.GetValueOrDefault(name) + sign * exponent;
            if (pos == s.Length) break;
            sign = s[pos] switch
            {
                '*' => 1,
                '/' => -1,
                _ => throw new FormatException($"Bad dimension signature '{signature}' at {pos}"),
            };
            pos++;
        }
        var list = powers.Where(p => p.Value != 0).OrderBy(p => p.Key, StringComparer.Ordinal)
            .Select(p => new Product.Power { Base = new Constant(p.Key), Exponent = p.Value }).ToArray();
        return new Product(list, 0);
    }

    public abstract partial class Named : Type, IComparable
    {
        public string m_nam;

        protected Named() { m_nam = ""; }

        public override string Name => m_nam;

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(m_nam);

        public override bool Equals(object o) => o is Named n && n.GetType() == GetType() && n.m_nam == m_nam;

        public virtual int CompareTo(object o) => string.CompareOrdinal(m_nam, ((Named)o).m_nam);
    }

    /// <summary>A named base dimension such as <c>length</c>.</summary>
    public partial class Constant : Named
    {
        public Constant(string n) { m_nam = n ?? ""; }
    }

    public partial class Product : Type
    {
        public Power[] Powers;
        private int m_var;
        private int m_con;

        /// <summary><paramref name="v"/> is the number of leading powers whose bases are type variables.</summary>
        public Product(Power[] p, int v)
        {
            Powers = p ?? [];
            m_var = v;
            m_con = Powers.Length - v;
        }

        public bool IsDimensionless => Powers.Length == 0;

        public override string Name
        {
            get
            {
                if (Powers.Length == 0) return "1";
                var sb = new StringBuilder();
                foreach (var p in Powers)
                {
                    if (sb.Length > 0) sb.Append(p.Exponent < 0 ? '/' : '*');
                    else if (p.Exponent < 0) sb.Append("1/");
                    sb.Append(p.Base.Name);
                    int e = Math.Abs(p.Exponent);
                    if (e != 1) sb.Append('^').Append(e.ToString(CultureInfo.InvariantCulture));
                }
                return sb.ToString();
            }
        }

        public partial struct Power
        {
            public Type Base;
            public int Exponent;
        }
    }
}
