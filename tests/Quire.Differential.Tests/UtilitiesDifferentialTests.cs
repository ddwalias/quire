using System.Reflection;
using static Quire.Differential.Tests.Reflect;
using Q = Quire.Mcd;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.Utilities</c> with Quire's using direct <see cref="MethodComparison.Compare"/>
/// calls across all branches and edge cases; cases are listed in docs/mirror/evidence/Mcd.Utilities.md.
/// </summary>
public class UtilitiesDifferentialTests
{
    private const string Contract = "Mcd.Utilities";

    private static System.Type QUtilities => typeof(Q.Utilities);
    private static System.Type MUtilities => Mathcad.Type("mplrun", "Mcd.Utilities");
    private static System.Type QTypeProduct => typeof(Q.Type.Product);
    private static System.Type MTypeProduct => Mathcad.Type("mplrun", "Mcd.Type+Product");

    private static readonly double[] Doubles =
    [
        0.0, -0.0, 1.0, -1.0, 1e308, -1e308, double.MaxValue, double.Epsilon, double.NaN, double.PositiveInfinity, double.NegativeInfinity,
        BitConverter.Int64BitsToDouble(0x7FF0000000000001), BitConverter.Int64BitsToDouble(unchecked((long)0xFFF8000000000000)),
        BitConverter.Int64BitsToDouble(0x7FF0000100000000L), BitConverter.Int64BitsToDouble(unchecked((long)0xFFF0000100000000L)),
        BitConverter.Int64BitsToDouble(0x7FF0000100000001L), BitConverter.Int64BitsToDouble(unchecked((long)0xFFF0000100000001L)),
    ];

    private static string ShowVal(Side side, object? v)
    {
        try
        {
            if (v == null) return "null";
            if (v is double[] arr)
            {
                var items = new List<string>();
                int len;
                try { len = arr.Length; } catch { return "[corrupt-array-len]"; }
                for (int i = 0; i < Math.Min(len, 20); i++)
                {
                    try { items.Add(arr[i].ToString("R")); }
                    catch (Exception ex) { items.Add($"[elem-err: {ex.GetType().Name}]"); break; }
                }
                return "[" + string.Join(", ", items) + "]";
            }
            if (v is string or int or uint or double or bool or long or byte or short) return Text(v);
            var typeName = v.GetType().Name;
            if (typeName == "Utilities") return "Utilities";
            if (typeName == "MatVal")
            {
                return $"MatVal[{Get(v, "Rows")}x{Get(v, "Cols")}, real={side.Show(Get(v, "m_rarr"))}, imag={side.Show(Get(v, "m_iarr"))}]";
            }
            return side.Show(v);
        }
        catch (Exception ex)
        {
            return $"[err: {ex.GetType().Name}]";
        }
    }

    private static void Compare(string caseId, MethodBase quire, object?[] quireArgs,
        MethodBase original, object?[] originalArgs)
    {
        MethodComparison.Compare(caseId, Contract, quire, null, quireArgs, original, null, originalArgs,
            v => ShowVal(Side.Quire, v), v => ShowVal(Side.Mathcad, v));
    }

    private static (MethodInfo q, MethodInfo m) Method(string name, params System.Type[] qParamTypes)
    {
        var mParamTypes = qParamTypes.Select(t =>
            t == QUtilities ? MUtilities :
            t == QTypeProduct ? MTypeProduct :
            t == typeof(Q.num_val) ? Mathcad.Type("mplrun", "Mcd.num_val") :
            t).ToArray();
        var q = QUtilities.GetMethod(name, qParamTypes)
            ?? throw new HarnessException($"Quire method not found: Utilities.{name}");
        var m = MUtilities.GetMethod(name, mParamTypes)
            ?? throw new HarnessException($"Mathcad method not found: Utilities.{name}");
        return (q, m);
    }

    private static (ConstructorInfo q, ConstructorInfo m) Ctor()
    {
        var q = QUtilities.GetConstructor(System.Type.EmptyTypes)
            ?? throw new HarnessException("Quire constructor not found");
        var m = MUtilities.GetConstructor(System.Type.EmptyTypes)
            ?? throw new HarnessException("Mathcad constructor not found");
        return (q, m);
    }

    [SkippableFact]
    public void Construction_and_scaling()
    {
        // 1. .ctor()
        var (qCtor, mCtor) = Ctor();
        Compare("new Utilities()", qCtor, [], mCtor, []);

        // 2. ScaleVector(double[], double)
        var (qScale, mScale) = Method("ScaleVector", typeof(double[]), typeof(double));
        double[][] vectors = [[1.0, 2.0, 3.0], [1.0, 2.0], [], [double.NaN, 0.0], [-4.0]];
        foreach (var vec in vectors)
        {
            foreach (double s in new[] { 2.0, 0.0, -0.5, double.NaN })
            {
                var qa = (double[])vec.Clone();
                var ma = (double[])vec.Clone();
                Compare($"ScaleVector([{string.Join(",", vec)}], {s:R})", qScale, [qa, s], mScale, [ma, s]);
            }
        }
        Compare("ScaleVector(null, 2.0)", qScale, [null, 2.0], mScale, [null, 2.0]);
    }

    [SkippableFact]
    public void Finite_checks_and_units()
    {
        Runtime.Sync();

        // 1. IsFinite(double)
        var (qIsFinite, mIsFinite) = Method("IsFinite", typeof(double));
        foreach (double d in Doubles)
        {
            Compare($"IsFinite({d:R})", qIsFinite, [d], mIsFinite, [d]);
        }

        // 2. AssertFinite(double)
        var (qAssertFiniteDbl, mAssertFiniteDbl) = Method("AssertFinite", typeof(double));
        foreach (double d in Doubles)
        {
            Compare($"AssertFinite({d:R})", qAssertFiniteDbl, [d], mAssertFiniteDbl, [d]);
        }

        // 3. AssertFinite(num_val)
        var (qAssertFiniteNum, mAssertFiniteNum) = Method("AssertFinite", typeof(Q.num_val));
        var mNumValType = Mathcad.Type("mplrun", "Mcd.num_val");
        foreach (double d in Doubles)
        {
            foreach (double e in new[] { 0.0, double.NaN, double.PositiveInfinity })
            {
                var qv = new Q.num_val(d, e);
                var mv = Activator.CreateInstance(mNumValType, d, e)!;
                Compare($"AssertFinite(num_val {d:R}, {e:R})", qAssertFiniteNum, [qv], mAssertFiniteNum, [mv]);
            }
        }

        // 4. GetUnits(Type.Product)
        var (qGetUnits, mGetUnits) = Method("GetUnits", QTypeProduct);
        var mType = Mathcad.Type("mplrun", "Mcd.Type");
        foreach (var s in new[] { "V{Length 60000}", "V{}", "V{a Length 60000}", "V{Length 30000 Time-60000}", "V{Foo 60000}", "V{Money 60000 Mass-120000}" })
        {
            object? qProd = null;
            object? mProd = null;
            try
            {
                qProd = Q.Type.Parse(s).GetDimensionTypeProduct();
                mProd = Call(Call(mType, "Parse", s)!, "GetDimensionTypeProduct");
            }
            catch
            {
                // Unparseable dimension signatures throw during parse, not inside GetUnits
            }
            if (qProd != null && mProd != null)
            {
                Compare($"GetUnits({s})", qGetUnits, [qProd], mGetUnits, [mProd]);
            }
        }
        Compare("GetUnits(null)", qGetUnits, [null], mGetUnits, [null]);
    }

    [SkippableFact]
    public void Divide_and_multiply()
    {
        var (qDiv, mDiv) = Method("Divide", typeof(object), typeof(double));
        var (qMul, mMul) = Method("Multiply", typeof(object), typeof(double));

        var qNum = Side.Quire.Num(4.0, 2.0, "identity");
        var mNum = Side.Mathcad.Num(4.0, 2.0, "identity");

        var qMat = New(Side.Quire.T("Mcd.MatVal"), [new double[] { 4.0, 6.0 }, null, 2, 1]);
        var mMat = New(Side.Mathcad.T("Mcd.MatVal"), [new double[] { 4.0, 6.0 }, null, 2, 1]);

        var testValues = new (object? qVal, object? mVal, string desc)[]
        {
            (qNum, mNum, "NumVal"),
            (qMat, mMat, "MatVal"),
            ("hello", "hello", "string"),
            (10.0, 10.0, "double"),
            (null, null, "null")
        };

        foreach (double s in new[] { 1.0, 0.0, 2.0, -0.5, double.NaN })
        {
            foreach (var (qv, mv, desc) in testValues)
            {
                Compare($"Divide({desc}, {s:R})", qDiv, [qv, s], mDiv, [mv, s]);
                Compare($"Multiply({desc}, {s:R})", qMul, [qv, s], mMul, [mv, s]);
            }
        }
    }
}
