using System.Reflection;
using Quire.Mcd;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

/// <summary>
/// Compares mplrun's <c>Mcd.Rational</c> with Quire's using direct <see cref="MethodComparison.Compare"/>
/// calls across all branches and edge cases; cases are listed in docs/mirror/evidence/Mcd.Rational.md.
/// </summary>
public class RationalDifferentialTests
{
    private const string Contract = "Mcd.Rational";

    private static System.Type QRational => typeof(Rational);
    private static System.Type MRational => Mathcad.Type("mplrun", "Mcd.Rational");

    private static string Show(object? o) => o switch
    {
        null => "null",
        Rational r => $"{r.Numerator}/{r.Denominator}",
        object r when r.GetType().Name == "Rational" => $"{Get(r, "Numerator")}/{Get(r, "Denominator")}",
        double d => d.ToString("R"),
        _ => Text(o)
    };

    private static string State(object o) => Show(o);

    private static void Compare(string caseId, MethodBase quire, object? quireTarget, object?[] quireArgs,
        MethodBase original, object? originalTarget, object?[] originalArgs)
    {
        bool isInstance = !quire.IsStatic && quire is not ConstructorInfo;
        MethodComparison.Compare(caseId, Contract, quire, quireTarget, quireArgs, original, originalTarget, originalArgs,
            Show, Show, isInstance ? State : null, isInstance ? State : null);
    }

    private static (MethodInfo q, MethodInfo m) Method(string name, params System.Type[] paramTypes)
    {
        var q = QRational.GetMethod(name, paramTypes)
            ?? throw new HarnessException($"Quire method not found: Rational.{name}");
        var mParamTypes = paramTypes.Select(p => p == QRational ? MRational : p).ToArray();
        var m = MRational.GetMethod(name, mParamTypes)
            ?? throw new HarnessException($"Mathcad method not found: Rational.{name}");
        return (q, m);
    }

    private static (MethodInfo q, MethodInfo m) StaticMethod(string name, System.Type returnType, params System.Type[] paramTypes)
    {
        var mRetType = returnType == QRational ? MRational : returnType;
        var mParamTypes = paramTypes.Select(p => p == QRational ? MRational : p).ToArray();
        var q = QRational.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == name && m.ReturnType == returnType && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(paramTypes));
        var m = MRational.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == name && m.ReturnType == mRetType && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(mParamTypes));
        return (q, m);
    }

    private static (ConstructorInfo q, ConstructorInfo m) Ctor(params System.Type[] paramTypes)
    {
        var q = QRational.GetConstructor(paramTypes)
            ?? throw new HarnessException("Quire constructor not found");
        var m = MRational.GetConstructor(paramTypes)
            ?? throw new HarnessException("Mathcad constructor not found");
        return (q, m);
    }

    private static Rational QRat(int n, int d) => new(n, d);
    private static object MRat(int n, int d) => Activator.CreateInstance(MRational, n, d)!;
    private static Rational QRat(int n) => new(n);
    private static object MRat(int n) => Activator.CreateInstance(MRational, n)!;
    private static Rational QDef() => default;
    private static object MDef() => Activator.CreateInstance(MRational)!;

    [SkippableFact]
    public void Constructors_and_conversions()
    {
        // 1. .ctor(int)
        var (qCtorInt, mCtorInt) = Ctor(typeof(int));
        foreach (int n in new[] { 0, 5, -5, int.MaxValue, int.MinValue })
        {
            Compare($"new Rational({n})", qCtorInt, null, [n], mCtorInt, null, [n]);
        }

        // 2. .ctor(int, int)
        var (qCtorIntInt, mCtorIntInt) = Ctor(typeof(int), typeof(int));
        var pairs = new[]
        {
            (6, 8), (6, -8), (-6, 8), (-6, -8), (0, 5), (5, 0), (0, 0),
            (int.MinValue, -1), (int.MaxValue, int.MaxValue), (int.MinValue, 2),
            (46341, 65536), (12, -18)
        };
        foreach (var (n, d) in pairs)
        {
            Compare($"new Rational({n}, {d})", qCtorIntInt, null, [n, d], mCtorIntInt, null, [n, d]);
        }

        // 3. .ctor(double)
        var (qCtorDbl, mCtorDbl) = Ctor(typeof(double));
        foreach (double x in new[] { 0.0, 0.5, -0.5, Math.PI, 1e10, -1e10, double.NaN, double.PositiveInfinity })
        {
            Compare($"new Rational({x:R})", qCtorDbl, null, [x], mCtorDbl, null, [x]);
        }

        // 4. .ctor(double, double)
        var (qCtorDblDbl, mCtorDblDbl) = Ctor(typeof(double), typeof(double));
        var doublePairs = new (double x, double err)[]
        {
            (0.0, 1e-5), (0.5, 1e-5), (-0.5, 1e-5), (1.0 / 3, 1e-5), (-2.25, 1e-5),
            (Math.PI, 1e-5), (Math.PI, 1e-2), (Math.PI, 0.5), (Math.PI, 1e-9), (Math.PI, 1e-12),
            (Math.PI, 0.0), (Math.PI, -1.0), (Math.PI, double.NaN),
            (Math.E, 1e-9), (Math.Sqrt(2), 1e-5), (0.1, 1e-5), (1e-3, 1e-5), (123.456, 1e-5), (-987.654321, 1e-5), (3.0, 1e-5),
            (1.0 / (1 << 30), 1e-12), (1.0 / ((1 << 30) - 1), 1e-12), (5 + 1.0 / (1 << 30), 1e-12),
            (4500675881.0, 1e-5), (1e10, 1e-5), (-1e10, 1e-5),
            ((double)int.MaxValue, 1e-5), ((double)int.MaxValue + 0.5, 1e-5),
            (1073741824.5, 1e-12), (-1073741824.5, 1e-12),
            (2147483647.0, 1e-12), (-2147483647.0, 1e-12),
            (2147483647.5, 1e-12), (-2147483647.5, 1e-12),
            (double.NaN, 1e-5), (double.PositiveInfinity, 1e-5), (double.NegativeInfinity, 1e-5), (double.Epsilon, 1e-5), (1e-300, 1e-5)
        };
        foreach (var (x, err) in doublePairs)
        {
            Compare($"new Rational({x:R}, {err:R})", qCtorDblDbl, null, [x, err], mCtorDblDbl, null, [x, err]);
        }
        var rng = new Random(20261014);
        for (int i = 0; i < 15; i++)
        {
            double rx = (rng.NextDouble() - 0.5) * Math.Pow(10, rng.Next(-5, 6));
            double re = 1e-5;
            Compare($"new Rational(rand {rx:R}, {re:R})", qCtorDblDbl, null, [rx, re], mCtorDblDbl, null, [rx, re]);
        }

        // 5. op_Implicit(int) -> Rational
        var (qOpImpInt, mOpImpInt) = StaticMethod("op_Implicit", QRational, typeof(int));
        foreach (int n in new[] { 0, 1, -1, 65536, int.MaxValue, int.MinValue })
        {
            Compare($"(Rational)({n})", qOpImpInt, null, [n], mOpImpInt, null, [n]);
        }

        // 6. op_Implicit(double) -> Rational
        var (qOpImpDbl, mOpImpDbl) = StaticMethod("op_Implicit", QRational, typeof(double));
        foreach (double x in new[] { 0.0, 2.5, -2.5, Math.PI })
        {
            Compare($"(Rational)({x:R})", qOpImpDbl, null, [x], mOpImpDbl, null, [x]);
        }

        // 7. op_Implicit(Rational) -> double
        var (qOpImpToDbl, mOpImpToDbl) = StaticMethod("op_Implicit", typeof(double), QRational);
        var toDoubleRats = new[] { QRat(3, 4), QRat(-3, 4), QRat(5, 1), QRat(1, 0), QRat(-1, 0), QDef() };
        var mToDoubleRats = new[] { MRat(3, 4), MRat(-3, 4), MRat(5, 1), MRat(1, 0), MRat(-1, 0), MDef() };
        for (int i = 0; i < toDoubleRats.Length; i++)
        {
            Compare($"(double)({Show(toDoubleRats[i])})", qOpImpToDbl, null, [toDoubleRats[i]], mOpImpToDbl, null, [mToDoubleRats[i]]);
        }

        // 8. op_Explicit(Rational) -> int
        var (qOpExpToInt, mOpExpToInt) = StaticMethod("op_Explicit", typeof(int), QRational);
        var toIntRats = new[] { QRat(7, 2), QRat(-7, 2), QRat(5, 1), QRat(1, 0), QDef() };
        var mToIntRats = new[] { MRat(7, 2), MRat(-7, 2), MRat(5, 1), MRat(1, 0), MDef() };
        for (int i = 0; i < toIntRats.Length; i++)
        {
            Compare($"(int)({Show(toIntRats[i])})", qOpExpToInt, null, [toIntRats[i]], mOpExpToInt, null, [mToIntRats[i]]);
        }

        // 9. GCD(int, int)
        var (qGcd, mGcd) = Method("GCD", typeof(int), typeof(int));
        var gcdPairs = new[]
        {
            (12, 18), (18, 12), (-12, 18), (12, -18), (-12, -18),
            (0, 5), (5, 0), (0, 0), (17, 19), (int.MinValue, 0), (int.MaxValue, int.MaxValue)
        };
        foreach (var (m, n) in gcdPairs)
        {
            Compare($"GCD({m}, {n})", qGcd, null, [m, n], mGcd, null, [m, n]);
        }
    }

    [SkippableFact]
    public void Operators_and_arithmetic()
    {
        // 1. op_Multiply(Rational, int) and op_Division(Rational, int)
        var (qMulInt, mMulInt) = StaticMethod("op_Multiply", QRational, QRational, typeof(int));
        var (qDivInt, mDivInt) = StaticMethod("op_Division", QRational, QRational, typeof(int));
        foreach (int k in new[] { 2, -2, 0, 65536 })
        {
            Compare($"Rational(3, 4) * {k}", qMulInt, null, [QRat(3, 4), k], mMulInt, null, [MRat(3, 4), k]);
            Compare($"Rational(3, 4) / {k}", qDivInt, null, [QRat(3, 4), k], mDivInt, null, [MRat(3, 4), k]);
        }
        Compare("Rational(1, 0) * 2", qMulInt, null, [QRat(1, 0), 2], mMulInt, null, [MRat(1, 0), 2]);
        Compare("Rational(1, 65536) / 65536", qDivInt, null, [QRat(1, 65536), 65536], mDivInt, null, [MRat(1, 65536), 65536]);

        // 2. op_Multiply(Rational, Rational) and op_Division(Rational, Rational)
        var (qMulRat, mMulRat) = StaticMethod("op_Multiply", QRational, QRational, QRational);
        var (qDivRat, mDivRat) = StaticMethod("op_Division", QRational, QRational, QRational);
        var rPairs = new[]
        {
            (QRat(2, 3), MRat(2, 3), QRat(5, 7), MRat(5, 7)),
            (QRat(2, 3), MRat(2, 3), QRat(-5, 7), MRat(-5, 7)),
            (QRat(2, 3), MRat(2, 3), QRat(0, 1), MRat(0, 1)),
            (QRat(65536, 1), MRat(65536, 1), QRat(65536, 1), MRat(65536, 1)),
            (QRat(1, 65536), MRat(1, 65536), QRat(65536, 1), MRat(65536, 1))
        };
        foreach (var (qa, ma, qb, mb) in rPairs)
        {
            Compare($"{Show(qa)} * {Show(qb)}", qMulRat, null, [qa, qb], mMulRat, null, [ma, mb]);
            Compare($"{Show(qa)} / {Show(qb)}", qDivRat, null, [qa, qb], mDivRat, null, [ma, mb]);
        }

        // 3. op_Equality and op_Inequality
        var (qEq, mEq) = StaticMethod("op_Equality", typeof(bool), QRational, QRational);
        var (qNeq, mNeq) = StaticMethod("op_Inequality", typeof(bool), QRational, QRational);
        var compPairs = new[]
        {
            (QRat(3, 4), MRat(3, 4), QRat(3, 4), MRat(3, 4)),
            (QRat(3, 4), MRat(3, 4), QRat(2, 4), MRat(2, 4)),
            (QRat(3, 4), MRat(3, 4), QRat(3, 5), MRat(3, 5)),
            (QRat(3, 4), MRat(3, 4), QRat(5, 6), MRat(5, 6)),
            (QDef(), MDef(), QDef(), MDef()),
            (QDef(), MDef(), QRat(0, 1), MRat(0, 1))
        };
        foreach (var (qa, ma, qb, mb) in compPairs)
        {
            Compare($"{Show(qa)} == {Show(qb)}", qEq, null, [qa, qb], mEq, null, [ma, mb]);
            Compare($"{Show(qa)} != {Show(qb)}", qNeq, null, [qa, qb], mNeq, null, [ma, mb]);
        }

        // 4. Comparison operators: <=, >=, <, >
        var (qLte, mLte) = StaticMethod("op_LessThanOrEqual", typeof(bool), QRational, QRational);
        var (qGte, mGte) = StaticMethod("op_GreaterThanOrEqual", typeof(bool), QRational, QRational);
        var (qLt, mLt) = StaticMethod("op_LessThan", typeof(bool), QRational, QRational);
        var (qGt, mGt) = StaticMethod("op_GreaterThan", typeof(bool), QRational, QRational);
        var orderPairs = new[]
        {
            (QRat(1, 2), MRat(1, 2), QRat(3, 4), MRat(3, 4)),
            (QRat(3, 4), MRat(3, 4), QRat(1, 2), MRat(1, 2)),
            (QRat(1, 2), MRat(1, 2), QRat(1, 2), MRat(1, 2)),
            (QRat(-1, 2), MRat(-1, 2), QRat(1, 2), MRat(1, 2)),
            (QRat(1, 0), MRat(1, 0), QRat(5, 1), MRat(5, 1))
        };
        foreach (var (qa, ma, qb, mb) in orderPairs)
        {
            Compare($"{Show(qa)} <= {Show(qb)}", qLte, null, [qa, qb], mLte, null, [ma, mb]);
            Compare($"{Show(qa)} >= {Show(qb)}", qGte, null, [qa, qb], mGte, null, [ma, mb]);
            Compare($"{Show(qa)} < {Show(qb)}", qLt, null, [qa, qb], mLt, null, [ma, mb]);
            Compare($"{Show(qa)} > {Show(qb)}", qGt, null, [qa, qb], mGt, null, [ma, mb]);
        }
    }

    [SkippableFact]
    public void Instance_members()
    {
        // 1. get_IsIntegral
        var (qIsInt, mIsInt) = Method("get_IsIntegral");
        var isIntTargets = new[]
        {
            (QRat(7, 1), MRat(7, 1)),
            (QRat(7, 2), MRat(7, 2)),
            (QRat(0, 1), MRat(0, 1)),
            (QDef(), MDef())
        };
        foreach (var (qt, mt) in isIntTargets)
        {
            Compare($"{Show(qt)}.IsIntegral", qIsInt, qt, [], mIsInt, mt, []);
        }

        // 2. Equals(object)
        var (qEquals, mEquals) = Method("Equals", typeof(object));
        var targetQ = QRat(3, 4);
        var targetM = MRat(3, 4);
        var eqCases = new (object? qArg, object? mArg, string desc)[]
        {
            (QRat(3, 4), MRat(3, 4), "equal Rational"),
            (QRat(2, 4), MRat(2, 4), "diff num Rational"),
            (QRat(3, 5), MRat(3, 5), "diff den Rational"),
            (null, null, "null"),
            (1, 1, "int"),
            ("3/4", "3/4", "string")
        };
        foreach (var (qa, ma, desc) in eqCases)
        {
            Compare($"Rational(3, 4).Equals({desc})", qEquals, targetQ, [qa], mEquals, targetM, [ma]);
        }
        Compare("default.Equals(default)", qEquals, QDef(), [QDef()], mEquals, MDef(), [MDef()]);
        Compare("default.Equals(Rational(0, 1))", qEquals, QDef(), [QRat(0, 1)], mEquals, MDef(), [MRat(0, 1)]);

        // 3. GetHashCode
        var (qHash, mHash) = Method("GetHashCode");
        var hashTargets = new[]
        {
            (QRat(10, 3), MRat(10, 3)),
            (QRat(-10, 3), MRat(-10, 3)),
            (QRat(0, 1), MRat(0, 1)),
            (QRat(1, 0), MRat(1, 0)),
            (QDef(), MDef())
        };
        foreach (var (qt, mt) in hashTargets)
        {
            Compare($"{Show(qt)}.GetHashCode()", qHash, qt, [], mHash, mt, []);
        }

        // 4. ToString
        var (qToStr, mToStr) = Method("ToString");
        var strTargets = new[]
        {
            (QRat(5, 1), MRat(5, 1)),
            (QRat(3, 4), MRat(3, 4)),
            (QRat(-3, 4), MRat(-3, 4)),
            (QRat(0, 1), MRat(0, 1)),
            (QRat(1, 0), MRat(1, 0)),
            (QDef(), MDef())
        };
        foreach (var (qt, mt) in strTargets)
        {
            Compare($"{Show(qt)}.ToString()", qToStr, qt, [], mToStr, mt, []);
        }
    }
}
