#nullable disable
using System.Globalization;
using System.Text;
using Quire.Mcd;

namespace Quire.Mpl.Run;

/// <summary>A value carrying a small integer tag that tells its subclasses apart.</summary>
public partial class Tag : DimVal
{
    public int m_tag;

    public Tag(int tag) { m_tag = tag; }
}

/// <summary>
/// An S-expression. Lists are <see cref="SCons"/> cells ending in <see cref="Nul"/>; atoms appear wrapped in a
/// <see cref="Tok"/>, which also carries the source handle used to place errors. The calculation engine receives
/// every region as such an expression ("Rl"), for example <c>(define x (op_mult 5 m))</c>.
/// </summary>
public abstract partial class Sexpr : Tag
{
    public const int NulTag = 0;
    public const int ConsTag = 1;
    public const int TokTag = 2;

    public Sexpr(int tag) : base(tag) { }

    /// <summary>The source handle of a token; 0 when there is none.</summary>
    public virtual uint handle => 0;

    /// <summary>The symbol of a symbol token, otherwise null.</summary>
    public virtual Sym sym => null;

    /// <summary>The text of a string token, otherwise null.</summary>
    public virtual string str => null;

    /// <summary>The cons cell when this is a non-empty list, otherwise null.</summary>
    public virtual SCons scons => null;

    /// <summary>The rest of a list being printed, closing parenthesis included.</summary>
    public virtual string FormatTail() => " . " + ToString() + ")";
}

/// <summary>The empty list.</summary>
public partial class Nul : Sexpr
{
    public static Nul singleton = new();

    public Nul() : base(NulTag) { }

    public override string ToString() => "()";

    public override string FormatTail() => ")";
}

/// <summary>A list cell: <see cref="car"/> is the element and <see cref="cdr"/> the rest of the list.</summary>
public partial class SCons : Sexpr
{
    public Sexpr Field1;
    public Sexpr Field2;

    public SCons(Sexpr first, Sexpr rest) : base(ConsTag)
    {
        Field1 = first;
        Field2 = rest ?? Nul.singleton;
    }

    public override SCons scons => this;

    public Sexpr car => Field1;
    public Sexpr cdr => Field2;
    public Sexpr head => Field1;
    public SCons tail => Field2 as SCons;

    public Sexpr first => Field1;
    public Sexpr second => tail?.Field1;
    public Sexpr third => tail?.tail?.Field1;
    public Sexpr fourth => tail?.tail?.tail?.Field1;
    public Sexpr fifth => tail?.tail?.tail?.tail?.Field1;

    /// <summary>The number of elements in the list this cell starts.</summary>
    public uint Length
    {
        get
        {
            uint n = 0;
            for (Sexpr e = this; e is SCons c; e = c.Field2) n++;
            return n;
        }
    }

    public static SCons list1(Sexpr x1) => new(x1, Nul.singleton);
    public static SCons list2(Sexpr x1, Sexpr x2) => new(x1, list1(x2));
    public static SCons list3(Sexpr x1, Sexpr x2, Sexpr x3) => new(x1, list2(x2, x3));
    public static SCons list4(Sexpr x1, Sexpr x2, Sexpr x3, Sexpr x4) => new(x1, list3(x2, x3, x4));

    public override string FormatTail() => " " + Field1 + Field2.FormatTail();

    public override string ToString() => "(" + Field1 + Field2.FormatTail();
}

/// <summary>An atom in an S-expression, with the source handle it came from.</summary>
public partial class Tok : Sexpr
{
    public Atom Field1;
    public uint Field2;

    public Tok(Atom a, uint tag) : base(TokTag)
    {
        Field1 = a;
        Field2 = tag;
    }

    public Tok(Atom a, int tag) : this(a, (uint)tag) { }

    public Atom atom => Field1;

    public override uint handle => Field2;

    public override Sym sym => Field1 as Sym;

    public override string str => (Field1 as str_constant)?.Field1;

    public override string ToString() => Field1?.ToString() ?? "";
}

/// <summary>An indivisible S-expression value: a symbol or a constant.</summary>
public abstract partial class Atom : Tag
{
    public const int SymTag = 0;
    public const int IntTag = 1;
    public const int RealTag = 2;
    public const int StrTag = 4;

    public Atom(int tag) : base(tag) { }

    public virtual double RealCon => throw new InvalidCastException($"{this} is not a real constant");

    public virtual int IntCon => throw new InvalidCastException($"{this} is not an integer constant");

    public virtual string StrCon => throw new InvalidCastException($"{this} is not a string constant");

    public Sym sym => this as Sym;
}

/// <summary>A symbol, optionally qualified by a module name.</summary>
public partial class Sym : Atom
{
    public string Field1;
    public string Field2;

    public Sym(object nm) : this(nm as string ?? nm?.ToString() ?? "", null) { }

    public Sym(string nm, string q) : base(SymTag)
    {
        Field1 = nm ?? "";
        Field2 = q;
    }

    public string name => Field1;

    public string qualifier => Field2;

    public string long_name => Field2 == null ? Field1 : Field2 + "." + Field1;

    /// <summary>A token for the unqualified symbol <paramref name="nm"/>.</summary>
    public static Tok MakeSym(string nm) => new(new Sym(nm, null), 0);

    /// <summary>The name, between bars when it would otherwise read as something else.</summary>
    public override string ToString()
    {
        string nm = long_name;
        bool quote = nm.Length == 0 || nm.Any(c => char.IsWhiteSpace(c) || c is '(' or ')' or '"' or '|' or '\\')
            || double.TryParse(nm, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        return quote ? "|" + nm.Replace("\\", "\\\\").Replace("|", "\\|") + "|" : nm;
    }
}

public partial class int_constant : Atom
{
    public int Field1;

    public int_constant(int num) : base(IntTag) { Field1 = num; }

    public override int IntCon => Field1;

    public override string ToString() => Field1.ToString(CultureInfo.InvariantCulture);
}

public partial class real_constant : Atom
{
    public double Field1;

    public real_constant(double v) : base(RealTag) { Field1 = v; }

    public override double RealCon => Field1;

    public override string ToString() => Field1.ToString("R", CultureInfo.InvariantCulture);
}

public partial class str_constant : Atom
{
    public string Field1;

    public str_constant(string v) : base(StrTag) { Field1 = v ?? ""; }

    public override string StrCon => Field1;

    public override string ToString()
    {
        var sb = new StringBuilder("\"");
        foreach (char c in Field1)
        {
            if (c is '\\' or '"') sb.Append('\\');
            sb.Append(c);
        }
        return sb.Append('"').ToString();
    }
}
