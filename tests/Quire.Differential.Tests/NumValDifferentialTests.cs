using System.Globalization;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.NumVal</c>, <c>Mcd.num_val</c>, <c>Mcd.NotAVal</c>, and <c>Mcd.StrVal</c> with Quire's
/// method by method using direct <see cref="MethodComparison.Compare"/> calls across all branches and edge cases.
/// Evidence contracts: Mcd.NumVal, Mcd.num_val, Mcd.NotAVal, Mcd.StrVal.
/// </summary>
public class NumValDifferentialTests
{
    private const string ContractNumVal = "Mcd.NumVal";
    private const string ContractNum_Val = "Mcd.num_val";
    private const string ContractNotAVal = "Mcd.NotAVal";
    private const string ContractStrVal = "Mcd.StrVal";

    private const BindingFlags DeclaredPublic = BindingFlags.Public | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPublic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStaticPrivate = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static System.Type MNumVal => Mathcad.Type("mplrun", "Mcd.NumVal");
    private static System.Type QNumVal => typeof(global::Quire.Mcd.NumVal);
    private static System.Type Mnum_val => Mathcad.Type("mplrun", "Mcd.num_val");
    private static System.Type Qnum_val => typeof(global::Quire.Mcd.num_val);
    private static System.Type MNotAVal => Mathcad.Type("mplrun", "Mcd.NotAVal");
    private static System.Type QNotAVal => typeof(global::Quire.Mcd.NotAVal);
    private static System.Type MStrVal => Mathcad.Type("mplrun", "Mcd.StrVal");
    private static System.Type QStrVal => typeof(global::Quire.Mcd.StrVal);
    private static System.Type MDim => Mathcad.Type("mplrun", "Mcd.Dim");
    private static System.Type QDim => typeof(global::Quire.Mcd.Dim);
    private static System.Type MDimVal => Mathcad.Type("mplrun", "Mcd.DimVal");
    private static System.Type QDimVal => typeof(global::Quire.Mcd.DimVal);

    private static System.Type MapToM(System.Type t)
    {
        if (t == QNumVal) return MNumVal;
        if (t == Qnum_val) return Mnum_val;
        if (t == QNotAVal) return MNotAVal;
        if (t == QStrVal) return MStrVal;
        if (t == QDim) return MDim;
        if (t == QDimVal) return MDimVal;
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

    private static (ConstructorInfo q, ConstructorInfo m) Ctor(System.Type qType, params System.Type[] qParamTypes)
    {
        var mType = MapToM(qType);
        var mParamTypes = qParamTypes.Select(MapToM).ToArray();
        var q = qType.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, qParamTypes, null)
            ?? throw new HarnessException($"Quire constructor not found: {qType.Name}({string.Join(", ", qParamTypes.Select(t => t.Name))})");
        var m = mType.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, mParamTypes, null)
            ?? throw new HarnessException($"Mathcad constructor not found: {mType.Name}({string.Join(", ", mParamTypes.Select(t => t.Name))})");
        return (q, m);
    }

    private static string ShowVal(Side side, object? v)
    {
        if (v == null) return "null";
        if (v is Pointer) return "ptr";
        if (v is string or int or uint or double or bool or long or byte or short) return Text(v);
        if (v.GetType().Name == "num_val") return $"num_val[rp={Get(v, "rp")},ip={Get(v, "ip")}]";
        if (v.GetType().Name == "NumVal")
        {
            var data = Get(v, "Data");
            return $"NumVal[rp={Get(data!, "rp")},ip={Get(data!, "ip")},dim={side.DimText(Get(v, "m_dim"))}]";
        }
        if (v.GetType().Name == "StrVal") return $"StrVal[{Text(Get(v, "Field1"))},dim={side.DimText(Get(v, "m_dim"))}]";
        if (v.GetType().Name == "NotAVal") return "NotAVal";
        if (v.GetType().Name == "Dim") return side.DimText(v);
        if (v.GetType().Name == "True") return "True";
        return Text(v);
    }

    private static Func<object?, string> Show(Side side) => v => ShowVal(side, v);

    private static Func<object, string> StateNumVal(Side side) => v =>
    {
        var data = Get(v, "Data");
        return $"NumVal[rp={Get(data!, "rp")},ip={Get(data!, "ip")},dim={side.DimText(Get(v, "m_dim"))}]";
    };
    private static Func<object, string> StateStrVal(Side side) => v => $"StrVal[{Text(Get(v, "Field1"))},dim={side.DimText(Get(v, "m_dim"))}]";
    private static Func<object, string> StateNotAVal(Side side) => _ => "NotAVal";

    private static readonly double[] Doubles =
    [
        0.0, -0.0, 1.0, -1.0, 2.5, 3.5, -2.5, 0.1, 1e-14, -1e-14, 1 + 1e-13, 1 + 1e-12, 5.0000000000001, 1e300, -1e300,
        2147483647.0, 2147483648.0, -2147483648.0, -2147483649.0, 3e9, 4294967295.0, 4294967296.0, 4503599627370496.0, 9007199254740993.0,
        double.Epsilon, double.MaxValue, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
    ];

    private static List<(string Name, object Q, object M)> SampleNumbers()
    {
        var list = new List<(string, object, object)>();
        foreach (var (re, im) in new[] { (0.0, 0.0), (1.0, 0.0), (1.0 + 1e-13, 0.0), (1.0 + 1e-11, 0.0), (2.0, 0.0), (-2.0, 0.0),
                     (0.0, 1.0), (0.0, 2.0), (1.0, 1.0), (1.0, -1.0), (1.0 + 1e-13, 1.0), (double.NaN, 0.0), (double.PositiveInfinity, 0.0) })
        {
            foreach (var dim in new[] { "poly", "identity", "mcd_zero", "L", "T" })
            {
                list.Add(($"({Side.Bits(re)}, {Side.Bits(im)}) {dim}", Side.Quire.Num(re, im, dim), Side.Mathcad.Num(re, im, dim)));
            }
        }
        return list;
    }

    [SkippableFact]
    public void Statics_and_construction()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = StateNumVal(Side.Quire);
        var mState = StateNumVal(Side.Mathcad);

        // --- num_val constructors ---
        var (qNumValCtor1, mNumValCtor1) = Ctor(Qnum_val, typeof(double));
        var (qNumValCtor2, mNumValCtor2) = Ctor(Qnum_val, typeof(double), typeof(double));
        MethodComparison.Compare("num_val(2.5)", ContractNum_Val, qNumValCtor1, null, [2.5], mNumValCtor1, null, [2.5], qs, ms);
        MethodComparison.Compare("num_val(0.0)", ContractNum_Val, qNumValCtor1, null, [0.0], mNumValCtor1, null, [0.0], qs, ms);
        MethodComparison.Compare("num_val(2.5, -1.5)", ContractNum_Val, qNumValCtor2, null, [2.5, -1.5], mNumValCtor2, null, [2.5, -1.5], qs, ms);
        MethodComparison.Compare("num_val(0, 0)", ContractNum_Val, qNumValCtor2, null, [0.0, 0.0], mNumValCtor2, null, [0.0, 0.0], qs, ms);

        // --- Static constructors (.cctor) ---
        var qNotAValCc = QNotAVal.TypeInitializer!;
        var mNotAValCc = MNotAVal.TypeInitializer!;
        MethodComparison.Compare("NotAVal.cctor", ContractNotAVal, qNotAValCc, null, [], mNotAValCc, null, [], qs, ms);

        var qNumValCc = QNumVal.TypeInitializer!;
        var mNumValCc = MNumVal.TypeInitializer!;
        MethodComparison.Compare("NumVal.cctor", ContractNumVal, qNumValCc, null, [], mNumValCc, null, [], qs, ms);

        // --- NotAVal constructor ---
        var (qNotAValCtor, mNotAValCtor) = Ctor(QNotAVal);
        MethodComparison.Compare("new NotAVal()", ContractNotAVal, qNotAValCtor, null, [], mNotAValCtor, null, [], qs, ms);

        // --- NumVal constructors ---
        var (qCtor0, mCtor0) = Ctor(QNumVal);
        var (qCtorInt, mCtorInt) = Ctor(QNumVal, typeof(int));
        var (qCtorDbl, mCtorDbl) = Ctor(QNumVal, typeof(double));
        var (qCtorDblDbl, mCtorDblDbl) = Ctor(QNumVal, typeof(double), typeof(double));
        var (qCtorDblDblDim, mCtorDblDblDim) = Ctor(QNumVal, typeof(double), typeof(double), QDim);
        var (qCtorCopy, mCtorCopy) = Ctor(QNumVal, QNumVal);
        var (qCtorCopyDim, mCtorCopyDim) = Ctor(QNumVal, QNumVal, QDim);

        MethodComparison.Compare("new NumVal()", ContractNumVal, qCtor0, null, [], mCtor0, null, [], qs, ms);
        MethodComparison.Compare("new NumVal(int 7)", ContractNumVal, qCtorInt, null, [7], mCtorInt, null, [7], qs, ms);
        MethodComparison.Compare("new NumVal(int 0)", ContractNumVal, qCtorInt, null, [0], mCtorInt, null, [0], qs, ms);
        MethodComparison.Compare("new NumVal(int -5)", ContractNumVal, qCtorInt, null, [-5], mCtorInt, null, [-5], qs, ms);
        MethodComparison.Compare("new NumVal(double 7.5)", ContractNumVal, qCtorDbl, null, [7.5], mCtorDbl, null, [7.5], qs, ms);
        MethodComparison.Compare("new NumVal(double 0.0)", ContractNumVal, qCtorDbl, null, [0.0], mCtorDbl, null, [0.0], qs, ms);
        MethodComparison.Compare("new NumVal(double, double)", ContractNumVal, qCtorDblDbl, null, [7.5, -1.0], mCtorDblDbl, null, [7.5, -1.0], qs, ms);
        MethodComparison.Compare("new NumVal(double, double, Dim)", ContractNumVal, qCtorDblDblDim, null, [7.5, -1.0, Side.Quire.D("L")], mCtorDblDblDim, null, [7.5, -1.0, Side.Mathcad.D("L")], qs, ms);
        MethodComparison.Compare("new NumVal(NumVal)", ContractNumVal, qCtorCopy, null, [Side.Quire.Num(7.5, -1, "L")], mCtorCopy, null, [Side.Mathcad.Num(7.5, -1, "L")], qs, ms);
        MethodComparison.Compare("new NumVal(NumVal, Dim)", ContractNumVal, qCtorCopyDim, null, [Side.Quire.Num(7.5, -1, "L"), Side.Quire.D("T")], mCtorCopyDim, null, [Side.Mathcad.Num(7.5, -1, "L"), Side.Mathcad.D("T")], qs, ms);
        MethodComparison.Compare("new NumVal(null)", ContractNumVal, qCtorCopy, null, [null], mCtorCopy, null, [null], qs, ms);

        // --- Static factories & utilities ---
        var (qNumDv, mNumDv) = Method(QNumVal, "NumDv", DeclaredStaticPublic, typeof(double), typeof(double));
        var (qIntDv, mIntDv) = Method(QNumVal, "IntDv", DeclaredStaticPublic, typeof(int));
        var (qRealDv, mRealDv) = Method(QNumVal, "RealDv", DeclaredStaticPublic, typeof(double));
        var (qLoadNaN, mLoadNaN) = Method(QNumVal, "LoadNaN", DeclaredStaticPublic, typeof(int));
        var (qMcdBool, mMcdBool) = Method(QNumVal, "McdBool", DeclaredStaticPublic, typeof(object));
        var (qIbv, mIbv) = Method(QNumVal, "ibv", DeclaredStaticPublic, typeof(int));
        var (qNotNoValue, mNotNoValue) = Method(QNumVal, "NotNoValue", DeclaredStaticPublic, typeof(object));
        var (qDiv, mDiv) = Method(QNumVal, "op_Division", DeclaredStaticPublic, QNumVal, typeof(double));

        MethodComparison.Compare("NumDv(1.5, 2.5)", ContractNumVal, qNumDv, null, [1.5, 2.5], mNumDv, null, [1.5, 2.5], qs, ms);
        MethodComparison.Compare("IntDv(-3)", ContractNumVal, qIntDv, null, [-3], mIntDv, null, [-3], qs, ms);
        MethodComparison.Compare("RealDv(4.5)", ContractNumVal, qRealDv, null, [4.5], mRealDv, null, [4.5], qs, ms);

        foreach (var payload in new[] { 0, 1, 0x12345678, -1, int.MinValue })
        {
            MethodComparison.Compare($"LoadNaN({payload})", ContractNumVal, qLoadNaN, null, [payload], mLoadNaN, null, [payload], qs, ms);
        }

        var rQ = Side.Quire.Num(1, 2, "L");
        var rM = Side.Mathcad.Num(1, 2, "L");
        foreach (var o in new object?[] { null, "x", false, 0, rQ })
        {
            var mArg = ReferenceEquals(o, rQ) ? rM : o;
            MethodComparison.Compare($"McdBool({Text(o)})", ContractNumVal, qMcdBool, null, [o], mMcdBool, null, [mArg], qs, ms);
        }

        foreach (int i in new[] { 0, 1, -1 })
        {
            MethodComparison.Compare($"ibv({i})", ContractNumVal, qIbv, null, [i], mIbv, null, [i], qs, ms);
        }

        MethodComparison.Compare("NotNoValue(NoValue)", ContractNumVal, qNotNoValue, null, [Get(QNumVal, "NoValue")], mNotNoValue, null, [Get(MNumVal, "NoValue")], qs, ms);
        MethodComparison.Compare("NotNoValue(new NotAVal)", ContractNumVal, qNotNoValue, null, [Side.Quire.NotAVal()], mNotNoValue, null, [Side.Mathcad.NotAVal()], qs, ms);
        MethodComparison.Compare("NotNoValue(null)", ContractNumVal, qNotNoValue, null, [null], mNotNoValue, null, [null], qs, ms);

        foreach (var (n, vQ, vM) in SampleNumbers())
        {
            foreach (var d in new[] { 2.0, 0.0, double.NaN })
            {
                MethodComparison.Compare($"{n} / {Side.Bits(d)}", ContractNumVal, qDiv, null, [vQ, d], mDiv, null, [vM, d], qs, ms);
            }
        }

        // --- Set instance methods ---
        var (qSetDbl, mSetDbl) = Method(QNumVal, "Set", DeclaredInstancePublic, typeof(double));
        var (qSetDblDbl, mSetDblDbl) = Method(QNumVal, "Set", DeclaredInstancePublic, typeof(double), typeof(double));
        var (qSetInt, mSetInt) = Method(QNumVal, "Set", DeclaredInstancePublic, typeof(int));

        foreach (var d in Doubles)
        {
            var tQ1 = Side.Quire.Num(9, 9, "L");
            var tM1 = Side.Mathcad.Num(9, 9, "L");
            MethodComparison.Compare($"Set({Side.Bits(d)})", ContractNumVal, qSetDbl, tQ1, [d], mSetDbl, tM1, [d], qs, ms, qState, mState);

            var tQ2 = Side.Quire.Num(9, 9, "L");
            var tM2 = Side.Mathcad.Num(9, 9, "L");
            MethodComparison.Compare($"Set({Side.Bits(d)}, 2)", ContractNumVal, qSetDblDbl, tQ2, [d, 2.0], mSetDblDbl, tM2, [d, 2.0], qs, ms, qState, mState);
        }

        foreach (int i in new[] { 0, 5, -5, int.MaxValue })
        {
            var tQ = Side.Quire.Num(9, 9, "L");
            var tM = Side.Mathcad.Num(9, 9, "L");
            MethodComparison.Compare($"Set(int {i})", ContractNumVal, qSetInt, tQ, [i], mSetInt, tM, [i], qs, ms, qState, mState);
        }

        // --- Property setters (set_Real, set_Imag) ---
        var (qSetReal, mSetReal) = Method(QNumVal, "set_Real", DeclaredInstancePublic, typeof(double));
        var (qSetImag, mSetImag) = Method(QNumVal, "set_Imag", DeclaredInstancePublic, typeof(double));

        var tQReal = Side.Quire.Num(1, 2, "L");
        var tMReal = Side.Mathcad.Num(1, 2, "L");
        MethodComparison.Compare("set_Real(5.0)", ContractNumVal, qSetReal, tQReal, [5.0], mSetReal, tMReal, [5.0], qs, ms, qState, mState);

        var tQImag = Side.Quire.Num(1, 2, "L");
        var tMImag = Side.Mathcad.Num(1, 2, "L");
        MethodComparison.Compare("set_Imag(6.0)", ContractNumVal, qSetImag, tQImag, [6.0], mSetImag, tMImag, [6.0], qs, ms, qState, mState);
    }

    [SkippableFact]
    public void Properties_and_conversions()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = StateNumVal(Side.Quire);
        var mState = StateNumVal(Side.Mathcad);

        var (qGetPhysDim, mGetPhysDim) = Method(QNumVal, "get_PhysicalDimension", DeclaredInstancePublic);
        var (qGetPureReal, mGetPureReal) = Method(QNumVal, "get_IsPureReal", DeclaredInstancePublic);
        var (qGetPureImag, mGetPureImag) = Method(QNumVal, "get_IsPureImag", DeclaredInstancePublic);
        var (qGetReal, mGetReal) = Method(QNumVal, "get_Real", DeclaredInstancePublic);
        var (qGetImag, mGetImag) = Method(QNumVal, "get_Imag", DeclaredInstancePublic);
        var (qGetIsInt, mGetIsInt) = Method(QNumVal, "get_IsInt", DeclaredInstancePublic);
        var (qGetIsReal, mGetIsReal) = Method(QNumVal, "get_IsReal", DeclaredInstancePublic);

        var (qGetHashCode, mGetHashCode) = Method(QNumVal, "GetHashCode", DeclaredInstancePublic);
        var (qAsResult, mAsResult) = Method(QNumVal, "asResult", DeclaredInstancePublic);
        var (qAsReal, mAsReal) = Method(QNumVal, "asReal", DeclaredInstancePublic, typeof(string), typeof(uint));
        var (qSetDim, mSetDim) = Method(QNumVal, "SetDim", DeclaredInstancePublic, QDim);

        foreach (var (n, vQ, vM) in SampleNumbers())
        {
            MethodComparison.Compare($"{n}.PhysicalDimension", ContractNumVal, qGetPhysDim, vQ, [], mGetPhysDim, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.IsPureReal", ContractNumVal, qGetPureReal, vQ, [], mGetPureReal, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.IsPureImag", ContractNumVal, qGetPureImag, vQ, [], mGetPureImag, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.Real", ContractNumVal, qGetReal, vQ, [], mGetReal, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.Imag", ContractNumVal, qGetImag, vQ, [], mGetImag, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.IsInt", ContractNumVal, qGetIsInt, vQ, [], mGetIsInt, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.IsReal", ContractNumVal, qGetIsReal, vQ, [], mGetIsReal, vM, [], qs, ms, qState, mState);

            MethodComparison.Compare($"{n}.GetHashCode", ContractNumVal, qGetHashCode, vQ, [], mGetHashCode, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.asResult", ContractNumVal, qAsResult, vQ, [], mAsResult, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.asReal(m, 3)", ContractNumVal, qAsReal, vQ, ["custom key", 3u], mAsReal, vM, ["custom key", 3u], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.SetDim(T)", ContractNumVal, qSetDim, vQ, [Side.Quire.D("T")], mSetDim, vM, [Side.Mathcad.D("T")], qs, ms, qState, mState);
        }

        // --- Integer detection: isInt(int*) and isNearInt(int*, double) ---
        unsafe
        {
            var (qIsInt, mIsInt) = Method(QNumVal, "isInt", DeclaredInstancePublic, typeof(int*));
            var (qIsNearInt, mIsNearInt) = Method(QNumVal, "isNearInt", DeclaredStaticPrivate, typeof(int*), typeof(double));

            foreach (var d in Doubles)
            {
                foreach (var im in new[] { 0.0, 1.0 })
                {
                    int qRes = -1, mRes = -1;
                    var numQ = Side.Quire.Num(d, im, "identity");
                    var numM = Side.Mathcad.Num(d, im, "identity");
                    MethodComparison.Compare($"isInt({Side.Bits(d)}, {im})", ContractNumVal, qIsInt, numQ,
                        [Pointer.Box(&qRes, typeof(int*))],
                        mIsInt, numM,
                        [Pointer.Box(&mRes, typeof(int*))],
                        qs, ms, qState, mState);
                }

                int qNearRes = -1, mNearRes = -1;
                MethodComparison.Compare($"isNearInt({Side.Bits(d)})", ContractNumVal, qIsNearInt, null,
                    [Pointer.Box(&qNearRes, typeof(int*)), d],
                    mIsNearInt, null,
                    [Pointer.Box(&mNearRes, typeof(int*)), d],
                    qs, ms);
            }
        }
    }

    [SkippableFact]
    public void Equality_and_order()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = StateNumVal(Side.Quire);
        var mState = StateNumVal(Side.Mathcad);

        var (qExactEquals, mExactEquals) = Method(QNumVal, "ExactEquals", DeclaredStaticPublic, QNumVal, QNumVal);
        var (qFuzzyEquals2, mFuzzyEquals2) = Method(QNumVal, "FuzzyEquals", DeclaredStaticPublic, QNumVal, QNumVal);
        var (qFuzzyEquals3, mFuzzyEquals3) = Method(QNumVal, "FuzzyEquals", DeclaredStaticPublic, QNumVal, QNumVal, typeof(double));
        var (qFuzzyEqualsDbl2, mFuzzyEqualsDbl2) = Method(QNumVal, "FuzzyEquals", DeclaredStaticPublic, typeof(double), typeof(double));
        var (qFuzzyEqualsDbl3, mFuzzyEqualsDbl3) = Method(QNumVal, "FuzzyEquals", DeclaredStaticPublic, typeof(double), typeof(double), typeof(double));

        var (qEquals, mEquals) = Method(QNumVal, "Equals", DeclaredInstancePublic, typeof(object));
        var (qEquals1, mEquals1) = Method(QNumVal, "Equals1", DeclaredInstancePublic, typeof(object));
        var (qNearlyEqual, mNearlyEqual) = Method(QNumVal, "nearly_equal", DeclaredInstancePublic, QDimVal, typeof(double));

        var (qLessThan, mLessThan) = Method(QNumVal, "LessThan", DeclaredStaticPublic, QNumVal, QNumVal);
        var (qGreaterThan, mGreaterThan) = Method(QNumVal, "GreaterThan", DeclaredStaticPublic, QNumVal, QNumVal);
        var (qLessEqThan, mLessEqThan) = Method(QNumVal, "LessEqThan", DeclaredStaticPublic, QNumVal, QNumVal);
        var (qGreaterEqThan, mGreaterEqThan) = Method(QNumVal, "GreaterEqThan", DeclaredStaticPublic, QNumVal, QNumVal);

        var (qLT, mLT) = Method(QNumVal, "LT", DeclaredStaticPublic, typeof(double), typeof(double));
        var (qGT, mGT) = Method(QNumVal, "GT", DeclaredStaticPublic, typeof(double), typeof(double));
        var (qLE, mLE) = Method(QNumVal, "LE", DeclaredStaticPublic, typeof(double), typeof(double));
        var (qGE, mGE) = Method(QNumVal, "GE", DeclaredStaticPublic, typeof(double), typeof(double));

        var pairs = new (string Desc, (double r1, double i1, string d1) A, (double r2, double i2, string d2) B)[]
        {
            ("equal reals", (1.0, 0.0, "identity"), (1.0, 0.0, "identity")),
            ("reals within fuzz", (1.0, 0.0, "identity"), (1.0 + 1e-13, 0.0, "identity")),
            ("reals outside fuzz", (1.0, 0.0, "identity"), (1.0 + 1e-11, 0.0, "identity")),
            ("reals distinct", (1.0, 0.0, "identity"), (2.0, 0.0, "identity")),
            ("reals negative", (-2.0, 0.0, "identity"), (-1.0, 0.0, "identity")),
            ("zeros identity", (0.0, 0.0, "identity"), (0.0, 0.0, "identity")),
            ("zero marker vs identity", (0.0, 0.0, "mcd_zero"), (0.0, 0.0, "identity")),
            ("zero marker vs dim L", (0.0, 0.0, "mcd_zero"), (0.0, 0.0, "L")),
            ("polymorphic vs identity", (1.0, 0.0, "poly"), (1.0, 0.0, "identity")),
            ("dim L vs dim T", (1.0, 0.0, "L"), (1.0, 0.0, "T")),
            ("equal pure imag", (0.0, 1.0, "identity"), (0.0, 1.0, "identity")),
            ("pure imag within fuzz", (0.0, 1.0, "identity"), (0.0, 1.0 + 1e-13, "identity")),
            ("pure imag distinct", (0.0, 1.0, "identity"), (0.0, 2.0, "identity")),
            ("pure imag negative", (0.0, -2.0, "identity"), (0.0, -1.0, "identity")),
            ("equal complex", (1.0, 2.0, "identity"), (1.0, 2.0, "identity")),
            ("complex within fuzz", (1.0, 2.0, "identity"), (1.0 + 1e-13, 2.0, "identity")),
            ("complex diff rp", (1.0, 2.0, "identity"), (2.0, 2.0, "identity")),
            ("complex diff ip", (1.0, 2.0, "identity"), (1.0, 3.0, "identity")),
            ("real vs imag (incomparable)", (1.0, 0.0, "identity"), (0.0, 1.0, "identity")),
            ("imag vs real (incomparable)", (0.0, 1.0, "identity"), (1.0, 0.0, "identity")),
            ("real vs complex", (1.0, 0.0, "identity"), (1.0, 1.0, "identity")),
            ("complex diff dim", (1.0, 2.0, "L"), (1.0, 2.0, "T")),
            ("NaN vs zero", (double.NaN, 0.0, "identity"), (0.0, 0.0, "identity")),
            ("NaN vs NaN", (double.NaN, 0.0, "identity"), (double.NaN, 0.0, "identity")),
            ("Inf vs zero", (double.PositiveInfinity, 0.0, "identity"), (0.0, 0.0, "identity")),
            ("Inf vs Inf", (double.PositiveInfinity, 0.0, "identity"), (double.PositiveInfinity, 0.0, "identity")),
            ("Inf vs -Inf", (double.PositiveInfinity, 0.0, "identity"), (double.NegativeInfinity, 0.0, "identity")),
            ("large ints", (2147483648.0, 0.0, "identity"), (2147483647.0, 0.0, "identity")),
        };

        var doublePairs = new (string Desc, double X, double Y)[]
        {
            ("0 == 0", 0.0, 0.0),
            ("0 == -0", 0.0, -0.0),
            ("-0 == 0", -0.0, 0.0),
            ("1 == 1", 1.0, 1.0),
            ("1 < 2", 1.0, 2.0),
            ("2 > 1", 2.0, 1.0),
            ("1 vs 1+1e-13", 1.0, 1.0 + 1e-13),
            ("1+1e-13 vs 1", 1.0 + 1e-13, 1.0),
            ("1 vs 1-1e-13", 1.0, 1.0 - 1e-13),
            ("1-1e-13 vs 1", 1.0 - 1e-13, 1.0),
            ("1 vs 1+1e-11", 1.0, 1.0 + 1e-11),
            ("1+1e-11 vs 1", 1.0 + 1e-11, 1.0),
            ("-1 vs 1", -1.0, 1.0),
            ("1 vs -1", 1.0, -1.0),
            ("-2 vs -1", -2.0, -1.0),
            ("-1 vs -2", -1.0, -2.0),
            ("0 vs 1e-14", 0.0, 1e-14),
            ("0 vs -1e-14", 0.0, -1e-14),
            ("int.Max vs int.Max+1", 2147483647.0, 2147483648.0),
            ("int.Max+1 vs int.Max", 2147483648.0, 2147483647.0),
            ("NaN vs 0", double.NaN, 0.0),
            ("0 vs NaN", 0.0, double.NaN),
            ("NaN vs NaN", double.NaN, double.NaN),
            ("Inf vs 0", double.PositiveInfinity, 0.0),
            ("0 vs Inf", 0.0, double.PositiveInfinity),
            ("Inf vs Inf", double.PositiveInfinity, double.PositiveInfinity),
            ("Inf vs -Inf", double.PositiveInfinity, double.NegativeInfinity),
            ("Epsilon vs 0", double.Epsilon, 0.0),
            ("MaxValue == MaxValue", double.MaxValue, double.MaxValue),
        };

        foreach (bool strict in new[] { false, true })
        {
            Side.Quire.Strict(strict, () =>
            Side.Mathcad.Strict(strict, () =>
            {
                foreach (var (desc, a, b) in pairs)
                {
                    var aQ = Side.Quire.Num(a.r1, a.i1, a.d1);
                    var aM = Side.Mathcad.Num(a.r1, a.i1, a.d1);
                    var bQ = Side.Quire.Num(b.r2, b.i2, b.d2);
                    var bM = Side.Mathcad.Num(b.r2, b.i2, b.d2);

                    string p = $"{(strict ? "strict " : "")}{desc}";
                    MethodComparison.Compare($"{p} Equals", ContractNumVal, qEquals, aQ, [bQ], mEquals, aM, [bM], qs, ms, qState, mState);
                    MethodComparison.Compare($"{p} Equals1", ContractNumVal, qEquals1, aQ, [bQ], mEquals1, aM, [bM], qs, ms, qState, mState);
                    MethodComparison.Compare($"{p} nearly_equal 1e-9", ContractNumVal, qNearlyEqual, aQ, [bQ, 1e-9], mNearlyEqual, aM, [bM, 1e-9], qs, ms, qState, mState);
                    MethodComparison.Compare($"{p} ExactEquals", ContractNumVal, qExactEquals, null, [aQ, bQ], mExactEquals, null, [aM, bM], qs, ms);
                    MethodComparison.Compare($"{p} FuzzyEquals", ContractNumVal, qFuzzyEquals2, null, [aQ, bQ], mFuzzyEquals2, null, [aM, bM], qs, ms);
                    MethodComparison.Compare($"{p} FuzzyEquals 1e-11", ContractNumVal, qFuzzyEquals3, null, [aQ, bQ, 1e-11], mFuzzyEquals3, null, [aM, bM, 1e-11], qs, ms);

                    MethodComparison.Compare($"{p} LessThan", ContractNumVal, qLessThan, null, [aQ, bQ], mLessThan, null, [aM, bM], qs, ms);
                    MethodComparison.Compare($"{p} GreaterThan", ContractNumVal, qGreaterThan, null, [aQ, bQ], mGreaterThan, null, [aM, bM], qs, ms);
                    MethodComparison.Compare($"{p} LessEqThan", ContractNumVal, qLessEqThan, null, [aQ, bQ], mLessEqThan, null, [aM, bM], qs, ms);
                    MethodComparison.Compare($"{p} GreaterEqThan", ContractNumVal, qGreaterEqThan, null, [aQ, bQ], mGreaterEqThan, null, [aM, bM], qs, ms);
                }

                foreach (var (desc, x, y) in doublePairs)
                {
                    string p = $"{(strict ? "strict " : "")}{desc}";
                    MethodComparison.Compare($"{p} LT", ContractNumVal, qLT, null, [x, y], mLT, null, [x, y], qs, ms);
                    MethodComparison.Compare($"{p} GT", ContractNumVal, qGT, null, [x, y], mGT, null, [x, y], qs, ms);
                    MethodComparison.Compare($"{p} LE", ContractNumVal, qLE, null, [x, y], mLE, null, [x, y], qs, ms);
                    MethodComparison.Compare($"{p} GE", ContractNumVal, qGE, null, [x, y], mGE, null, [x, y], qs, ms);
                    MethodComparison.Compare($"{p} FuzzyEquals(d,d)", ContractNumVal, qFuzzyEqualsDbl2, null, [x, y], mFuzzyEqualsDbl2, null, [x, y], qs, ms);
                    MethodComparison.Compare($"{p} FuzzyEquals(d,d,1e-3)", ContractNumVal, qFuzzyEqualsDbl3, null, [x, y, 1e-3], mFuzzyEqualsDbl3, null, [x, y, 1e-3], qs, ms);
                }
                return 0;
            }));
        }

        var oneQ = Side.Quire.Num(1, 0, "identity");
        var oneM = Side.Mathcad.Num(1, 0, "identity");
        MethodComparison.Compare("Equals(null)", ContractNumVal, qEquals, oneQ, [null], mEquals, oneM, [null], qs, ms, qState, mState);
        MethodComparison.Compare("Equals(string)", ContractNumVal, qEquals, oneQ, ["1"], mEquals, oneM, ["1"], qs, ms, qState, mState);
        MethodComparison.Compare("Equals1(StrVal)", ContractNumVal, qEquals1, oneQ, [Side.Quire.Str("1")], mEquals1, oneM, [Side.Mathcad.Str("1")], qs, ms, qState, mState);
        MethodComparison.Compare("nearly_equal(StrVal)", ContractNumVal, qNearlyEqual, oneQ, [Side.Quire.Str("1"), 0.1], mNearlyEqual, oneM, [Side.Mathcad.Str("1"), 0.1], qs, ms, qState, mState);
    }

    [SkippableFact]
    public void Formatting()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = StateNumVal(Side.Quire);
        var mState = StateNumVal(Side.Mathcad);

        var (qToString0, mToString0) = Method(QNumVal, "ToString", DeclaredInstancePublic);
        var (qToStringStr, mToStringStr) = Method(QNumVal, "ToString", DeclaredInstancePublic, typeof(string));
        var (qToStringCult, mToStringCult) = Method(QNumVal, "ToString", DeclaredInstancePublic, typeof(IFormatProvider));
        var (qToStringCultStr, mToStringCultStr) = Method(QNumVal, "ToString", DeclaredInstancePublic, typeof(IFormatProvider), typeof(string));

        var (qFormatBool1, mFormatBool1) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(bool));
        var (qFormatBool2, mFormatBool2) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(bool), typeof(bool));
        var (qFormatStrBool, mFormatStrBool) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(string), typeof(bool));
        var (qFormatStrBoolBool, mFormatStrBoolBool) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(string), typeof(bool), typeof(bool));
        var (qFormatCultStrBoolBool, mFormatCultStrBoolBool) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(IFormatProvider), typeof(string), typeof(bool), typeof(bool));

        var values = SampleNumbers().Concat(new (string, object, object)[]
        {
            ("1500 v", Side.Quire.Num(1500, 0, "v"), Side.Mathcad.Num(1500, 0, "v")),
            ("2 N", Side.Quire.Num(2, -3, "N"), Side.Mathcad.Num(2, -3, "N")),
            ("1e-3 L2", Side.Quire.Num(1e-3, 0, "L2"), Side.Mathcad.Num(1e-3, 0, "L2")),
            ("-0.5i", Side.Quire.Num(0, -0.5, "identity"), Side.Mathcad.Num(0, -0.5, "identity")),
            ("1/3", Side.Quire.Num(1.0 / 3, 2.0 / 3, "identity"), Side.Mathcad.Num(1.0 / 3, 2.0 / 3, "identity")),
            ("-0 -0", Side.Quire.Num(-0.0, -0.0, "identity"), Side.Mathcad.Num(-0.0, -0.0, "identity")),
            ("NaN i", Side.Quire.Num(1, double.NaN, "identity"), Side.Mathcad.Num(1, double.NaN, "identity")),
        });

        foreach (var (n, vQ, vM) in values)
        {
            MethodComparison.Compare($"{n}.ToString()", ContractNumVal, qToString0, vQ, [], mToString0, vM, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.ToString(F2)", ContractNumVal, qToStringStr, vQ, ["F2"], mToStringStr, vM, ["F2"], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.Format(true)", ContractNumVal, qFormatBool1, vQ, [true], mFormatBool1, vM, [true], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.Format(true, true)", ContractNumVal, qFormatBool2, vQ, [true, true], mFormatBool2, vM, [true, true], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.Format(E3, true)", ContractNumVal, qFormatStrBool, vQ, ["E3", true], mFormatStrBool, vM, ["E3", true], qs, ms, qState, mState);
            MethodComparison.Compare($"{n}.Format(G4, false, true)", ContractNumVal, qFormatStrBoolBool, vQ, ["G4", false, true], mFormatStrBoolBool, vM, ["G4", false, true], qs, ms, qState, mState);

            foreach (var culture in new[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de-DE") })
            {
                MethodComparison.Compare($"{n}.ToString({culture.Name})", ContractNumVal, qToStringCult, vQ, [culture], mToStringCult, vM, [culture], qs, ms, qState, mState);
                MethodComparison.Compare($"{n}.ToString({culture.Name}, N3)", ContractNumVal, qToStringCultStr, vQ, [culture, "N3"], mToStringCultStr, vM, [culture, "N3"], qs, ms, qState, mState);
                MethodComparison.Compare($"{n}.Format({culture.Name}, g15, true, false)", ContractNumVal, qFormatCultStrBoolBool, vQ, [culture, "g15", true, false], mFormatCultStrBoolBool, vM, [culture, "g15", true, false], qs, ms, qState, mState);
            }
            MethodComparison.Compare($"{n}.Format(bad format)", ContractNumVal, qFormatStrBool, vQ, ["{", false], mFormatStrBool, vM, ["{", false], qs, ms, qState, mState);
        }
    }

    [SkippableFact]
    public void Strings_and_no_value()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qStateStr = StateStrVal(Side.Quire);
        var mStateStr = StateStrVal(Side.Mathcad);
        var qStateNotAVal = StateNotAVal(Side.Quire);
        var mStateNotAVal = StateNotAVal(Side.Mathcad);

        // --- StrVal members ---
        var (qStrCtor, mStrCtor) = Ctor(QStrVal, typeof(string));
        var (qGetIsString, mGetIsString) = Method(QStrVal, "get_IsString", DeclaredInstancePublic);
        var (qGetMString, mGetMString) = Method(QStrVal, "get_m_string", DeclaredInstancePublic);
        var (qAsString, mAsString) = Method(QStrVal, "asString", DeclaredInstancePublic, typeof(string), typeof(uint));
        var (qStrToString, mStrToString) = Method(QStrVal, "ToString", DeclaredInstancePublic);
        var (qStrEquals, mStrEquals) = Method(QStrVal, "Equals", DeclaredInstancePublic, typeof(object));
        var (qStrGetHashCode, mStrGetHashCode) = Method(QStrVal, "GetHashCode", DeclaredInstancePublic);

        foreach (var text in new[] { "abc", "", "a\tb", null })
        {
            MethodComparison.Compare($"StrVal({Text(text)})", ContractStrVal, qStrCtor, null, [text], mStrCtor, null, [text], qs, ms);

            var vQ = Side.Quire.Str(text);
            var vM = Side.Mathcad.Str(text);

            MethodComparison.Compare($"StrVal({Text(text)}).IsString", ContractStrVal, qGetIsString, vQ, [], mGetIsString, vM, [], qs, ms, qStateStr, mStateStr);
            MethodComparison.Compare($"StrVal({Text(text)}).m_string", ContractStrVal, qGetMString, vQ, [], mGetMString, vM, [], qs, ms, qStateStr, mStateStr);
            MethodComparison.Compare($"StrVal({Text(text)}).asString(m, 2)", ContractStrVal, qAsString, vQ, ["key", 2u], mAsString, vM, ["key", 2u], qs, ms, qStateStr, mStateStr);
            MethodComparison.Compare($"StrVal({Text(text)}).ToString", ContractStrVal, qStrToString, vQ, [], mStrToString, vM, [], qs, ms, qStateStr, mStateStr);
            if (text != null)
            {
                MethodComparison.Compare($"StrVal({Text(text)}).GetHashCode", ContractStrVal, qStrGetHashCode, vQ, [], mStrGetHashCode, vM, [], qs, ms, qStateStr, mStateStr);
            }

            foreach (var other in new object?[] { "abc", null })
            {
                var oQ = other is string s ? Side.Quire.Str(s) : null;
                var oM = other is string sm ? Side.Mathcad.Str(sm) : null;
                MethodComparison.Compare($"StrVal({Text(text)}).Equals(StrVal {Text(other)})", ContractStrVal, qStrEquals, vQ, [oQ], mStrEquals, vM, [oM], qs, ms, qStateStr, mStateStr);
                MethodComparison.Compare($"StrVal({Text(text)}).Equals(obj {Text(other)})", ContractStrVal, qStrEquals, vQ, [other], mStrEquals, vM, [other], qs, ms, qStateStr, mStateStr);
            }
        }

        // --- NotAVal.ToString() ---
        var (qNotAValToString, mNotAValToString) = Method(QNotAVal, "ToString", DeclaredInstancePublic);
        var naQ = Side.Quire.NotAVal();
        var naM = Side.Mathcad.NotAVal();
        MethodComparison.Compare("NotAVal.ToString()", ContractNotAVal, qNotAValToString, naQ, [], mNotAValToString, naM, [], qs, ms, qStateNotAVal, mStateNotAVal);
    }

    private static string Statics(Side side) => string.Join(" ", new[] { "Zero", "ZeroId", "ZeroPoly", "One", "mcNaN", "NoValue", "Inf", "McdNaN", "NaN", "NoNoNaN" }
        .Select(f => $"{f}={side.Show(Get(side.T("Mcd.NumVal"), f))}"));

    /// <summary>A unit system whose length unit is the foot.</summary>
    private static object Feet(Side side)
    {
        var standard = Get(side.T("Mcd.RuntimeOptions"), "s_default_unit_system")!;
        var basis = (Array)((Array)Get(standard, "Basis")!).Clone();
        for (int i = 0; i < basis.Length; i++)
            if ((string?)Get(basis.GetValue(i)!, "Quantity") == "Length")
                basis.SetValue(New(side.T("Mcd.Unit"), "foot", "ft", "Length", 0.3048, "V{Length 60000}"), i);
        return New(side.T("Mcd.UnitSystem"), "feet", 7u, "guid", basis, Get(standard, "Defaults"));
    }

    /// <summary>The statics the type initializer sets, NaN payload bits, whether <c>asResult</c> returns the number
    /// itself, fuzzy comparison of complex parts, and formatting in units whose magnitude isn't 1.</summary>
    [SkippableFact]
    public void Statics_payloads_identity_and_units()
    {
        Runtime.Sync();
        Func<object?, string> qs = Side.Quire.Show;
        Func<object?, string> ms = Side.Mathcad.Show;

        // First, so that a failing type initializer is seen inside a comparison.
        var (qLoadNaN, mLoadNaN) = Method(QNumVal, "LoadNaN", DeclaredStaticPublic, typeof(int));
        foreach (var payload in new[] { 0, 0x12345678 })
            MethodComparison.Compare($"LoadNaN({payload}) bits", ContractNumVal, qLoadNaN, null, [payload], mLoadNaN, null, [payload], qs, ms);

        MethodComparison.Compare("NumVal.cctor: statics", ContractNumVal, QNumVal.TypeInitializer!, null, [], MNumVal.TypeInitializer!, null, [],
            _ => Statics(Side.Quire), _ => Statics(Side.Mathcad));

        var (qAsResult, mAsResult) = Method(QNumVal, "asResult", DeclaredInstancePublic);
        foreach (var dim in new[] { "identity", "L" })
        {
            var vQ = Side.Quire.Num(2.0, 0.0, dim);
            var vM = Side.Mathcad.Num(2.0, 0.0, dim);
            MethodComparison.Compare($"NumVal(2, {dim}).asResult is itself", ContractNumVal, qAsResult, vQ, [], mAsResult, vM, [],
                r => ReferenceEquals(r, vQ) ? "itself" : qs(r), r => ReferenceEquals(r, vM) ? "itself" : ms(r), qs, ms);
        }

        var (qFuzzyEquals3, mFuzzyEquals3) = Method(QNumVal, "FuzzyEquals", DeclaredStaticPublic, QNumVal, QNumVal, typeof(double));
        foreach (var (x, y, fuzz) in new[] { ((1.0, 1.0), (1.0, 0.0), 1e-11), ((1.0, 2.0), (1.0, 1.0), 1e-11), ((1.0, 1.0), (1.0, 1.01), 0.0045) })
        {
            MethodComparison.Compare($"FuzzyEquals({x}, {y}, {fuzz})", ContractNumVal,
                qFuzzyEquals3, null, [Side.Quire.Num(x.Item1, x.Item2, "identity"), Side.Quire.Num(y.Item1, y.Item2, "identity"), fuzz],
                mFuzzyEquals3, null, [Side.Mathcad.Num(x.Item1, x.Item2, "identity"), Side.Mathcad.Num(y.Item1, y.Item2, "identity"), fuzz], qs, ms);
        }

        var (qFormat, mFormat) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(IFormatProvider), typeof(string), typeof(bool), typeof(bool));
        var qNumber = Side.Quire.Num(3.048, 0.6096, "L");
        var mNumber = Side.Mathcad.Num(3.048, 0.6096, "L");
        var qOld = Get(Side.Quire.T("Mcd.RuntimeOptions"), "the_unit_system");
        var mOld = Get(Side.Mathcad.T("Mcd.RuntimeOptions"), "the_unit_system");
        Set(Side.Quire.T("Mcd.RuntimeOptions"), "the_unit_system", Feet(Side.Quire));
        Set(Side.Mathcad.T("Mcd.RuntimeOptions"), "the_unit_system", Feet(Side.Mathcad));
        try
        {
            MethodComparison.Compare("Format(g15, units) in feet", ContractNumVal,
                qFormat, qNumber, [CultureInfo.InvariantCulture, "g15", true, false], mFormat, mNumber, [CultureInfo.InvariantCulture, "g15", true, false],
                qs, ms, StateNumVal(Side.Quire), StateNumVal(Side.Mathcad));
        }
        finally
        {
            Set(Side.Quire.T("Mcd.RuntimeOptions"), "the_unit_system", qOld);
            Set(Side.Mathcad.T("Mcd.RuntimeOptions"), "the_unit_system", mOld);
        }
    }
}
