using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRDimArithmeticDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int MMult1Token = 0x060007cc;
    private const int MMult2CDimToken = 0x060007cd;
    private const int MMult2RcDimToken = 0x060007ce;
    private const int DMult1Token = 0x060007cf;
    private const int DMult2RDimToken = 0x060007d0;
    private const int DMult2CDimToken = 0x060007d1;
    private const int TransposeToken = 0x060007d2;
    private const int DetToken = 0x060007d3;
    private const int InvertToken = 0x060007d4;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");
    private static Type RcDim(Side side) => Nested(MatDim(side), "rc_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static MethodInfo MMult1(Side side) =>
        RDim(side).GetMethod("mmult1", DeclaredInstance, null, [MatDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.mmult1(MatDim)");

    private static MethodInfo MMult2CDim(Side side) =>
        RDim(side).GetMethod("mmult2", DeclaredInstance, null, [CDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.mmult2(c_dim)");

    private static MethodInfo MMult2RcDim(Side side) =>
        RDim(side).GetMethod("mmult2", DeclaredInstance, null, [RcDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.mmult2(rc_dim)");

    private static MethodInfo DMult1(Side side) =>
        RDim(side).GetMethod("dmult1", DeclaredInstance, null, [MatDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.dmult1(MatDim)");

    private static MethodInfo DMult2RDim(Side side) =>
        RDim(side).GetMethod("dmult2", DeclaredInstance, null, [RDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.dmult2(r_dim)");

    private static MethodInfo DMult2CDim(Side side) =>
        RDim(side).GetMethod("dmult2", DeclaredInstance, null, [CDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.dmult2(c_dim)");

    private static MethodInfo Transpose(Side side) =>
        RDim(side).GetMethod("transpose", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.transpose()");

    private static MethodInfo Det(Side side) =>
        RDim(side).GetMethod("det", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.det(int)");

    private static MethodInfo Invert(Side side) =>
        RDim(side).GetMethod("invert", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.invert()");

    private static object MakeRDim(Side side, string[] dimNames, int cols)
    {
        var array = Array.CreateInstance(side.Dim, dimNames.Length);
        for (int i = 0; i < dimNames.Length; i++) array.SetValue(side.D(dimNames[i]), i);
        return Call(RDim(side), "make", array, cols)!;
    }

    private static object MakeCDim(Side side, string[] dimNames, int rows)
    {
        var array = Array.CreateInstance(side.Dim, dimNames.Length);
        for (int i = 0; i < dimNames.Length; i++) array.SetValue(side.D(dimNames[i]), i);
        return Call(CDim(side), "make", array, rows)!;
    }

    private static object MakeRcDim(Side side, string[] rNames, string[] cNames)
    {
        var rArray = Array.CreateInstance(side.Dim, rNames.Length);
        for (int i = 0; i < rNames.Length; i++) rArray.SetValue(side.D(rNames[i]), i);
        var cArray = Array.CreateInstance(side.Dim, cNames.Length);
        for (int i = 0; i < cNames.Length; i++) cArray.SetValue(side.D(cNames[i]), i);
        return Call(RcDim(side), "make_rc", rArray, cArray)!;
    }

    private static object MakeUniform(Side side, string dimName, int rows, int cols) =>
        Call(UniformDim(side), "make", side.D(dimName), rows, cols)!;

    private static object MakeCrazy(Side side)
    {
        object matrix = New(side.T("Mcd.MatVal"), 2, 3);
        Set(matrix, "m_dim", side.D("L"));
        var type = Nested(side.T("Mcd.MatDim"), "crazy_dim");
        var make = type.GetMethod("make", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            null, [side.T("Mcd.MatVal")], null) ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");
        return make.Invoke(null, [matrix])!;
    }

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is int i) return i.ToString();
        if (value.GetType() == side.Dim) return $"Dim({side.DimText(value)})";
        if (value.GetType() == UniformDim(side))
        {
            return $"uniform_dim({Get(value, "Rows")}x{Get(value, "Cols")},dim={side.DimText(Get(value, "m_dim"))})";
        }
        if (value.GetType() == RDim(side))
        {
            var rowdim = (Array)Get(value, "m_rowdim")!;
            var items = string.Join(",", rowdim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"r_dim({Get(value, "Rows")}x{Get(value, "Cols")},rowdim=[{items}])";
        }
        if (value.GetType() == CDim(side))
        {
            var coldim = (Array)Get(value, "m_coldim")!;
            var items = string.Join(",", coldim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"c_dim({Get(value, "Rows")}x{Get(value, "Cols")},coldim=[{items}])";
        }
        if (value.GetType() == RcDim(side))
        {
            var rowdim = (Array)Get(value, "m_rowdim")!;
            var coldim = (Array)Get(value, "m_coldim")!;
            var rItems = string.Join(",", rowdim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            var cItems = string.Join(",", coldim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"rc_dim({Get(value, "Rows")}x{Get(value, "Cols")},rowdim=[{rItems}],coldim=[{cItems}])";
        }
        if (value.GetType().Name == "crazy_dim")
        {
            return $"crazy_dim({Get(value, "Rows")}x{Get(value, "Cols")})";
        }
        return Reflect.Text(value);
    }

    private static string State(Side side, object target) => Show(side, target);

    [SkippableFact]
    public void R_dim_arithmetic_operations_match_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(MMult1Token, MMult1(Side.Mathcad).MetadataToken);
        Assert.Equal(MMult2CDimToken, MMult2CDim(Side.Mathcad).MetadataToken);
        Assert.Equal(MMult2RcDimToken, MMult2RcDim(Side.Mathcad).MetadataToken);
        Assert.Equal(DMult1Token, DMult1(Side.Mathcad).MetadataToken);
        Assert.Equal(DMult2RDimToken, DMult2RDim(Side.Mathcad).MetadataToken);
        Assert.Equal(DMult2CDimToken, DMult2CDim(Side.Mathcad).MetadataToken);
        Assert.Equal(TransposeToken, Transpose(Side.Mathcad).MetadataToken);
        Assert.Equal(DetToken, Det(Side.Mathcad).MetadataToken);
        Assert.Equal(InvertToken, Invert(Side.Mathcad).MetadataToken);

        // mmult1 dispatch
        {
            object qTarget = MakeRDim(Side.Quire, ["L", "T"], 3);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T"], 3);
            object qCDim = MakeCDim(Side.Quire, ["identity", "identity", "identity"], 2);
            object mCDim = MakeCDim(Side.Mathcad, ["identity", "identity", "identity"], 2);
            MethodComparison.Compare("mmult1 dispatch to c_dim", Contract,
                MMult1(Side.Quire), qTarget, [qCDim],
                MMult1(Side.Mathcad), mTarget, [mCDim],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qUniform = MakeUniform(Side.Quire, "L", 3, 4);
            object mUniform = MakeUniform(Side.Mathcad, "L", 3, 4);
            MethodComparison.Compare("mmult1 dispatch to uniform_dim", Contract,
                MMult1(Side.Quire), qTarget, [qUniform],
                MMult1(Side.Mathcad), mTarget, [mUniform],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qRDim = MakeRDim(Side.Quire, ["L", "T", "v"], 2);
            object mRDim = MakeRDim(Side.Mathcad, ["L", "T", "v"], 2);
            MethodComparison.Compare("mmult1 dispatch to r_dim returns null", Contract,
                MMult1(Side.Quire), qTarget, [qRDim],
                MMult1(Side.Mathcad), mTarget, [mRDim],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qCrazy = MakeCrazy(Side.Quire);
            object mCrazy = MakeCrazy(Side.Mathcad);
            MethodComparison.Compare("mmult1 dispatch to crazy_dim fallback returns null", Contract,
                MMult1(Side.Quire), qTarget, [qCrazy],
                MMult1(Side.Mathcad), mTarget, [mCrazy],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // mmult2(c_dim)
        {
            object qTargetUniform = MakeRDim(Side.Quire, ["L", "L"], 3);
            object mTargetUniform = MakeRDim(Side.Mathcad, ["L", "L"], 3);
            object qCDimMatching = MakeCDim(Side.Quire, ["identity", "identity"], 4);
            object mCDimMatching = MakeCDim(Side.Mathcad, ["identity", "identity"], 4);
            MethodComparison.Compare("mmult2(c_dim) compatible lengths with non-null factor", Contract,
                MMult2CDim(Side.Quire), qTargetUniform, [qCDimMatching],
                MMult2CDim(Side.Mathcad), mTargetUniform, [mCDimMatching],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qTarget = MakeRDim(Side.Quire, ["L", "T"], 3);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T"], 3);
            object qCDimMismatched = MakeCDim(Side.Quire, ["identity", "identity", "identity"], 4);
            object mCDimMismatched = MakeCDim(Side.Mathcad, ["identity", "identity", "identity"], 4);
            MethodComparison.Compare("mmult2(c_dim) incompatible lengths returns null", Contract,
                MMult2CDim(Side.Quire), qTarget, [qCDimMismatched],
                MMult2CDim(Side.Mathcad), mTarget, [mCDimMismatched],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qCDimIncompatibleUnits = MakeCDim(Side.Quire, ["L", "T"], 4);
            object mCDimIncompatibleUnits = MakeCDim(Side.Mathcad, ["L", "T"], 4);
            MethodComparison.Compare("mmult2(c_dim) incompatible unit summation returns null", Contract,
                MMult2CDim(Side.Quire), qTarget, [qCDimIncompatibleUnits],
                MMult2CDim(Side.Mathcad), mTarget, [mCDimIncompatibleUnits],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // mmult2(rc_dim)
        {
            object qTargetUniform = MakeRDim(Side.Quire, ["L", "L"], 3);
            object mTargetUniform = MakeRDim(Side.Mathcad, ["L", "L"], 3);
            object qRcMatching = MakeRcDim(Side.Quire, ["v", "N"], ["identity", "identity"]);
            object mRcMatching = MakeRcDim(Side.Mathcad, ["v", "N"], ["identity", "identity"]);
            MethodComparison.Compare("mmult2(rc_dim) compatible inner length with non-null factor", Contract,
                MMult2RcDim(Side.Quire), qTargetUniform, [qRcMatching],
                MMult2RcDim(Side.Mathcad), mTargetUniform, [mRcMatching],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qTarget = MakeRDim(Side.Quire, ["L", "T"], 3);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T"], 3);
            object qRcMismatched = MakeRcDim(Side.Quire, ["v", "N"], ["identity", "identity", "identity"]);
            object mRcMismatched = MakeRcDim(Side.Mathcad, ["v", "N"], ["identity", "identity", "identity"]);
            MethodComparison.Compare("mmult2(rc_dim) mismatched inner length returns null", Contract,
                MMult2RcDim(Side.Quire), qTarget, [qRcMismatched],
                MMult2RcDim(Side.Mathcad), mTarget, [mRcMismatched],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qRcIncompatible = MakeRcDim(Side.Quire, ["v", "N"], ["L", "T"]);
            object mRcIncompatible = MakeRcDim(Side.Mathcad, ["v", "N"], ["L", "T"]);
            MethodComparison.Compare("mmult2(rc_dim) incompatible unit summation returns null", Contract,
                MMult2RcDim(Side.Quire), qTarget, [qRcIncompatible],
                MMult2RcDim(Side.Mathcad), mTarget, [mRcIncompatible],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // dmult1 dispatch
        {
            object qTarget = MakeRDim(Side.Quire, ["L", "T"], 3);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T"], 3);
            object qRDimMatching = MakeRDim(Side.Quire, ["identity", "identity"], 2);
            object mRDimMatching = MakeRDim(Side.Mathcad, ["identity", "identity"], 2);
            MethodComparison.Compare("dmult1 dispatch to r_dim", Contract,
                DMult1(Side.Quire), qTarget, [qRDimMatching],
                DMult1(Side.Mathcad), mTarget, [mRDimMatching],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qCDimMatching = MakeCDim(Side.Quire, ["identity", "identity"], 5);
            object mCDimMatching = MakeCDim(Side.Mathcad, ["identity", "identity"], 5);
            MethodComparison.Compare("dmult1 dispatch to c_dim", Contract,
                DMult1(Side.Quire), qTarget, [qCDimMatching],
                DMult1(Side.Mathcad), mTarget, [mCDimMatching],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qUniform = MakeUniform(Side.Quire, "L", 2, 3);
            object mUniform = MakeUniform(Side.Mathcad, "L", 2, 3);
            MethodComparison.Compare("dmult1 dispatch to uniform_dim returns null", Contract,
                DMult1(Side.Quire), qTarget, [qUniform],
                DMult1(Side.Mathcad), mTarget, [mUniform],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // dmult2(r_dim)
        {
            object qTarget = MakeRDim(Side.Quire, ["L", "T"], 3);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T"], 3);
            object qRDimMatching = MakeRDim(Side.Quire, ["identity", "identity"], 2);
            object mRDimMatching = MakeRDim(Side.Mathcad, ["identity", "identity"], 2);
            MethodComparison.Compare("dmult2(r_dim) matching lengths", Contract,
                DMult2RDim(Side.Quire), qTarget, [qRDimMatching],
                DMult2RDim(Side.Mathcad), mTarget, [mRDimMatching],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qRDimMismatched = MakeRDim(Side.Quire, ["identity"], 2);
            object mRDimMismatched = MakeRDim(Side.Mathcad, ["identity"], 2);
            MethodComparison.Compare("dmult2(r_dim) mismatched lengths returns null", Contract,
                DMult2RDim(Side.Quire), qTarget, [qRDimMismatched],
                DMult2RDim(Side.Mathcad), mTarget, [mRDimMismatched],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qRDimIncompatible = MakeRDim(Side.Quire, ["L", "T"], 2);
            object mRDimIncompatible = MakeRDim(Side.Mathcad, ["L", "T"], 2);
            MethodComparison.Compare("dmult2(r_dim) incompatible summation returns null", Contract,
                DMult2RDim(Side.Quire), qTarget, [qRDimIncompatible],
                DMult2RDim(Side.Mathcad), mTarget, [mRDimIncompatible],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // dmult2(c_dim)
        {
            object qTarget = MakeRDim(Side.Quire, ["L", "T"], 3);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T"], 3);
            object qCDimMatching = MakeCDim(Side.Quire, ["identity", "identity"], 5);
            object mCDimMatching = MakeCDim(Side.Mathcad, ["identity", "identity"], 5);
            MethodComparison.Compare("dmult2(c_dim) matching lengths", Contract,
                DMult2CDim(Side.Quire), qTarget, [qCDimMatching],
                DMult2CDim(Side.Mathcad), mTarget, [mCDimMatching],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qCDimMismatched = MakeCDim(Side.Quire, ["identity", "identity", "identity"], 5);
            object mCDimMismatched = MakeCDim(Side.Mathcad, ["identity", "identity", "identity"], 5);
            MethodComparison.Compare("dmult2(c_dim) mismatched lengths returns null", Contract,
                DMult2CDim(Side.Quire), qTarget, [qCDimMismatched],
                DMult2CDim(Side.Mathcad), mTarget, [mCDimMismatched],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qCDimIncompatible = MakeCDim(Side.Quire, ["L", "T"], 5);
            object mCDimIncompatible = MakeCDim(Side.Mathcad, ["L", "T"], 5);
            MethodComparison.Compare("dmult2(c_dim) incompatible summation returns null", Contract,
                DMult2CDim(Side.Quire), qTarget, [qCDimIncompatible],
                DMult2CDim(Side.Mathcad), mTarget, [mCDimIncompatible],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // transpose
        {
            object qTarget = MakeRDim(Side.Quire, ["L", "T", "v"], 4);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T", "v"], 4);
            MethodComparison.Compare("transpose normal r_dim to c_dim", Contract,
                Transpose(Side.Quire), qTarget, [],
                Transpose(Side.Mathcad), mTarget, [],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qSingleton = MakeRDim(Side.Quire, ["identity"], 1);
            object mSingleton = MakeRDim(Side.Mathcad, ["identity"], 1);
            MethodComparison.Compare("transpose singleton r_dim to c_dim", Contract,
                Transpose(Side.Quire), qSingleton, [],
                Transpose(Side.Mathcad), mSingleton, [],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qEmpty = MakeRDim(Side.Quire, [], 2);
            object mEmpty = MakeRDim(Side.Mathcad, [], 2);
            MethodComparison.Compare("transpose empty r_dim to c_dim", Contract,
                Transpose(Side.Quire), qEmpty, [],
                Transpose(Side.Mathcad), mEmpty, [],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // det(int n)
        {
            object qTarget = MakeRDim(Side.Quire, ["L", "T", "v"], 4);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T", "v"], 4);
            foreach (int n in new[] { 0, 1, 3, -2 })
            {
                MethodComparison.Compare($"det with n={n}", Contract,
                    Det(Side.Quire), qTarget, [n],
                    Det(Side.Mathcad), mTarget, [n],
                    v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                    t => State(Side.Quire, t), t => State(Side.Mathcad, t));
            }

            object qSingleton = MakeRDim(Side.Quire, ["L"], 1);
            object mSingleton = MakeRDim(Side.Mathcad, ["L"], 1);
            MethodComparison.Compare("det singleton r_dim", Contract,
                Det(Side.Quire), qSingleton, [1],
                Det(Side.Mathcad), mSingleton, [1],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qEmpty = MakeRDim(Side.Quire, [], 0);
            object mEmpty = MakeRDim(Side.Mathcad, [], 0);
            MethodComparison.Compare("det empty r_dim returns identity", Contract,
                Det(Side.Quire), qEmpty, [1],
                Det(Side.Mathcad), mEmpty, [1],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }

        // invert()
        {
            object qTarget = MakeRDim(Side.Quire, ["L", "T"], 3);
            object mTarget = MakeRDim(Side.Mathcad, ["L", "T"], 3);
            MethodComparison.Compare("invert normal r_dim to c_dim", Contract,
                Invert(Side.Quire), qTarget, [],
                Invert(Side.Mathcad), mTarget, [],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qSingleton = MakeRDim(Side.Quire, ["L"], 1);
            object mSingleton = MakeRDim(Side.Mathcad, ["L"], 1);
            MethodComparison.Compare("invert singleton r_dim to c_dim", Contract,
                Invert(Side.Quire), qSingleton, [],
                Invert(Side.Mathcad), mSingleton, [],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));

            object qEmpty = MakeRDim(Side.Quire, [], 0);
            object mEmpty = MakeRDim(Side.Mathcad, [], 0);
            MethodComparison.Compare("invert empty r_dim to c_dim", Contract,
                Invert(Side.Quire), qEmpty, [],
                Invert(Side.Mathcad), mEmpty, [],
                v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
                t => State(Side.Quire, t), t => State(Side.Mathcad, t));
        }
    }
}
