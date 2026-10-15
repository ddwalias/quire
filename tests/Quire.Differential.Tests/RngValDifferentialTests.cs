using System.Reflection;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.RngVal</c>, <c>Mcd.RngValInterpolate</c>, <c>Mcd.Iterator</c> (with its scalar and
/// sequence iterators) and <c>Mcd.RangeIterator</c> with Quire's: range construction and its errors, element access
/// past either end, equality, copies, and iteration. Cases are listed in docs/mirror/evidence/Mcd.RngVal.md.
/// </summary>
public class RngValDifferentialTests
{
    private const string Contract = "Mcd.RngVal";
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstanceNonPublic = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static System.Type MRngVal => Mathcad.Type("mplrun", "Mcd.RngVal");
    private static System.Type QRngVal => typeof(global::Quire.Mcd.RngVal);
    private static System.Type MRngValInterpolate => Mathcad.Type("mplrun", "Mcd.RngValInterpolate");
    private static System.Type QRngValInterpolate => typeof(global::Quire.Mcd.RngValInterpolate);
    private static System.Type MIterator => Mathcad.Type("mplrun", "Mcd.Iterator");
    private static System.Type QIterator => typeof(global::Quire.Mcd.Iterator);
    private static System.Type MRangeIterator => Mathcad.Type("mplrun", "Mcd.RangeIterator");
    private static System.Type QRangeIterator => typeof(global::Quire.Mcd.RangeIterator);
    private static System.Type MSeqIterator => Mathcad.Type("mplrun", "Mcd.Iterator+SeqIterator");
    private static System.Type QSeqIterator => typeof(global::Quire.Mcd.Iterator.SeqIterator);
    private static System.Type MScalarIterator => Mathcad.Type("mplrun", "Mcd.Iterator+ScalarIterator");
    private static System.Type QScalarIterator => typeof(global::Quire.Mcd.Iterator.ScalarIterator);
    private static System.Type MDim => Mathcad.Type("mplrun", "Mcd.Dim");
    private static System.Type QDim => typeof(global::Quire.Mcd.Dim);
    private static System.Type MDimVal => Mathcad.Type("mplrun", "Mcd.DimVal");
    private static System.Type QDimVal => typeof(global::Quire.Mcd.DimVal);
    private static System.Type MMatVal => Mathcad.Type("mplrun", "Mcd.MatVal");
    private static System.Type QMatVal => typeof(global::Quire.Mcd.MatVal);

    private static System.Type MapToM(System.Type t)
    {
        if (t == QRngVal) return MRngVal;
        if (t == QRngValInterpolate) return MRngValInterpolate;
        if (t == QIterator) return MIterator;
        if (t == QRangeIterator) return MRangeIterator;
        if (t == QSeqIterator) return MSeqIterator;
        if (t == QScalarIterator) return MScalarIterator;
        if (t == QDim) return MDim;
        if (t == QDimVal) return MDimVal;
        if (t == QMatVal) return MMatVal;
        if (t == typeof(global::Quire.Mcd.Iterator[]))
            return Array.CreateInstance(MIterator, 0).GetType();
        return t;
    }

    private static (MethodInfo q, MethodInfo m) Method(System.Type qType, string name, BindingFlags flags, params System.Type[] qParamTypes)
    {
        var mType = MapToM(qType);
        var mParamTypes = qParamTypes.Select(MapToM).ToArray();
        var q = qType.GetMethod(name, flags, qParamTypes)
            ?? throw new HarnessException($"Quire method not found: {qType.Name}.{name}({string.Join(", ", qParamTypes.Select(t => t.Name))})");
        var m = mType.GetMethod(name, flags, mParamTypes)
            ?? throw new HarnessException($"Mathcad method not found: {mType.Name}.{name}({string.Join(", ", mParamTypes.Select(t => t.Name))})");
        return (q, m);
    }

    private static (ConstructorInfo q, ConstructorInfo m) Ctor(System.Type qType, BindingFlags flags, params System.Type[] qParamTypes)
    {
        var mType = MapToM(qType);
        var mParamTypes = qParamTypes.Select(MapToM).ToArray();
        var q = qType.GetConstructor(flags, null, qParamTypes, null)
            ?? throw new HarnessException($"Quire constructor not found: {qType.Name}({string.Join(", ", qParamTypes.Select(t => t.Name))})");
        var m = mType.GetConstructor(flags, null, mParamTypes, null)
            ?? throw new HarnessException($"Mathcad constructor not found: {mType.Name}({string.Join(", ", mParamTypes.Select(t => t.Name))})");
        return (q, m);
    }

    private static string ShowRngVal(Side side, object? v)
    {
        if (v == null) return "null";
        var init = Get(v, "m_init");
        var incr = Get(v, "m_incr");
        var n = Get(v, "m_n");
        var integral = Get(v, "m_integral");
        var dim = side.DimText(Get(v, "m_dim"));
        var tName = v.GetType().Name;
        if (tName == "RngValInterpolate")
        {
            var last = Get(v, "m_last");
            var nint = Get(v, "m_ninterval");
            return $"RngValInterpolate[init={init},incr={incr},n={n},int={integral},last={last},nint={nint},dim={dim}]";
        }
        return $"RngVal[init={init},incr={incr},n={n},int={integral},dim={dim}]";
    }

    private static string ShowVal(Side side, object? v)
    {
        if (v == null) return "null";
        if (v is string or int or uint or double or bool or long or byte or short) return Text(v);
        if (v is Array arr)
        {
            var items = new List<string>();
            foreach (var item in arr) items.Add(ShowVal(side, item));
            return "[" + string.Join(", ", items) + "]";
        }
        var tName = v.GetType().Name;
        if (tName is "RngVal" or "RngValInterpolate") return ShowRngVal(side, v);
        if (tName == "RangeIterator")
        {
            var it = Get(v, "m_it");
            var n = Get(v, "m_n");
            var rng = ShowRngVal(side, Get(v, "m_rng"));
            return $"RangeIterator[it={it},n={n},rng={rng}]";
        }
        if (tName == "ScalarIterator")
        {
            return $"ScalarIterator[init={ShowVal(side, Get(v, "m_init"))}]";
        }
        if (tName == "SeqIterator")
        {
            var curr = Get(v, "m_curr");
            var its = Get(v, "m_its") as Array;
            return $"SeqIterator[curr={curr},len={its?.Length ?? 0}]";
        }
        if (tName == "MatrixIterator")
        {
            return $"MatrixIterator[r={Get(v, "m_row")},c={Get(v, "m_col")}]";
        }
        if (tName == "NumVal")
        {
            var data = Get(v, "Data");
            return $"NumVal[rp={Get(data!, "rp")},ip={Get(data!, "ip")},dim={side.DimText(Get(v, "m_dim"))}]";
        }
        if (tName == "MatVal")
        {
            var re = Get(v, "m_rarr") is double[] r ? $"[{string.Join(",", r.Select(Side.Bits))}]" : "null";
            return $"MatVal[{Get(v, "Rows")}x{Get(v, "Cols")},re={re},dim={side.DimText(Get(v, "m_dim"))}]";
        }
        if (tName == "Dim") return side.DimText(v);
        return Text(v);
    }

    private static Func<object?, string> Show(Side side) => v => ShowVal(side, v);

    private static Func<object, string> StateRngVal(Side side) => v => ShowRngVal(side, v);
    private static Func<object, string> StateRangeIterator(Side side) => v => ShowVal(side, v);
    private static Func<object, string> StateScalarIterator(Side side) => v => ShowVal(side, v);
    private static Func<object, string> StateSeqIterator(Side side) => v => ShowVal(side, v);
    private static Func<object, string> StateIterator(Side side) => v => ShowVal(side, v);

    private static (object q, object m) MakeRange(double init, double incr, int n, string dim = "identity")
    {
        var qDim = Side.Quire.D(dim);
        var mDim = Side.Mathcad.D(dim);
        var qCtor = QRngVal.GetConstructor(DeclaredInstanceNonPublic, null, [typeof(double), typeof(double), typeof(int), QDim], null)!;
        var mCtor = MRngVal.GetConstructor(DeclaredInstanceNonPublic, null, [typeof(double), typeof(double), typeof(int), MDim], null)!;
        return (qCtor.Invoke([init, incr, n, qDim])!, mCtor.Invoke([init, incr, n, mDim])!);
    }

    private static (object q, object m) MakeInterp(double init, double last, int n, string dim = "identity")
    {
        var qDim = Side.Quire.D(dim);
        var mDim = Side.Mathcad.D(dim);
        var qCtor = QRngValInterpolate.GetConstructor(DeclaredInstancePublic, null, [typeof(double), typeof(double), typeof(int), QDim], null)!;
        var mCtor = MRngValInterpolate.GetConstructor(DeclaredInstancePublic, null, [typeof(double), typeof(double), typeof(int), MDim], null)!;
        return (qCtor.Invoke([init, last, n, qDim])!, mCtor.Invoke([init, last, n, mDim])!);
    }

    [SkippableFact]
    public void Constructors_and_properties()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qStateRng = StateRngVal(Side.Quire);
        var mStateRng = StateRngVal(Side.Mathcad);

        // 1. Protected RngVal constructors
        var (qRngCtor4, mRngCtor4) = Ctor(QRngVal, DeclaredInstanceNonPublic, typeof(double), typeof(double), typeof(int), QDim);
        var (qRngCtorCopy, mRngCtorCopy) = Ctor(QRngVal, DeclaredInstanceNonPublic, QRngVal);

        MethodComparison.Compare("RngVal(0, 1, 5, L)", Contract,
            qRngCtor4, null, [0.0, 1.0, 5, Side.Quire.D("L")],
            mRngCtor4, null, [0.0, 1.0, 5, Side.Mathcad.D("L")], qs, ms);
        MethodComparison.Compare("RngVal(0.5, 0.25, 0, identity)", Contract,
            qRngCtor4, null, [0.5, 0.25, 0, Side.Quire.D("identity")],
            mRngCtor4, null, [0.5, 0.25, 0, Side.Mathcad.D("identity")], qs, ms);

        var (qBase, mBase) = MakeRange(1.0, 0.5, 4, "T");
        MethodComparison.Compare("RngVal(copy)", Contract,
            qRngCtorCopy, null, [qBase],
            mRngCtorCopy, null, [mBase], qs, ms);

        // 2. RngValInterpolate constructor
        var (qInterpCtor, mInterpCtor) = Ctor(QRngValInterpolate, DeclaredInstancePublic, typeof(double), typeof(double), typeof(int), QDim);
        MethodComparison.Compare("RngValInterpolate(0, 1, 10, L)", Contract,
            qInterpCtor, null, [0.0, 1.0, 10, Side.Quire.D("L")],
            mInterpCtor, null, [0.0, 1.0, 10, Side.Mathcad.D("L")], qs, ms);
        MethodComparison.Compare("RngValInterpolate(1, 3, 4, poly)", Contract,
            qInterpCtor, null, [1.0, 3.0, 4, Side.Quire.D("poly")],
            mInterpCtor, null, [1.0, 3.0, 4, Side.Mathcad.D("poly")], qs, ms);

        // 3. Properties and instance methods on RngVal
        var (qGetIntegral, mGetIntegral) = Method(QRngVal, "get_Integral", DeclaredInstancePublic);
        var (qGetPhysDim, mGetPhysDim) = Method(QRngVal, "get_PhysicalDimension", DeclaredInstancePublic);
        var (qGetRsize, mGetRsize) = Method(QRngVal, "get_Rsize", DeclaredInstancePublic);
        var (qGetIsize, mGetIsize) = Method(QRngVal, "get_Isize", DeclaredInstancePublic);
        var (qAsResult, mAsResult) = Method(QRngVal, "asResult", DeclaredInstancePublic);
        var (qAsRange, mAsRange) = Method(QRngVal, "asRange", DeclaredInstancePublic, typeof(string), typeof(uint));
        var (qGetHashCode, mGetHashCode) = Method(QRngVal, "GetHashCode", DeclaredInstancePublic);
        var (qGet, mGet) = Method(QRngVal, "get", DeclaredInstancePublic, typeof(int));
        var (qRangeGet, mRangeGet) = Method(QRngVal, "range_get", DeclaredInstancePublic, typeof(int));
        var (qIterGet, mIterGet) = Method(QRngVal, "iterator_get", DeclaredInstancePublic, typeof(int));

        var testRanges = new[]
        {
            ("integral_L", MakeRange(0.0, 1.0, 5, "L")),
            ("non_integral_identity", MakeRange(0.1, 0.25, 4, "identity")),
            ("poly", MakeRange(1.0, 2.0, 3, "poly")),
            ("empty", MakeRange(0.0, 1.0, 0, "identity"))
        };

        foreach (var (tag, (qTgt, mTgt)) in testRanges)
        {
            MethodComparison.Compare($"{tag}.Integral", Contract,
                qGetIntegral, qTgt, [], mGetIntegral, mTgt, [], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"{tag}.PhysicalDimension", Contract,
                qGetPhysDim, qTgt, [], mGetPhysDim, mTgt, [], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"{tag}.Rsize", Contract,
                qGetRsize, qTgt, [], mGetRsize, mTgt, [], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"{tag}.Isize", Contract,
                qGetIsize, qTgt, [], mGetIsize, mTgt, [], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"{tag}.asResult", Contract,
                qAsResult, qTgt, [], mAsResult, mTgt, [], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"{tag}.asRange('m', 1)", Contract,
                qAsRange, qTgt, ["m", 1u], mAsRange, mTgt, ["m", 1u], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"{tag}.GetHashCode", Contract,
                qGetHashCode, qTgt, [], mGetHashCode, mTgt, [], qs, ms, qStateRng, mStateRng);

            foreach (int idx in new[] { -1, 0, 1, 4, 10 })
            {
                MethodComparison.Compare($"{tag}.get({idx})", Contract,
                    qGet, qTgt, [idx], mGet, mTgt, [idx], qs, ms, qStateRng, mStateRng);
                MethodComparison.Compare($"{tag}.range_get({idx})", Contract,
                    qRangeGet, qTgt, [idx], mRangeGet, mTgt, [idx], qs, ms, qStateRng, mStateRng);
                MethodComparison.Compare($"{tag}.iterator_get({idx})", Contract,
                    qIterGet, qTgt, [idx], mIterGet, mTgt, [idx], qs, ms, qStateRng, mStateRng);
            }
        }

        var (qOdd, mOdd) = MakeRange(0.1, 0.25, 3, "identity");
        MethodComparison.Compare("RngVal(0.1, 0.25, 3).GetHashCode", Contract,
            qGetHashCode, qOdd, [], mGetHashCode, mOdd, [], qs, ms, qStateRng, mStateRng);

        // 4. Overridden methods on RngValInterpolate
        var (qInterpGet, mInterpGet) = Method(QRngValInterpolate, "get", DeclaredInstancePublic, typeof(int));
        var (qInterpRangeGet, mInterpRangeGet) = Method(QRngValInterpolate, "range_get", DeclaredInstancePublic, typeof(int));
        var (qInterpIterGet, mInterpIterGet) = Method(QRngValInterpolate, "iterator_get", DeclaredInstancePublic, typeof(int));

        var (qI, mI) = MakeInterp(0.0, 1.0, 5, "L");
        foreach (int idx in new[] { -1, 0, 2, 5, 6 })
        {
            MethodComparison.Compare($"RngValInterpolate.get({idx})", Contract,
                qInterpGet, qI, [idx], mInterpGet, mI, [idx], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"RngValInterpolate.range_get({idx})", Contract,
                qInterpRangeGet, qI, [idx], mInterpRangeGet, mI, [idx], qs, ms, qStateRng, mStateRng);
            MethodComparison.Compare($"RngValInterpolate.iterator_get({idx})", Contract,
                qInterpIterGet, qI, [idx], mInterpIterGet, mI, [idx], qs, ms, qStateRng, mStateRng);
        }

        var (qI2, mI2) = MakeInterp(1.0, 3.0, 4, "L");
        foreach (int idx in new[] { 1, 3 })
        {
            MethodComparison.Compare($"RngValInterpolate(1, 3, 4).get({idx})", Contract,
                qInterpGet, qI2, [idx], mInterpGet, mI2, [idx], qs, ms, qStateRng, mStateRng);
        }

        // 5. Matrix conversions on RngVal (Analyzed members)
        var (qAsVector, mAsVector) = Method(QRngVal, "asVector", DeclaredInstancePublic, typeof(string), typeof(uint));
        var (qAsMatrix, mAsMatrix) = Method(QRngVal, "asMatrix", DeclaredInstancePublic, typeof(string), typeof(uint));
        var (qAsRealMatrix, mAsRealMatrix) = Method(QRngVal, "asRealMatrix", DeclaredInstancePublic, typeof(string), typeof(uint));
        var (qAsRealVector, mAsRealVector) = Method(QRngVal, "asRealVector", DeclaredInstancePublic, typeof(string), typeof(uint));

        var (qMatRng, mMatRng) = MakeRange(0.0, 1.0, 3, "L");
        MethodComparison.Compare("asVector('m', 1)", Contract,
            qAsVector, qMatRng, ["m", 1u], mAsVector, mMatRng, ["m", 1u], qs, ms, qStateRng, mStateRng);
        MethodComparison.Compare("asMatrix('m', 2)", Contract,
            qAsMatrix, qMatRng, ["m", 2u], mAsMatrix, mMatRng, ["m", 2u], qs, ms, qStateRng, mStateRng);
        MethodComparison.Compare("asRealMatrix('m', 3)", Contract,
            qAsRealMatrix, qMatRng, ["m", 3u], mAsRealMatrix, mMatRng, ["m", 3u], qs, ms, qStateRng, mStateRng);
        MethodComparison.Compare("asRealVector('m', 4)", Contract,
            qAsRealVector, qMatRng, ["m", 4u], mAsRealVector, mMatRng, ["m", 4u], qs, ms, qStateRng, mStateRng);
        var (qFrom2, mFrom2) = MakeRange(2.0, 1.0, 3, "L");
        MethodComparison.Compare("RngVal(2, 1, 3).asRealVector('m', 4)", Contract,
            qAsRealVector, qFrom2, ["m", 4u], mAsRealVector, mFrom2, ["m", 4u], qs, ms, qStateRng, mStateRng);
    }

    [SkippableFact]
    public void Range_factories()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);

        var (qIncrRange5, mIncrRange5) = Method(QRngVal, "IncrRange", DeclaredStaticPublic, typeof(double), typeof(double), typeof(double), QDim, typeof(string));
        var (qIncrRange4, mIncrRange4) = Method(QRngVal, "IncrRange", DeclaredStaticPublic, typeof(double), typeof(double), typeof(double), QDim);
        var (qIncrRangeInt, mIncrRangeInt) = Method(QRngVal, "IncrRange", DeclaredStaticPublic, typeof(double), typeof(double), typeof(int), QDim);
        var (qRange2, mRange2) = Method(QRngVal, "Range", DeclaredStaticPublic, typeof(double), typeof(double));
        var (qRange4, mRange4) = Method(QRngVal, "Range", DeclaredStaticPublic, typeof(double), typeof(double), typeof(double), QDim);

        (double First, double Incr, double Last)[] cases =
        [
            (0, 1, 10), (0, 0.1, 1), (0, 0.25, 1), (1, 0.3, 2), (10, -1, 0), (1, -0.1, 0),
            (0, 2, 9), (0, 1, 0), (0, 1, -1), (0, -1, 1), (0, 0, 1),
            (double.NaN, 1, 2), (0, double.NaN, 2), (0, 1, double.NaN),
            (0, 1e-9, 10), (0, 1, double.PositiveInfinity), (0, 1, double.NegativeInfinity),
            (-5, 0.5, 5), (0.1, 0.1, 0.3), (1e10, 1, 1e10 + 5), (0, 1e-300, 1), (0, 3, 10)
        ];

        foreach (var (f, i, l) in cases)
        {
            string p = $"({f}, {i}, {l})";
            MethodComparison.Compare($"IncrRange{p}_custom", Contract,
                qIncrRange5, null, [f, i, l, Side.Quire.D("L"), "custom msg"],
                mIncrRange5, null, [f, i, l, Side.Mathcad.D("L"), "custom msg"], qs, ms);

            MethodComparison.Compare($"IncrRange{p}_default", Contract,
                qIncrRange4, null, [f, i, l, Side.Quire.D("identity")],
                mIncrRange4, null, [f, i, l, Side.Mathcad.D("identity")], qs, ms);

            MethodComparison.Compare($"Range({f}, {l})", Contract,
                qRange2, null, [f, l],
                mRange2, null, [f, l], qs, ms);

            MethodComparison.Compare($"Range{p}", Contract,
                qRange4, null, [f, f + i, l, Side.Quire.D("identity")],
                mRange4, null, [f, f + i, l, Side.Mathcad.D("identity")], qs, ms);
        }

        foreach (int n in new[] { 0, 1, 5, -3, int.MaxValue, int.MinValue })
        {
            MethodComparison.Compare($"IncrRange(2, 0.5, n={n})", Contract,
                qIncrRangeInt, null, [2.0, 0.5, n, Side.Quire.D("T")],
                mIncrRangeInt, null, [2.0, 0.5, n, Side.Mathcad.D("T")], qs, ms);
        }
    }

    [SkippableFact]
    public void Range_operators()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);

        var (qMkVec2, mMkVec2) = Method(QRngVal, "mkVec2", DeclaredStaticPublic, QDimVal, QDimVal);
        var (qMkVec3, mMkVec3) = Method(QRngVal, "mkVec3", DeclaredStaticPublic, QDimVal, QDimVal, QDimVal);
        var (qMkRange2, mMkRange2) = Method(QRngVal, "mkRange2", DeclaredStaticPublic, QDimVal, QDimVal);
        var (qMkRange3, mMkRange3) = Method(QRngVal, "mkRange3", DeclaredStaticPublic, QDimVal, QDimVal, QDimVal);

        (string Name, object Q, object M)[] vals =
        [
            ("0", Side.Quire.Num(0, 0, "identity"), Side.Mathcad.Num(0, 0, "identity")),
            ("3", Side.Quire.Num(3, 0, "identity"), Side.Mathcad.Num(3, 0, "identity")),
            ("0.5", Side.Quire.Num(0.5, 0, "identity"), Side.Mathcad.Num(0.5, 0, "identity")),
            ("-2", Side.Quire.Num(-2, 0, "identity"), Side.Mathcad.Num(-2, 0, "identity")),
            ("1m", Side.Quire.Num(1, 0, "L"), Side.Mathcad.Num(1, 0, "L")),
            ("4m", Side.Quire.Num(4, 0, "L"), Side.Mathcad.Num(4, 0, "L")),
            ("2s", Side.Quire.Num(2, 0, "T"), Side.Mathcad.Num(2, 0, "T")),
            ("1+i", Side.Quire.Num(1, 1, "identity"), Side.Mathcad.Num(1, 1, "identity")),
            ("3poly", Side.Quire.Num(3, 0, "poly"), Side.Mathcad.Num(3, 0, "poly")),
            ("str", Side.Quire.Str("x"), Side.Mathcad.Str("x"))
        ];

        foreach (var (an, aQ, aM) in vals)
        {
            foreach (var (bn, bQ, bM) in vals)
            {
                MethodComparison.Compare($"mkVec2({an}, {bn})", Contract,
                    qMkVec2, null, [aQ, bQ], mMkVec2, null, [aM, bM], qs, ms);
                MethodComparison.Compare($"mkRange2({an}, {bn})", Contract,
                    qMkRange2, null, [aQ, bQ], mMkRange2, null, [aM, bM], qs, ms);

                foreach (var (cn, cQ, cM) in vals.Where(v => v.Name is "3" or "4m" or "2s" or "str"))
                {
                    MethodComparison.Compare($"mkVec3({an}, {bn}, {cn})", Contract,
                        qMkVec3, null, [aQ, bQ, cQ], mMkVec3, null, [aM, bM, cM], qs, ms);
                    MethodComparison.Compare($"mkRange3({an}, {bn}, {cn})", Contract,
                        qMkRange3, null, [aQ, bQ, cQ], mMkRange3, null, [aM, bM, cM], qs, ms);
                }
            }
        }
    }

    [SkippableFact]
    public void Equality()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qStateRng = StateRngVal(Side.Quire);
        var mStateRng = StateRngVal(Side.Mathcad);

        var (qEquals, mEquals) = Method(QRngVal, "Equals", DeclaredInstancePublic, typeof(object));

        var ranges = new (string Name, (object Q, object M) Val)[]
        {
            ("a", MakeRange(0, 1, 5, "L")),
            ("a_copy", MakeRange(0, 1, 5, "L")),
            ("diff_n", MakeRange(0, 1, 6, "L")),
            ("diff_dim", MakeRange(0, 1, 5, "T")),
            ("zero_dim", MakeRange(0, 1, 5, "mcd_zero")),
            ("identity", MakeRange(0, 1, 5, "identity")),
            ("interp", MakeInterp(0.0, 1.0, 10, "L")),
            ("plain_step", MakeRange(0.0, 0.1, 11, "L"))
        };

        foreach (var (an, (aQ, aM)) in ranges)
        {
            foreach (var (bn, (bQ, bM)) in ranges)
            {
                MethodComparison.Compare($"{an} == {bn}", Contract,
                    qEquals, aQ, [bQ], mEquals, aM, [bM], qs, ms, qStateRng, mStateRng);
            }

            MethodComparison.Compare($"{an} == number", Contract,
                qEquals, aQ, [Side.Quire.Num(0, 0, "L")],
                mEquals, aM, [Side.Mathcad.Num(0, 0, "L")], qs, ms, qStateRng, mStateRng);

            MethodComparison.Compare($"{an} == null", Contract,
                qEquals, aQ, [null],
                mEquals, aM, [null], qs, ms, qStateRng, mStateRng);
        }
    }

    [SkippableFact]
    public void Iterators()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qStateRng = StateRngVal(Side.Quire);
        var mStateRng = StateRngVal(Side.Mathcad);
        var qStateRangeIt = StateRangeIterator(Side.Quire);
        var mStateRangeIt = StateRangeIterator(Side.Mathcad);
        var qStateScalarIt = StateScalarIterator(Side.Quire);
        var mStateScalarIt = StateScalarIterator(Side.Mathcad);
        var qStateSeqIt = StateSeqIterator(Side.Quire);
        var mStateSeqIt = StateSeqIterator(Side.Mathcad);

        // 2. Iterator static factories
        var (qForRange3, mForRange3) = Method(QIterator, "forRange", DeclaredStaticPublic, typeof(double), typeof(double), typeof(int));
        var (qForRange2, mForRange2) = Method(QIterator, "forRange", DeclaredStaticPublic, typeof(double), typeof(double));
        var (qForRange3Dbl, mForRange3Dbl) = Method(QIterator, "forRange", DeclaredStaticPublic, typeof(double), typeof(double), typeof(double));
        var (qForRange3Dim, mForRange3Dim) = Method(QIterator, "forRange", DeclaredStaticPublic, QDimVal, QDimVal, QDimVal);
        var (qForRange2Dim, mForRange2Dim) = Method(QIterator, "forRange", DeclaredStaticPublic, QDimVal, QDimVal);
        var (qForRangeRng, mForRangeRng) = Method(QIterator, "forRange", DeclaredStaticPublic, QRngVal);
        var (qForValues, mForValues) = Method(QIterator, "forValues", DeclaredStaticPublic, typeof(global::Quire.Mcd.Iterator[]));
        var (qForValue, mForValue) = Method(QIterator, "forValue", DeclaredStaticPublic, typeof(object));
        var (qItIncr, mItIncr) = Method(QIterator, "it_incr", DeclaredStaticPublic, typeof(object), QIterator);

        MethodComparison.Compare("forRange(1, 0.5, 4)", Contract,
            qForRange3, null, [1.0, 0.5, 4], mForRange3, null, [1.0, 0.5, 4], qs, ms);
        MethodComparison.Compare("forRange(1, 0.5, 0)", Contract,
            qForRange3, null, [1.0, 0.5, 0], mForRange3, null, [1.0, 0.5, 0], qs, ms);
        MethodComparison.Compare("forRange(3, 0)", Contract,
            qForRange2, null, [3.0, 0.0], mForRange2, null, [3.0, 0.0], qs, ms);
        MethodComparison.Compare("forRange(0, 0.1, 0.5)", Contract,
            qForRange3Dbl, null, [0.0, 0.1, 0.5], mForRange3Dbl, null, [0.0, 0.1, 0.5], qs, ms);
        MethodComparison.Compare("forRange(0, 0, 1)", Contract,
            qForRange3Dbl, null, [0.0, 0.0, 1.0], mForRange3Dbl, null, [0.0, 0.0, 1.0], qs, ms);

        MethodComparison.Compare("forRange(1m, 1.5m, 3m)", Contract,
            qForRange3Dim, null, [Side.Quire.Num(1, 0, "L"), Side.Quire.Num(1.5, 0, "L"), Side.Quire.Num(3, 0, "L")],
            mForRange3Dim, null, [Side.Mathcad.Num(1, 0, "L"), Side.Mathcad.Num(1.5, 0, "L"), Side.Mathcad.Num(3, 0, "L")], qs, ms);
        MethodComparison.Compare("forRange(1m, 2s, 3m)", Contract,
            qForRange3Dim, null, [Side.Quire.Num(1, 0, "L"), Side.Quire.Num(2, 0, "T"), Side.Quire.Num(3, 0, "L")],
            mForRange3Dim, null, [Side.Mathcad.Num(1, 0, "L"), Side.Mathcad.Num(2, 0, "T"), Side.Mathcad.Num(3, 0, "L")], qs, ms);
        MethodComparison.Compare("forRange(1, 4 dv)", Contract,
            qForRange2Dim, null, [Side.Quire.Num(1, 0, "identity"), Side.Quire.Num(4, 0, "identity")],
            mForRange2Dim, null, [Side.Mathcad.Num(1, 0, "identity"), Side.Mathcad.Num(4, 0, "identity")], qs, ms);
        MethodComparison.Compare("forRange(1m, 4m dv)", Contract,
            qForRange2Dim, null, [Side.Quire.Num(1, 0, "L"), Side.Quire.Num(4, 0, "L")],
            mForRange2Dim, null, [Side.Mathcad.Num(1, 0, "L"), Side.Mathcad.Num(4, 0, "L")], qs, ms);

        var (qR, mR) = MakeRange(0.0, 2.0, 3, "T");
        MethodComparison.Compare("forRange(RngVal)", Contract,
            qForRangeRng, null, [qR], mForRangeRng, null, [mR], qs, ms);

        MethodComparison.Compare("forValue(range)", Contract,
            qForValue, null, [qR], mForValue, null, [mR], qs, ms);
        MethodComparison.Compare("forValue(number)", Contract,
            qForValue, null, [Side.Quire.Num(7, 0, "L")],
            mForValue, null, [Side.Mathcad.Num(7, 0, "L")], qs, ms);
        MethodComparison.Compare("forValue(string)", Contract,
            qForValue, null, ["text"], mForValue, null, ["text"], qs, ms);
        MethodComparison.Compare("forValue(null)", Contract,
            qForValue, null, [null], mForValue, null, [null], qs, ms);

        var (qMatCtor2, mMatCtor2) = Ctor(QMatVal, DeclaredInstancePublic, typeof(int), typeof(int));
        var qMat2x3 = qMatCtor2.Invoke([2, 3])!;
        var mMat2x3 = mMatCtor2.Invoke([2, 3])!;
        MethodComparison.Compare("forValue(matrix 2x3)", Contract,
            qForValue, null, [qMat2x3], mForValue, null, [mMat2x3], qs, ms);

        foreach (var (r, c) in new[] { (1, 3), (3, 1) })
        {
            MethodComparison.Compare($"forValue(matrix {r}x{c})", Contract,
                qForValue, null, [qMatCtor2.Invoke([r, c])], mForValue, null, [mMatCtor2.Invoke([r, c])], qs, ms);
        }

        var qMat0x0 = qMatCtor2.Invoke([0, 0])!;
        var mMat0x0 = mMatCtor2.Invoke([0, 0])!;
        MethodComparison.Compare("forValue(matrix 0x0)", Contract,
            qForValue, null, [qMat0x0], mForValue, null, [mMat0x0], qs, ms);

        // 3. ScalarIterator constructor and methods
        var (qScalarCtor, mScalarCtor) = Ctor(QScalarIterator, DeclaredInstancePublic, typeof(object));
        var (qScalarFirst, mScalarFirst) = Method(QScalarIterator, "first", DeclaredInstancePublic);
        var (qScalarNext, mScalarNext) = Method(QScalarIterator, "next", DeclaredInstancePublic);

        MethodComparison.Compare("ScalarIterator('val')", Contract,
            qScalarCtor, null, ["val"], mScalarCtor, null, ["val"], qs, ms);
        MethodComparison.Compare("ScalarIterator(null)", Contract,
            qScalarCtor, null, [null], mScalarCtor, null, [null], qs, ms);

        var qSc = qScalarCtor.Invoke(["hello"])!;
        var mSc = mScalarCtor.Invoke(["hello"])!;
        MethodComparison.Compare("ScalarIterator.first()", Contract,
            qScalarFirst, qSc, [], mScalarFirst, mSc, [], qs, ms, qStateScalarIt, mStateScalarIt);
        MethodComparison.Compare("ScalarIterator.next()", Contract,
            qScalarNext, qSc, [], mScalarNext, mSc, [], qs, ms, qStateScalarIt, mStateScalarIt);

        // 4. RangeIterator constructor and methods
        var (qRangeItCtor, mRangeItCtor) = Ctor(QRangeIterator, DeclaredInstancePublic, QRngVal);
        var (qRangeItFirst, mRangeItFirst) = Method(QRangeIterator, "first", DeclaredInstancePublic);
        var (qRangeItRiFirst, mRangeItRiFirst) = Method(QRangeIterator, "ri_first", DeclaredInstancePublic);
        var (qRangeItNext, mRangeItNext) = Method(QRangeIterator, "next", DeclaredInstancePublic);
        var (qRangeItRiNext, mRangeItRiNext) = Method(QRangeIterator, "ri_next", DeclaredInstancePublic);

        var (qR2, mR2) = MakeRange(0.0, 1.0, 2, "identity");
        MethodComparison.Compare("RangeIterator(RngVal)", Contract,
            qRangeItCtor, null, [qR2], mRangeItCtor, null, [mR2], qs, ms);

        var qRi = qRangeItCtor.Invoke([qR2])!;
        var mRi = mRangeItCtor.Invoke([mR2])!;

        MethodComparison.Compare("RangeIterator.first() initial", Contract,
            qRangeItFirst, qRi, [], mRangeItFirst, mRi, [], qs, ms, qStateRangeIt, mStateRangeIt);
        MethodComparison.Compare("RangeIterator.ri_first() initial", Contract,
            qRangeItRiFirst, qRi, [], mRangeItRiFirst, mRi, [], qs, ms, qStateRangeIt, mStateRangeIt);

        MethodComparison.Compare("RangeIterator.ri_next() step 1", Contract,
            qRangeItRiNext, qRi, [], mRangeItRiNext, mRi, [], qs, ms, qStateRangeIt, mStateRangeIt);
        MethodComparison.Compare("RangeIterator.ri_first() step 1", Contract,
            qRangeItRiFirst, qRi, [], mRangeItRiFirst, mRi, [], qs, ms, qStateRangeIt, mStateRangeIt);

        MethodComparison.Compare("RangeIterator.next() step 2 (end)", Contract,
            qRangeItNext, qRi, [], mRangeItNext, mRi, [], qs, ms, qStateRangeIt, mStateRangeIt);
        MethodComparison.Compare("RangeIterator.first() past end", Contract,
            qRangeItFirst, qRi, [], mRangeItFirst, mRi, [], qs, ms, qStateRangeIt, mStateRangeIt);
        MethodComparison.Compare("RangeIterator.next() past end again", Contract,
            qRangeItNext, qRi, [], mRangeItNext, mRi, [], qs, ms, qStateRangeIt, mStateRangeIt);

        // 5. SeqIterator constructor and methods
        var (qSeqCtor, mSeqCtor) = Ctor(QSeqIterator, DeclaredInstancePublic, typeof(global::Quire.Mcd.Iterator[]));
        var (qSeqFirst, mSeqFirst) = Method(QSeqIterator, "first", DeclaredInstancePublic);
        var (qSeqNext, mSeqNext) = Method(QSeqIterator, "next", DeclaredInstancePublic);

        var (qChildR, mChildR) = MakeRange(0.0, 1.0, 2, "identity");
        var qChild1 = (global::Quire.Mcd.Iterator)qRangeItCtor.Invoke([qChildR])!;
        var mChild1 = mRangeItCtor.Invoke([mChildR])!;
        var qChild2 = (global::Quire.Mcd.Iterator)qScalarCtor.Invoke(["end"])!;
        var mChild2 = mScalarCtor.Invoke(["end"])!;

        var (qIts, mIts) = (new global::Quire.Mcd.Iterator[] { qChild1, qChild2 }, Array.CreateInstance(MIterator, 2));
        mIts.SetValue(mChild1, 0);
        mIts.SetValue(mChild2, 1);

        MethodComparison.Compare("SeqIterator(its)", Contract,
            qSeqCtor, null, [qIts], mSeqCtor, null, [mIts], qs, ms);

        var (qEmptyIts, mEmptyIts) = (new global::Quire.Mcd.Iterator[0], Array.CreateInstance(MIterator, 0));
        MethodComparison.Compare("forValues(empty)", Contract,
            qForValues, null, [qEmptyIts], mForValues, null, [mEmptyIts], qs, ms);
        MethodComparison.Compare("forValues(child1, child2)", Contract,
            qForValues, null, [qIts], mForValues, null, [mIts], qs, ms);

        var qSeq = qSeqCtor.Invoke([qIts])!;
        var mSeq = mSeqCtor.Invoke([mIts])!;

        MethodComparison.Compare("SeqIterator.first()", Contract,
            qSeqFirst, qSeq, [], mSeqFirst, mSeq, [], qs, ms, qStateSeqIt, mStateSeqIt);
        MethodComparison.Compare("SeqIterator.next() child 1 advance", Contract,
            qSeqNext, qSeq, [], mSeqNext, mSeq, [], qs, ms, qStateSeqIt, mStateSeqIt);
        MethodComparison.Compare("SeqIterator.next() switch to child 2", Contract,
            qSeqNext, qSeq, [], mSeqNext, mSeq, [], qs, ms, qStateSeqIt, mStateSeqIt);
        MethodComparison.Compare("SeqIterator.first() on child 2", Contract,
            qSeqFirst, qSeq, [], mSeqFirst, mSeq, [], qs, ms, qStateSeqIt, mStateSeqIt);
        MethodComparison.Compare("SeqIterator.next() exhaust child 2", Contract,
            qSeqNext, qSeq, [], mSeqNext, mSeq, [], qs, ms, qStateSeqIt, mStateSeqIt);

        // 6. it_incr
        var (qRCounting, mRCounting) = MakeRange(5.0, 1.0, 2, "identity");
        var qItCounting = qForRangeRng.Invoke(null, [qRCounting])!;
        var mItCounting = mForRangeRng.Invoke(null, [mRCounting])!;

        MethodComparison.Compare("it_incr step 1", Contract,
            qItIncr, null, [null, qItCounting], mItIncr, null, [null, mItCounting], qs, ms);
        MethodComparison.Compare("it_incr step 2", Contract,
            qItIncr, null, [null, qItCounting], mItIncr, null, [null, mItCounting], qs, ms);
        MethodComparison.Compare("it_incr step 3 past end", Contract,
            qItIncr, null, [null, qItCounting], mItIncr, null, [null, mItCounting], qs, ms);
    }
}
