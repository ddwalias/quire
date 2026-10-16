using System.Globalization;
using System.Reflection;
using static Quire.Differential.Tests.Reflect;
using Xunit;
using Q = Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Differential tests covering sensitive caller behavior in DimVal, NumVal, and RngVal.
/// Kept in a dedicated test class to avoid invalidating the broader DimVal/RngVal test classes
/// while maintaining complete mutant kill coverage for cross-lane callers.
/// </summary>
public class CallerSensitivityDifferentialTests
{
    private const string ContractDimVal = "Mcd.DimVal";
    private const string ContractNumVal = "Mcd.NumVal";
    private const string ContractRngVal = "Mcd.RngVal";

    private const BindingFlags DeclaredPublic = BindingFlags.Public | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstancePublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredInstanceNonPublic = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private static System.Type MDimVal => Mathcad.Type("mplrun", "Mcd.DimVal");
    private static System.Type QDimVal => typeof(global::Quire.Mcd.DimVal);
    private static System.Type MNumVal => Mathcad.Type("mplrun", "Mcd.NumVal");
    private static System.Type QNumVal => typeof(global::Quire.Mcd.NumVal);
    private static System.Type MRngVal => Mathcad.Type("mplrun", "Mcd.RngVal");
    private static System.Type QRngVal => typeof(global::Quire.Mcd.RngVal);
    private static System.Type MDim => Mathcad.Type("mplrun", "Mcd.Dim");
    private static System.Type QDim => typeof(global::Quire.Mcd.Dim);
    private static System.Type MRuntimeOptions => Mathcad.Type("mplrun", "Mcd.RuntimeOptions");
    private static System.Type MUnit => Mathcad.Type("mplrun", "Mcd.Unit");
    private static System.Type MUnitSystem => Mathcad.Type("mplrun", "Mcd.UnitSystem");

    private static System.Type MapToM(System.Type t)
    {
        if (t == QDimVal) return MDimVal;
        if (t == QNumVal) return MNumVal;
        if (t == QRngVal) return MRngVal;
        if (t == QDim) return MDim;
        if (t == typeof(global::Quire.Mcd.MatVal)) return Mathcad.Type("mplrun", "Mcd.MatVal");
        if (t == typeof(global::Quire.Mcd.MatDim)) return Mathcad.Type("mplrun", "Mcd.MatDim");
        if (t == typeof(global::Quire.Mcd.Units)) return Mathcad.Type("mplrun", "Mcd.Units");
        return t;
    }

    private static (MethodInfo q, MethodInfo m) Method(System.Type qType, string name, BindingFlags flags, params System.Type[] qParamTypes)
    {
        var mType = MapToM(qType);
        var mParamTypes = qParamTypes.Select(MapToM).ToArray();
        var q = qType.GetMethod(name, flags, qParamTypes)
            ?? throw new HarnessException($"Quire method not found on {qType.Name}: {name}({string.Join(", ", qParamTypes.Select(t => t.Name))})");
        var m = mType.GetMethod(name, flags, mParamTypes)
            ?? throw new HarnessException($"Mathcad method not found on {mType.Name}: {name}({string.Join(", ", mParamTypes.Select(t => t.Name))})");
        return (q, m);
    }

    private static (ConstructorInfo q, ConstructorInfo m) Ctor(System.Type qType, params System.Type[] qParamTypes)
    {
        var mType = MapToM(qType);
        var mParamTypes = qParamTypes.Select(MapToM).ToArray();
        var q = qType.GetConstructor(DeclaredInstancePublic, null, qParamTypes, null)
            ?? throw new HarnessException($"Quire constructor not found: {qType.Name}({string.Join(", ", qParamTypes.Select(t => t.Name))})");
        var m = mType.GetConstructor(DeclaredInstancePublic, null, mParamTypes, null)
            ?? throw new HarnessException($"Mathcad constructor not found: {mType.Name}({string.Join(", ", mParamTypes.Select(t => t.Name))})");
        return (q, m);
    }

    private static (object q, object m) MakeRange(double init, double incr, int n, string dim = "identity")
    {
        var qDim = Side.Quire.D(dim);
        var mDim = Side.Mathcad.D(dim);
        var qCtor = QRngVal.GetConstructor(DeclaredInstanceNonPublic, null, [typeof(double), typeof(double), typeof(int), QDim], null)!;
        var mCtor = MRngVal.GetConstructor(DeclaredInstanceNonPublic, null, [typeof(double), typeof(double), typeof(int), MDim], null)!;
        return (qCtor.Invoke([init, incr, n, qDim])!, mCtor.Invoke([init, incr, n, mDim])!);
    }

    private static string ShowVal(Side side, object? v)
    {
        if (v == null) return "null";
        if (v is Pointer) return "ptr";
        if (v is string or int or uint or double or bool or long or byte or short) return Text(v);
        if (v is int[] a) return $"[{string.Join(",", a)}]";
        if (v is object[] arr) return $"[{string.Join(", ", arr.Select(x => ShowVal(side, x)))}]";
        if (v.GetType().Name == "Units") return Text(v);
        if (v.GetType().Name == "MatVal" || v.GetType().FullName!.Contains("MatVal")) return MatValDifferentialTests.Storage(side, v);
        if (v.GetType().Name == "MatDim" || v.GetType().FullName!.Contains("MatDim")) return $"MatDim[{Get(v, "Rows")}x{Get(v, "Cols")}]";
        if (v.GetType().Name == "Dim") return side.DimText(v);
        if (v.GetType().Name is "NotAVal" or "StrVal" or "NumVal" || v.GetType().IsSubclassOf(side.T("Mcd.DimVal")))
        {
            return $"{v.GetType().Name}[dim={side.DimText(Get(v, "m_dim"))}]";
        }
        return Text(v);
    }

    private static Func<object?, string> Show(Side side) => v => ShowVal(side, v);
    private static Func<object, string> State(Side side) => _ => "";

    [SkippableFact]
    public void DimVal_IsDimlessReal_and_asDimlessReal()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var (qGetIsDimlessReal, mGetIsDimlessReal) = Method(QDimVal, "get_IsDimlessReal", DeclaredInstancePublic);
        var (qAsDimlessReal1, mAsDimlessReal1) = Method(QDimVal, "asDimlessReal", DeclaredInstancePublic, typeof(uint));
        var (qAsDimlessReal2, mAsDimlessReal2) = Method(QDimVal, "asDimlessReal", DeclaredInstancePublic, typeof(string), typeof(uint));

        var targets = new (string label, object q, object m)[]
        {
            ("NumVal_dimless", Side.Quire.Num(42.0, 0.0, "identity"), Side.Mathcad.Num(42.0, 0.0, "identity")),
            ("NumVal_dim", Side.Quire.Num(42.0, 0.0, "L"), Side.Mathcad.Num(42.0, 0.0, "L")),
            ("NumVal_complex", Side.Quire.Num(1.0, 2.0, "identity"), Side.Mathcad.Num(1.0, 2.0, "identity")),
            ("StrVal_dimless", Side.Quire.Str("hello"), Side.Mathcad.Str("hello")),
        };

        foreach (var (label, qT, mT) in targets)
        {
            MethodComparison.Compare($"{label}.IsDimlessReal", ContractDimVal,
                qGetIsDimlessReal, qT, [], mGetIsDimlessReal, mT, [], qs, ms, qState, mState);
            MethodComparison.Compare($"{label}.asDimlessReal(5)", ContractDimVal,
                qAsDimlessReal1, qT, [5u], mAsDimlessReal1, mT, [5u], qs, ms, qState, mState);
            MethodComparison.Compare($"{label}.asDimlessReal(m, 5)", ContractDimVal,
                qAsDimlessReal2, qT, ["custom key", 5u], mAsDimlessReal2, mT, ["custom key", 5u], qs, ms, qState, mState);
        }
    }

    [SkippableFact]
    public void DimVal_matrix_set0()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var (qMatrixSet0_1, mMatrixSet0_1) = Method(QDimVal, "matrix_set0_1", DeclaredInstancePublic, typeof(int));
        var (qMatrixSet0_2, mMatrixSet0_2) = Method(QDimVal, "matrix_set0_2", DeclaredInstancePublic, typeof(int), typeof(int));

        var qNotAVal = Side.Quire.NotAVal();
        var mNotAVal = Side.Mathcad.NotAVal();

        MethodComparison.Compare("NotAVal.matrix_set0_1(2)", ContractDimVal,
            qMatrixSet0_1, qNotAVal, [2], mMatrixSet0_1, mNotAVal, [2], qs, ms, qState, mState);
        MethodComparison.Compare("NotAVal.matrix_set0_2(1, 2)", ContractDimVal,
            qMatrixSet0_2, qNotAVal, [1, 2], mMatrixSet0_2, mNotAVal, [1, 2], qs, ms, qState, mState);
    }

    [SkippableFact]
    public void RngVal_asRealVector_nonzero_and_GetHashCode()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var (qAsRealVector, mAsRealVector) = Method(QRngVal, "asRealVector", DeclaredInstancePublic, typeof(string), typeof(uint));
        var (qGetHashCode, mGetHashCode) = Method(QRngVal, "GetHashCode", DeclaredInstancePublic);

        var testRanges = new (string label, double init, double incr, int n, string dim)[]
        {
            ("range_2_5_1_3_L", 2.5, 1.0, 3, "L"),
            ("range_0_0_5_10_id", 0.0, 0.5, 10, "identity"),
            ("range_neg3_1_5_4_T", -3.0, 1.5, 4, "T"),
        };

        foreach (var (label, init, incr, n, dim) in testRanges)
        {
            var (qRng, mRng) = MakeRange(init, incr, n, dim);

            MethodComparison.Compare($"{label}.asRealVector('m', 4)", ContractRngVal,
                qAsRealVector, qRng, ["m", 4u], mAsRealVector, mRng, ["m", 4u], qs, ms, qState, mState);
            MethodComparison.Compare($"{label}.GetHashCode", ContractRngVal,
                qGetHashCode, qRng, [], mGetHashCode, mRng, [], qs, ms, qState, mState);
        }
    }

    [SkippableFact]
    public void NumVal_Format_non_unit_magnitude_and_asResult_polymorphic()
    {
        Runtime.Sync();
        var qs = Show(Side.Quire);
        var ms = Show(Side.Mathcad);
        var qState = State(Side.Quire);
        var mState = State(Side.Mathcad);

        var (qFormatBool1, mFormatBool1) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(bool));
        var (qFormatBool2, mFormatBool2) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(bool), typeof(bool));
        var (qFormatStrBool, mFormatStrBool) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(string), typeof(bool));
        var (qFormatStrBoolBool, mFormatStrBoolBool) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(string), typeof(bool), typeof(bool));
        var (qFormat4, mFormat4) = Method(QNumVal, "Format", DeclaredInstancePublic, typeof(IFormatProvider), typeof(string), typeof(bool), typeof(bool));

        // Create a US unit system with foot (magnitude 0.3048) for Length
        var qFoot = new Q.Unit("foot", "ft", "Length", 0.3048, "V{Length 60000}");
        var mFoot = New(MUnit, "foot", "ft", "Length", 0.3048, "V{Length 60000}");

        var qBasis = new Q.Unit[]
        {
            new("pound", "lb", "Mass", 0.45359237, "V{Mass 60000}"),
            qFoot,
            new("second", "s", "Time", 1, "V{Time 60000}"),
            new("ampere", "A", "Current", 1, "V{Current 60000}"),
            new("rankine", "R", "Temperature", 5.0 / 9, "V{Temperature 60000}"),
            new("mole", "mol", "Substance", 1, "V{Substance 60000}"),
            new("candela", "cd", "Luminosity", 1, "V{Luminosity 60000}"),
            new("dollar", "$", "Money", 1, "V{Money 60000}"),
        };
        var mBasis = Array.CreateInstance(MUnit, qBasis.Length);
        mBasis.SetValue(New(MUnit, "pound", "lb", "Mass", 0.45359237, "V{Mass 60000}"), 0);
        mBasis.SetValue(mFoot, 1);
        mBasis.SetValue(New(MUnit, "second", "s", "Time", 1.0, "V{Time 60000}"), 2);
        mBasis.SetValue(New(MUnit, "ampere", "A", "Current", 1.0, "V{Current 60000}"), 3);
        mBasis.SetValue(New(MUnit, "rankine", "R", "Temperature", 5.0 / 9.0, "V{Temperature 60000}"), 4);
        mBasis.SetValue(New(MUnit, "mole", "mol", "Substance", 1.0, "V{Substance 60000}"), 5);
        mBasis.SetValue(New(MUnit, "candela", "cd", "Luminosity", 1.0, "V{Luminosity 60000}"), 6);
        mBasis.SetValue(New(MUnit, "dollar", "$", "Money", 1.0, "V{Money 60000}"), 7);

        var qSys = new Q.UnitSystem("US", 0u, "7F1921F3-0A1A-11E0-8A9E-11F8E2A8C36C", qBasis, []);
        var mSys = New(MUnitSystem, "US", 0u, "7F1921F3-0A1A-11E0-8A9E-11F8E2A8C36C", mBasis, Array.CreateInstance(MUnit, 0));

        var qOldSys = Q.RuntimeOptions.the_unit_system;
        var mOldSys = Get(MRuntimeOptions, "the_unit_system");

        try
        {
            Q.RuntimeOptions.the_unit_system = qSys;
            Set(MRuntimeOptions, "the_unit_system", mSys);

            var testNumbers = new (string label, double re, double im, string dim)[]
            {
                ("100_50_L", 100.0, 50.0, "L"),
                ("5_neg2_L", 5.0, -2.5, "L"),
                ("12_0_L", 12.0, 0.0, "L"),
            };

            foreach (var (label, re, im, dim) in testNumbers)
            {
                var qNum = Side.Quire.Num(re, im, dim);
                var mNum = Side.Mathcad.Num(re, im, dim);

                MethodComparison.Compare($"{label}.Format(true)", ContractNumVal,
                    qFormatBool1, qNum, [true], mFormatBool1, mNum, [true], qs, ms, qState, mState);
                MethodComparison.Compare($"{label}.Format(true, true)", ContractNumVal,
                    qFormatBool2, qNum, [true, true], mFormatBool2, mNum, [true, true], qs, ms, qState, mState);
                MethodComparison.Compare($"{label}.Format(g15, true)", ContractNumVal,
                    qFormatStrBool, qNum, ["g15", true], mFormatStrBool, mNum, ["g15", true], qs, ms, qState, mState);
                MethodComparison.Compare($"{label}.Format(g15, true, true)", ContractNumVal,
                    qFormatStrBoolBool, qNum, ["g15", true, true], mFormatStrBoolBool, mNum, ["g15", true, true], qs, ms, qState, mState);
                MethodComparison.Compare($"{label}.Format(Inv, g15, true, false)", ContractNumVal,
                    qFormat4, qNum, [CultureInfo.InvariantCulture, "g15", true, false], mFormat4, mNum, [CultureInfo.InvariantCulture, "g15", true, false], qs, ms, qState, mState);
                MethodComparison.Compare($"{label}.Format(Inv, g15, true, true)", ContractNumVal,
                    qFormat4, qNum, [CultureInfo.InvariantCulture, "g15", true, true], mFormat4, mNum, [CultureInfo.InvariantCulture, "g15", true, true], qs, ms, qState, mState);
            }
        }
        finally
        {
            Q.RuntimeOptions.the_unit_system = qOldSys;
            Set(MRuntimeOptions, "the_unit_system", mOldSys);
        }

        // Polymorphic NumVal with null m_dim: tests asResult() polymorphic path
        var (qCtorDblDblDim, mCtorDblDblDim) = Ctor(QNumVal, typeof(double), typeof(double), QDim);
        var (qAsResult, mAsResult) = Method(QNumVal, "asResult", DeclaredInstancePublic);

        var qPolyNum = qCtorDblDblDim.Invoke([7.5, -1.0, null]);
        var mPolyNum = mCtorDblDblDim.Invoke([7.5, -1.0, null]);

        MethodComparison.Compare("NumVal(7.5, -1.0, null).asResult", ContractNumVal,
            qAsResult, qPolyNum, [], mAsResult, mPolyNum, [], qs, ms, qState, mState);

        // Non-null m_dim: tests asResult() identity preservation (kills cgt.un -> clt.un)
        var qNormNum = Side.Quire.Num(7.5, -1.0, "L");
        var mNormNum = Side.Mathcad.Num(7.5, -1.0, "L");
        Func<object?, string> qShowSame = res => ReferenceEquals(res, qNormNum) ? "SAME_INSTANCE" : qs(res);
        Func<object?, string> mShowSame = res => ReferenceEquals(res, mNormNum) ? "SAME_INSTANCE" : ms(res);

        MethodComparison.Compare("NumVal(7.5, -1.0, L).asResult returns this", ContractNumVal,
            qAsResult, qNormNum, [], mAsResult, mNormNum, [], qShowSame, mShowSame, qState, mState);
    }
}
