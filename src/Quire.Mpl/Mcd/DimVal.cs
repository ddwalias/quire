#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;

namespace Quire.Mcd;

/// <summary>
/// Base of every Mpl value. <see cref="m_dim"/> is the value's dimension (null when polymorphic). The base answers
/// every capability query negatively and every conversion with an <see cref="AppException"/> carrying Mathcad's
/// message key, so subclasses override only what they support. Behaviour follows mplrun 12.0.1.0
/// (docs/mirror/evidence/Mcd.DimVal.md).
/// </summary>
[Serializable]
public abstract partial class DimVal
{
    [MirrorVerified("DimValDifferentialTests")]
    public Dim m_dim;

    [MirrorVerified("DimValDifferentialTests")]
    protected DimVal() { }

    [MirrorVerified("DimValDifferentialTests")]
    public bool IsPolymorphic => m_dim == null;

    /// <summary>One of the physical dimension, or <see cref="NumVal.mcNaN"/> when there is none or it is mixed.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual NumVal PhysicalUnit
    {
        get
        {
            var d = PhysicalDimension;
            return d == null || d == Dim.mixed_matrix_dim ? NumVal.mcNaN : new NumVal(1.0, 0.0, d);
        }
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual Dim PhysicalDimension => null;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual Dim UniformDimension => m_dim;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual string IsString => null;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual int IsReal => 0;

    /// <summary>Whether <see cref="m_dim"/> (not the physical dimension) is unitless, polymorphic or zero.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual bool IsDimless => Dim.Id_b(m_dim);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual bool IsDimlessReal => IsDimless && IsReal != 0;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual MatVal IsMatrix => null;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual Function IsFunction => null;

    /// <summary>How many values a range holds; 1 for anything else.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual int Rsize => 1;

    [MirrorVerified("DimValDifferentialTests")]
    public DimVal Rfinal => range_get(Rsize - 1);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual int Isize => 1;

    /// <summary>Sets the dimension of a value in place; anything else passes through.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public static object dyn(object o, Dim d)
    {
        DimVal dimVal = o as DimVal;
        if (dimVal == null)
        {
            return o;
        }
        dimVal.m_dim = d;
        return dimVal;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public DimVal dv(Dim d)
    {
        m_dim = d;
        return this;
    }

    /// <summary><see cref="nearly_equal"/> when both are values, else <see cref="object.Equals(object)"/> on
    /// <paramref name="x1"/>.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public static bool NearlyEqual(object x1, object x2, double fuzz)
    {
        DimVal dimVal = x1 as DimVal;
        DimVal dimVal2 = x2 as DimVal;
        if (dimVal == null || dimVal2 == null)
        {
            return x1.Equals(x2);
        }
        return dimVal.nearly_equal(dimVal2, fuzz);
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual bool nearly_equal(DimVal obj, double fuzz) => Equals(obj);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual string ToString(IFormatProvider culture) => ToString();

    /// <summary>The units of <paramref name="d"/> (unitless when null) in the thread's unit system: simplified to
    /// defaults, or plain basis units.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public static Units GetUnits(Dim d, bool simplify)
    {
        int[] global = (d == null) ? Dim.identity.Coordinates : d.Coordinates;
        if (!simplify)
        {
            return RuntimeOptions.the_unit_system.Normalize(global);
        }
        return RuntimeOptions.the_unit_system.Simplify(global);
    }

    [MirrorVerified("DimValDifferentialTests")]
    public Units GetUnits(bool simplify) => GetUnits(m_dim, simplify);

    /// <summary>The uniform dimension of a value; anything that isn't one throws <see cref="NullReferenceException"/>.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public static Dim DimOf(object o) => (o as DimVal).UniformDimension;

    /// <summary>True for anything that isn't a value.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public static bool IsDimOf(object o, Dim d) => o is not DimVal v || v.IsUniformDimOf(d);

    [MirrorVerified("DimValDifferentialTests")]
    public static object DimCast(object o, Dim d) => o is DimVal v ? v.SetDim(d) : o;

    /// <summary>Sets the dimension in place (numbers return a copy instead).</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal SetDim(Dim d)
    {
        m_dim = d;
        return this;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual bool IsUniformDimOf(Dim d) => Dim.Eq_b(m_dim, d);

    /// <summary>Uses the key "must be real", as the original does.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public string asString() => asString("must be real", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public string asString(uint a) => asString("must be string", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual string asString(string m, uint a)
    {
        AppException.Raise(m, a);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public double asReal() => asReal("must be real", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public double asReal(uint a) => asReal("must be real", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual double asReal(string m, uint a)
    {
        AppException.Raise(m, a);
        return double.NaN;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual unsafe bool isInt(int* ret) => false;

    [MirrorVerified("DimValDifferentialTests")]
    public int asInt() => asInt("must be integer", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public int asInt(uint a) => asInt("must be integer", a);

    /// <summary>The integer from <see cref="isInt"/>; otherwise raises "must be integer" whatever
    /// <paramref name="m"/> says.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public unsafe int asInt(string m, uint a)
    {
        int value;
        if (isInt(&value)) return value;
        AppException.Raise("must be integer", a);
        return 0;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public int asDimlessInt() => asDimlessInt("must be integer", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public int asDimlessInt(uint a) => asDimlessInt("must be integer", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual int asDimlessInt(string m, uint a) => asDimless(a).asInt(m, a);

    /// <summary>This value, after checking that <see cref="m_dim"/> is unitless (argument 1 in the error).</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal asDimless()
    {
        Dim.Id(m_dim);
        return this;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal asDimless(uint a)
    {
        Dim.Id(m_dim, a);
        return this;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual double asDimlessReal(uint a)
    {
        Dim.Id(m_dim, a);
        return asReal(a);
    }

    /// <summary>Raises <paramref name="m"/> for a value with units, before the real check.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public double asDimlessReal(string m, uint a)
    {
        if (!IsDimless) AppException.Raise(m, a);
        return asReal(m, a);
    }

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asVector() => asVector("must be vector", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asVector(uint a) => asVector("must be vector", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual MatVal asVector(string m, uint a)
    {
        AppException.Raise(m, a);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asRealVector() => asRealVector("must be real vector", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asRealVector(uint a) => asRealVector("must be real vector", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual MatVal asRealVector(string m, uint a)
    {
        AppException.Raise(m, a);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asRealMatrix() => asRealMatrix("must be real matrix", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asRealMatrix(uint a) => asRealMatrix("must be real matrix", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual MatVal asRealMatrix(string m, uint a)
    {
        AppException.Raise(m, a);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asMatrix() => asMatrix("must be matrix", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public MatVal asMatrix(uint a) => asMatrix("must be matrix", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual MatVal asMatrix(string m, uint a)
    {
        AppException.Raise(m, a);
        return null;
    }

    /// <summary>Raises "incompatible shape" with <paramref name="handle"/> as the argument.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual object[] asTuple(uint r, uint c, uint handle)
    {
        AppException.Raise("incompatible shape", handle);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public RngVal asRange() => asRange("must be range", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public RngVal asRange(uint a) => asRange("must be range", a);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual RngVal asRange(string m, uint a)
    {
        AppException.Raise(m, a);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public Function asFunction() => asFunction("bad_function", 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public Function asFunction(uint a) => asFunction("bad_function", a);

    /// <summary>Raises "<paramref name="m"/>\t<c>this</c>".</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual Function asFunction(string m, uint a)
    {
        OnError(a, m, this);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual Function IsFunctionN(int argc) => null;

    [MirrorVerified("DimValDifferentialTests")]
    public Function asFunctionN(int argc) => asFunctionN(argc, 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual Function asFunctionN(int argc, uint errpos)
    {
        BadCall(-1, argc, errpos);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal range_get(int n) => this;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal iterator_get(int n) => this;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal asIterator() => this;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object matrix_get_1(int i)
    {
        AppException.Raise("must be vector", 1u);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object sym_matrix_get_1(int i)
    {
        AppException.Raise("must be vector", 1u);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object matrix_get_2(int i, int j)
    {
        AppException.Raise("must be matrix", 1u);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object sym_matrix_get_2(int i, int j)
    {
        AppException.Raise("must be matrix", 1u);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal asImmutable() => this;

    [MirrorVerified("DimValDifferentialTests")]
    public virtual DimVal asResult() => this;

    [MirrorVerified("DimValDifferentialTests")]
    public static object as_result(object o)
    {
        DimVal dimVal = o as DimVal;
        if (dimVal != null)
        {
            return dimVal.asResult();
        }
        return o;
    }

    /// <summary>Raises "bad_function\t<c>this</c>" (argument 0).</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public virtual object Apply(object[] args)
    {
        OnError(0u, "bad_function", this);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public static void OnError(uint a, string m, int i, int j) =>
        AppException.Raise(string.Format("{0}\t{1}\t{2}", m, i, j), a);

    [MirrorVerified("DimValDifferentialTests")]
    public static void OnError(uint a, string m, int i) =>
        AppException.Raise(string.Format("{0}\t{1}", m, i), a);

    [MirrorVerified("DimValDifferentialTests")]
    public static void OnError(uint a, string m, object thing) =>
        AppException.Raise(string.Format("{0}\t{1}", m, thing), a);

    /// <summary>
    /// Raises "too_many_arguments1\tactual" when one argument was required and more were given,
    /// "too_many_arguments\trequired\tactual" for other surpluses, and "too_few_arguments\trequired\tactual"
    /// otherwise (equal counts included).
    /// </summary>
    [MirrorVerified("DimValDifferentialTests")]
    public static object WrongNumberOfArguments(int required, int actual, uint errpos)
    {
        if (required < actual)
        {
            if (required == 1)
            {
                OnError(errpos, "too_many_arguments1", actual);
            }
            else
            {
                OnError(errpos, "too_many_arguments", required, actual);
            }
        }
        else
        {
            OnError(errpos, "too_few_arguments", required, actual);
        }
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public static object WrongNumberOfArguments(int required, int actual) => WrongNumberOfArguments(required, actual, 0u);

    /// <summary>A required count of -1 means this isn't a function at all ("bad_function"); otherwise the
    /// argument-count error.</summary>
    [MirrorVerified("DimValDifferentialTests")]
    public object BadCall(int required, int actual, uint errpos)
    {
        if (required == -1) OnError(errpos, "bad_function", this);
        WrongNumberOfArguments(required, actual, errpos);
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public object BadCall(int required, int actual) => BadCall(required, actual, 0u);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall()
    {
        AppException.Raise("Not Implemented");
        return null;
    }

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b) => BadCall(-1, 1);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c) => BadCall(-1, 2);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d) => BadCall(-1, 3);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e) => BadCall(-1, 4);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f) => BadCall(-1, 5);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g) => BadCall(-1, 6);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h) => BadCall(-1, 7);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i) => BadCall(-1, 8);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j) => BadCall(-1, 9);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j, object k) =>
        BadCall(-1, 10);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j, object k,
        object l) => BadCall(-1, 11);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j, object k,
        object l, object m) => BadCall(-1, 12);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j, object k,
        object l, object m, object n) => BadCall(-1, 13);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j, object k,
        object l, object m, object n, object o) => BadCall(-1, 14);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j, object k,
        object l, object m, object n, object o, object p) => BadCall(-1, 15);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynCall(object b, object c, object d, object e, object f, object g, object h, object i, object j, object k,
        object l, object m, object n, object o, object p, object q) => BadCall(-1, 16);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object DynApply(object[] args) => BadCall(-1, args.Length);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object VecCall(object b) => BadCall(-1, 1);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object VecCall(object b, object c) => BadCall(-1, 2);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object VecCall(object b, object c, object d) => BadCall(-1, 3);

    [MirrorVerified("DimValDifferentialTests")]
    public virtual object VecApply(object[] args) => BadCall(-1, args.Length);
}
