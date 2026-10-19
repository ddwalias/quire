using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimOperationsDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private const int CtorToken = 0x0600060b;
    private const int GetRowsToken = 0x0600060f;
    private const int GetColsToken = 0x06000610;
    private const int EqBMatDimMatDimToken = 0x06000615;
    private const int EqBMatDimDimToken = 0x06000616;
    private const int EqMatDimMatDimToken = 0x06000617;
    private const int EqMatDimDimToken = 0x06000618;
    private const int MMultToken = 0x0600061a;
    private const int DMultToken = 0x0600061b;
    private const int TransposeToken = 0x0600061c;
    private const int CMultToken = 0x0600061d;
    private const int DetToken = 0x0600061e;
    private const int PushToken = 0x06000630;
    private const int MMult1Token = 0x06000631;
    private const int MMult2UniformDimToken = 0x06000632;
    private const int MMult2RDimToken = 0x06000633;
    private const int MMult2CDimToken = 0x06000634;
    private const int MMult2RcDimToken = 0x06000635;
    private const int DMult1Token = 0x06000636;
    private const int DMult2UniformDimToken = 0x06000637;
    private const int DMult2RDimToken = 0x06000638;
    private const int DMult2CDimToken = 0x06000639;
    private const int DMult2RcDimToken = 0x0600063a;
    private const int Eq2BDimToken = 0x0600063c;
    private const int Eq2BRDimToken = 0x0600063d;
    private const int Eq2BCDimToken = 0x0600063e;
    private const int Eq2BRcDimToken = 0x0600063f;
    private const int Eq2BCrazyDimToken = 0x06000640;
    private const int OnErrorStringDimUIntToken = 0x06000641;
    private const int OnErrorDimUIntToken = 0x06000642;
    private const int OnErrorStringDimIntIntUIntToken = 0x06000643;
    private const int OnErrorStringDimIntUIntToken = 0x06000644;
    private const int PickLoserToken = 0x06000645;
    private const int TransposeVirtualToken = 0x06000646;
    private const int InvertVirtualToken = 0x06000647;
    private const int DetVirtualToken = 0x06000648;
    private const int PwrVirtualToken = 0x06000649;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");
    private static Type RcDim(Side side) => Nested(MatDim(side), "rc_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");
    private static Type CrazyDim(Side side) => Nested(MatDim(side), "crazy_dim");

    private static object MakeCrazy(Side side, int rows = 2, int cols = 3)
    {
        object matrix = New(side.T("Mcd.MatVal"), rows, cols);
        Set(matrix, "m_dim", side.D("L"));
        var make = CrazyDim(side).GetMethod("make", DeclaredStatic, null, [side.T("Mcd.MatVal")], null)
            ?? throw new HarnessException($"Missing {side.Name} crazy_dim.make(MatVal)");
        return make.Invoke(null, [matrix])!;
    }

    private static object MakeUniform(Side side, string dimName, int rows, int cols) =>
        Call(UniformDim(side), "make", side.D(dimName), rows, cols)!;

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

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is int i) return i.ToString();
        if (value is bool b) return b ? "true" : "false";
        if (value.GetType() == side.Dim) return $"Dim({side.DimText(value)})";
        if (value.GetType() == Array.CreateInstance(side.Dim, 0).GetType())
        {
            var arr = (Array)value;
            var items = string.Join(",", arr.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"Dim[{items}]";
        }
        var t = side.T("Mcd.MatDim");
        var uniform = Nested(t, "uniform_dim");
        var row = Nested(t, "r_dim");
        var col = Nested(t, "c_dim");
        var rc = Nested(t, "rc_dim");
        if (value.GetType() == uniform)
        {
            return $"uniform_dim({Get(value, "Rows")}x{Get(value, "Cols")},dim={side.DimText(Get(value, "m_dim"))})";
        }
        if (value.GetType() == row)
        {
            var rowdim = (Array)Get(value, "m_rowdim")!;
            var items = string.Join(",", rowdim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"r_dim({Get(value, "Rows")}x{Get(value, "Cols")},rowdim=[{items}])";
        }
        if (value.GetType() == col)
        {
            var coldim = (Array)Get(value, "m_coldim")!;
            var items = string.Join(",", coldim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"c_dim({Get(value, "Rows")}x{Get(value, "Cols")},coldim=[{items}])";
        }
        if (value.GetType() == rc)
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
    public void Base_virtual_dispatch_methods_match_mathcad()
    {
        Runtime.Sync();

        var qMatDim = MatDim(Side.Quire);
        var mMatDim = MatDim(Side.Mathcad);

        var qMmult1 = qMatDim.GetMethod("mmult1", DeclaredInstance, null, [qMatDim], null)!;
        var mMmult1 = mMatDim.GetMethod("mmult1", DeclaredInstance, null, [mMatDim], null)!;
        Assert.Equal(MMult1Token, mMmult1.MetadataToken);
        Assert.Equal(MathcadMvid, mMmult1.Module.ModuleVersionId);

        var qMmult2Uniform = qMatDim.GetMethod("mmult2", DeclaredInstance, null, [UniformDim(Side.Quire)], null)!;
        var mMmult2Uniform = mMatDim.GetMethod("mmult2", DeclaredInstance, null, [UniformDim(Side.Mathcad)], null)!;
        Assert.Equal(MMult2UniformDimToken, mMmult2Uniform.MetadataToken);
        Assert.Equal(MathcadMvid, mMmult2Uniform.Module.ModuleVersionId);

        var qMmult2RDim = qMatDim.GetMethod("mmult2", DeclaredInstance, null, [RDim(Side.Quire)], null)!;
        var mMmult2RDim = mMatDim.GetMethod("mmult2", DeclaredInstance, null, [RDim(Side.Mathcad)], null)!;
        Assert.Equal(MMult2RDimToken, mMmult2RDim.MetadataToken);
        Assert.Equal(MathcadMvid, mMmult2RDim.Module.ModuleVersionId);

        var qMmult2CDim = qMatDim.GetMethod("mmult2", DeclaredInstance, null, [CDim(Side.Quire)], null)!;
        var mMmult2CDim = mMatDim.GetMethod("mmult2", DeclaredInstance, null, [CDim(Side.Mathcad)], null)!;
        Assert.Equal(MMult2CDimToken, mMmult2CDim.MetadataToken);
        Assert.Equal(MathcadMvid, mMmult2CDim.Module.ModuleVersionId);

        var qMmult2RcDim = qMatDim.GetMethod("mmult2", DeclaredInstance, null, [RcDim(Side.Quire)], null)!;
        var mMmult2RcDim = mMatDim.GetMethod("mmult2", DeclaredInstance, null, [RcDim(Side.Mathcad)], null)!;
        Assert.Equal(MMult2RcDimToken, mMmult2RcDim.MetadataToken);
        Assert.Equal(MathcadMvid, mMmult2RcDim.Module.ModuleVersionId);

        var qDmult1 = qMatDim.GetMethod("dmult1", DeclaredInstance, null, [qMatDim], null)!;
        var mDmult1 = mMatDim.GetMethod("dmult1", DeclaredInstance, null, [mMatDim], null)!;
        Assert.Equal(DMult1Token, mDmult1.MetadataToken);
        Assert.Equal(MathcadMvid, mDmult1.Module.ModuleVersionId);

        var qDmult2Uniform = qMatDim.GetMethod("dmult2", DeclaredInstance, null, [UniformDim(Side.Quire)], null)!;
        var mDmult2Uniform = mMatDim.GetMethod("dmult2", DeclaredInstance, null, [UniformDim(Side.Mathcad)], null)!;
        Assert.Equal(DMult2UniformDimToken, mDmult2Uniform.MetadataToken);
        Assert.Equal(MathcadMvid, mDmult2Uniform.Module.ModuleVersionId);

        var qDmult2RDim = qMatDim.GetMethod("dmult2", DeclaredInstance, null, [RDim(Side.Quire)], null)!;
        var mDmult2RDim = mMatDim.GetMethod("dmult2", DeclaredInstance, null, [RDim(Side.Mathcad)], null)!;
        Assert.Equal(DMult2RDimToken, mDmult2RDim.MetadataToken);
        Assert.Equal(MathcadMvid, mDmult2RDim.Module.ModuleVersionId);

        var qDmult2CDim = qMatDim.GetMethod("dmult2", DeclaredInstance, null, [CDim(Side.Quire)], null)!;
        var mDmult2CDim = mMatDim.GetMethod("dmult2", DeclaredInstance, null, [CDim(Side.Mathcad)], null)!;
        Assert.Equal(DMult2CDimToken, mDmult2CDim.MetadataToken);
        Assert.Equal(MathcadMvid, mDmult2CDim.Module.ModuleVersionId);

        var qDmult2RcDim = qMatDim.GetMethod("dmult2", DeclaredInstance, null, [RcDim(Side.Quire)], null)!;
        var mDmult2RcDim = mMatDim.GetMethod("dmult2", DeclaredInstance, null, [RcDim(Side.Mathcad)], null)!;
        Assert.Equal(DMult2RcDimToken, mDmult2RcDim.MetadataToken);
        Assert.Equal(MathcadMvid, mDmult2RcDim.Module.ModuleVersionId);

        var qEq2BDim = qMatDim.GetMethod("eq2_b", DeclaredInstance, null, [Side.Quire.Dim], null)!;
        var mEq2BDim = mMatDim.GetMethod("eq2_b", DeclaredInstance, null, [Side.Mathcad.Dim], null)!;
        Assert.Equal(Eq2BDimToken, mEq2BDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEq2BDim.Module.ModuleVersionId);

        var qEq2BRDim = qMatDim.GetMethod("eq2_b", DeclaredInstance, null, [RDim(Side.Quire)], null)!;
        var mEq2BRDim = mMatDim.GetMethod("eq2_b", DeclaredInstance, null, [RDim(Side.Mathcad)], null)!;
        Assert.Equal(Eq2BRDimToken, mEq2BRDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEq2BRDim.Module.ModuleVersionId);

        var qEq2BCDim = qMatDim.GetMethod("eq2_b", DeclaredInstance, null, [CDim(Side.Quire)], null)!;
        var mEq2BCDim = mMatDim.GetMethod("eq2_b", DeclaredInstance, null, [CDim(Side.Mathcad)], null)!;
        Assert.Equal(Eq2BCDimToken, mEq2BCDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEq2BCDim.Module.ModuleVersionId);

        var qEq2BRcDim = qMatDim.GetMethod("eq2_b", DeclaredInstance, null, [RcDim(Side.Quire)], null)!;
        var mEq2BRcDim = mMatDim.GetMethod("eq2_b", DeclaredInstance, null, [RcDim(Side.Mathcad)], null)!;
        Assert.Equal(Eq2BRcDimToken, mEq2BRcDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEq2BRcDim.Module.ModuleVersionId);

        var qEq2BCrazyDim = qMatDim.GetMethod("eq2_b", DeclaredInstance, null, [CrazyDim(Side.Quire)], null)!;
        var mEq2BCrazyDim = mMatDim.GetMethod("eq2_b", DeclaredInstance, null, [CrazyDim(Side.Mathcad)], null)!;
        Assert.Equal(Eq2BCrazyDimToken, mEq2BCrazyDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEq2BCrazyDim.Module.ModuleVersionId);

        var qTranspose = qMatDim.GetMethod("transpose", DeclaredInstance, null, Type.EmptyTypes, null)!;
        var mTranspose = mMatDim.GetMethod("transpose", DeclaredInstance, null, Type.EmptyTypes, null)!;
        Assert.Equal(TransposeVirtualToken, mTranspose.MetadataToken);
        Assert.Equal(MathcadMvid, mTranspose.Module.ModuleVersionId);

        var qInvert = qMatDim.GetMethod("invert", DeclaredInstance, null, Type.EmptyTypes, null)!;
        var mInvert = mMatDim.GetMethod("invert", DeclaredInstance, null, Type.EmptyTypes, null)!;
        Assert.Equal(InvertVirtualToken, mInvert.MetadataToken);
        Assert.Equal(MathcadMvid, mInvert.Module.ModuleVersionId);

        var qDet = qMatDim.GetMethod("det", DeclaredInstance, null, [typeof(int)], null)!;
        var mDet = mMatDim.GetMethod("det", DeclaredInstance, null, [typeof(int)], null)!;
        Assert.Equal(DetVirtualToken, mDet.MetadataToken);
        Assert.Equal(MathcadMvid, mDet.Module.ModuleVersionId);

        var qPwr = qMatDim.GetMethod("pwr", DeclaredInstance, null, [typeof(uint)], null)!;
        var mPwr = mMatDim.GetMethod("pwr", DeclaredInstance, null, [typeof(uint)], null)!;
        Assert.Equal(PwrVirtualToken, mPwr.MetadataToken);
        Assert.Equal(MathcadMvid, mPwr.Module.ModuleVersionId);

        // Fixtures for testing base virtual dispatches
        object qCrazyTarget = MakeCrazy(Side.Quire, 2, 3);
        object mCrazyTarget = MakeCrazy(Side.Mathcad, 2, 3);

        object qUniformOperand = MakeUniform(Side.Quire, "L", 2, 2);
        object mUniformOperand = MakeUniform(Side.Mathcad, "L", 2, 2);

        object qRDimOperand = MakeRDim(Side.Quire, ["L", "T"], 3);
        object mRDimOperand = MakeRDim(Side.Mathcad, ["L", "T"], 3);

        object qCDimOperand = MakeCDim(Side.Quire, ["L", "T", "v"], 2);
        object mCDimOperand = MakeCDim(Side.Mathcad, ["L", "T", "v"], 2);

        object qRcDimOperand = MakeRcDim(Side.Quire, ["L", "T"], ["v", "N"]);
        object mRcDimOperand = MakeRcDim(Side.Mathcad, ["L", "T"], ["v", "N"]);

        object qDimOperand = Side.Quire.D("L");
        object mDimOperand = Side.Mathcad.D("L");

        // mmult1 returns null
        MethodComparison.Compare("MatDim.mmult1", Contract,
            qMmult1, qCrazyTarget, [qCrazyTarget],
            mMmult1, mCrazyTarget, [mCrazyTarget],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // mmult2 overloads return null
        MethodComparison.Compare("MatDim.mmult2.uniform", Contract,
            qMmult2Uniform, qCrazyTarget, [qUniformOperand],
            mMmult2Uniform, mCrazyTarget, [mUniformOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.mmult2.r_dim", Contract,
            qMmult2RDim, qCrazyTarget, [qRDimOperand],
            mMmult2RDim, mCrazyTarget, [mRDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.mmult2.c_dim", Contract,
            qMmult2CDim, qCrazyTarget, [qCDimOperand],
            mMmult2CDim, mCrazyTarget, [mCDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.mmult2.rc_dim", Contract,
            qMmult2RcDim, qCrazyTarget, [qRcDimOperand],
            mMmult2RcDim, mCrazyTarget, [mRcDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // dmult1 returns null
        MethodComparison.Compare("MatDim.dmult1", Contract,
            qDmult1, qCrazyTarget, [qCrazyTarget],
            mDmult1, mCrazyTarget, [mCrazyTarget],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // dmult2 overloads return null
        MethodComparison.Compare("MatDim.dmult2.uniform", Contract,
            qDmult2Uniform, qCrazyTarget, [qUniformOperand],
            mDmult2Uniform, mCrazyTarget, [mUniformOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.dmult2.r_dim", Contract,
            qDmult2RDim, qCrazyTarget, [qRDimOperand],
            mDmult2RDim, mCrazyTarget, [mRDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.dmult2.c_dim", Contract,
            qDmult2CDim, qCrazyTarget, [qCDimOperand],
            mDmult2CDim, mCrazyTarget, [mCDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.dmult2.rc_dim", Contract,
            qDmult2RcDim, qCrazyTarget, [qRcDimOperand],
            mDmult2RcDim, mCrazyTarget, [mRcDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // eq2_b overloads return false
        MethodComparison.Compare("MatDim.eq2_b.Dim", Contract,
            qEq2BDim, qCrazyTarget, [qDimOperand],
            mEq2BDim, mCrazyTarget, [mDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.eq2_b.r_dim", Contract,
            qEq2BRDim, qCrazyTarget, [qRDimOperand],
            mEq2BRDim, mCrazyTarget, [mRDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.eq2_b.c_dim", Contract,
            qEq2BCDim, qCrazyTarget, [qCDimOperand],
            mEq2BCDim, mCrazyTarget, [mCDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.eq2_b.rc_dim", Contract,
            qEq2BRcDim, qCrazyTarget, [qRcDimOperand],
            mEq2BRcDim, mCrazyTarget, [mRcDimOperand],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // eq2_b(crazy_dim) on uniform_dim target returns false
        MethodComparison.Compare("MatDim.eq2_b.crazy_dim", Contract,
            qEq2BCrazyDim, qUniformOperand, [qCrazyTarget],
            mEq2BCrazyDim, mUniformOperand, [mCrazyTarget],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // transpose returns null
        MethodComparison.Compare("MatDim.transpose", Contract,
            qTranspose, qCrazyTarget, [],
            mTranspose, mCrazyTarget, [],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // invert returns null
        MethodComparison.Compare("MatDim.invert", Contract,
            qInvert, qCrazyTarget, [],
            mInvert, mCrazyTarget, [],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // det returns null
        MethodComparison.Compare("MatDim.det", Contract,
            qDet, qCrazyTarget, [2],
            mDet, mCrazyTarget, [2],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        // pwr returns null
        MethodComparison.Compare("MatDim.pwr", Contract,
            qPwr, qCrazyTarget, [3u],
            mPwr, mCrazyTarget, [3u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Static_operations_match_mathcad()
    {
        Runtime.Sync();

        var qMatDim = MatDim(Side.Quire);
        var mMatDim = MatDim(Side.Mathcad);

        var qEqBMatDim = qMatDim.GetMethod("Eq_b", DeclaredStatic, null, [qMatDim, qMatDim], null)!;
        var mEqBMatDim = mMatDim.GetMethod("Eq_b", DeclaredStatic, null, [mMatDim, mMatDim], null)!;
        Assert.Equal(EqBMatDimMatDimToken, mEqBMatDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEqBMatDim.Module.ModuleVersionId);

        var qEqBDim = qMatDim.GetMethod("Eq_b", DeclaredStatic, null, [qMatDim, Side.Quire.Dim], null)!;
        var mEqBDim = mMatDim.GetMethod("Eq_b", DeclaredStatic, null, [mMatDim, Side.Mathcad.Dim], null)!;
        Assert.Equal(EqBMatDimDimToken, mEqBDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEqBDim.Module.ModuleVersionId);

        var qEqMatDim = qMatDim.GetMethod("Eq", DeclaredStatic, null, [qMatDim, qMatDim, typeof(uint)], null)!;
        var mEqMatDim = mMatDim.GetMethod("Eq", DeclaredStatic, null, [mMatDim, mMatDim, typeof(uint)], null)!;
        Assert.Equal(EqMatDimMatDimToken, mEqMatDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEqMatDim.Module.ModuleVersionId);

        var qEqDim = qMatDim.GetMethod("Eq", DeclaredStatic, null, [qMatDim, Side.Quire.Dim, typeof(uint)], null)!;
        var mEqDim = mMatDim.GetMethod("Eq", DeclaredStatic, null, [mMatDim, Side.Mathcad.Dim, typeof(uint)], null)!;
        Assert.Equal(EqMatDimDimToken, mEqDim.MetadataToken);
        Assert.Equal(MathcadMvid, mEqDim.Module.ModuleVersionId);

        var qMMult = qMatDim.GetMethod("MMult", DeclaredStatic, null, [qMatDim, qMatDim], null)!;
        var mMMult = mMatDim.GetMethod("MMult", DeclaredStatic, null, [mMatDim, mMatDim], null)!;
        Assert.Equal(MMultToken, mMMult.MetadataToken);
        Assert.Equal(MathcadMvid, mMMult.Module.ModuleVersionId);

        var qDMult = qMatDim.GetMethod("DMult", DeclaredStatic, null, [qMatDim, qMatDim], null)!;
        var mDMult = mMatDim.GetMethod("DMult", DeclaredStatic, null, [mMatDim, mMatDim], null)!;
        Assert.Equal(DMultToken, mDMult.MetadataToken);
        Assert.Equal(MathcadMvid, mDMult.Module.ModuleVersionId);

        var qTranspose = qMatDim.GetMethod("Transpose", DeclaredStatic, null, [qMatDim], null)!;
        var mTranspose = mMatDim.GetMethod("Transpose", DeclaredStatic, null, [mMatDim], null)!;
        Assert.Equal(TransposeToken, mTranspose.MetadataToken);
        Assert.Equal(MathcadMvid, mTranspose.Module.ModuleVersionId);

        var qDet = qMatDim.GetMethod("Det", DeclaredStatic, null, [qMatDim, typeof(int)], null)!;
        var mDet = mMatDim.GetMethod("Det", DeclaredStatic, null, [mMatDim, typeof(int)], null)!;
        Assert.Equal(DetToken, mDet.MetadataToken);
        Assert.Equal(MathcadMvid, mDet.Module.ModuleVersionId);

        object qUniform1 = MakeUniform(Side.Quire, "L", 2, 2);
        object mUniform1 = MakeUniform(Side.Mathcad, "L", 2, 2);

        object qUniform2 = MakeUniform(Side.Quire, "L", 2, 2);
        object mUniform2 = MakeUniform(Side.Mathcad, "L", 2, 2);

        object qUniformDiff = MakeUniform(Side.Quire, "T", 2, 2);
        object mUniformDiff = MakeUniform(Side.Mathcad, "T", 2, 2);

        object qCrazy = MakeCrazy(Side.Quire, 2, 2);
        object mCrazy = MakeCrazy(Side.Mathcad, 2, 2);

        object qDimL = Side.Quire.D("L");
        object mDimL = Side.Mathcad.D("L");

        object qDimT = Side.Quire.D("T");
        object mDimT = Side.Mathcad.D("T");

        // Eq_b(MatDim, MatDim)
        MethodComparison.Compare("MatDim.Eq_b.MatDim.true", Contract,
            qEqBMatDim, null, [qUniform1, qUniform2],
            mEqBMatDim, null, [mUniform1, mUniform2],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Eq_b.MatDim.false", Contract,
            qEqBMatDim, null, [qUniform1, qUniformDiff],
            mEqBMatDim, null, [mUniform1, mUniformDiff],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Eq_b(MatDim, Dim)
        MethodComparison.Compare("MatDim.Eq_b.Dim.true", Contract,
            qEqBDim, null, [qUniform1, qDimL],
            mEqBDim, null, [mUniform1, mDimL],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Eq_b.Dim.false", Contract,
            qEqBDim, null, [qUniform1, qDimT],
            mEqBDim, null, [mUniform1, mDimT],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Eq(MatDim, MatDim, uint)
        MethodComparison.Compare("MatDim.Eq.MatDim.pass", Contract,
            qEqMatDim, null, [qUniform1, qUniform2, 7u],
            mEqMatDim, null, [mUniform1, mUniform2, 7u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Eq.MatDim.throws", Contract,
            qEqMatDim, null, [qUniform1, qUniformDiff, 7u],
            mEqMatDim, null, [mUniform1, mUniformDiff, 7u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Eq(MatDim, Dim, uint)
        MethodComparison.Compare("MatDim.Eq.Dim.pass", Contract,
            qEqDim, null, [qUniform1, qDimL, 5u],
            mEqDim, null, [mUniform1, mDimL, 5u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Eq.Dim.throws", Contract,
            qEqDim, null, [qUniform1, qDimT, 5u],
            mEqDim, null, [mUniform1, mDimT, 5u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // MMult(MatDim, MatDim)
        MethodComparison.Compare("MatDim.MMult.uniform", Contract,
            qMMult, null, [qUniform1, qUniformDiff],
            mMMult, null, [mUniform1, mUniformDiff],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.MMult.crazy", Contract,
            qMMult, null, [qCrazy, qCrazy],
            mMMult, null, [mCrazy, mCrazy],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // DMult(MatDim, MatDim)
        MethodComparison.Compare("MatDim.DMult.uniform", Contract,
            qDMult, null, [qUniform1, qUniformDiff],
            mDMult, null, [mUniform1, mUniformDiff],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.DMult.crazy", Contract,
            qDMult, null, [qCrazy, qCrazy],
            mDMult, null, [mCrazy, mCrazy],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Transpose(MatDim)
        MethodComparison.Compare("MatDim.Transpose.uniform", Contract,
            qTranspose, null, [qUniform1],
            mTranspose, null, [mUniform1],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Transpose.crazy", Contract,
            qTranspose, null, [qCrazy],
            mTranspose, null, [mCrazy],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Det(MatDim, int)
        MethodComparison.Compare("MatDim.Det.uniform", Contract,
            qDet, null, [qUniform1, 2],
            mDet, null, [mUniform1, 2],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Det.crazy", Contract,
            qDet, null, [qCrazy, 2],
            mDet, null, [mCrazy, 2],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));
    }

    [SkippableFact]
    public void Accessors_and_helpers_match_mathcad()
    {
        Runtime.Sync();

        var qMatDim = MatDim(Side.Quire);
        var mMatDim = MatDim(Side.Mathcad);

        var qGetRows = qMatDim.GetMethod("get_Rows", DeclaredInstance, null, Type.EmptyTypes, null)!;
        var mGetRows = mMatDim.GetMethod("get_Rows", DeclaredInstance, null, Type.EmptyTypes, null)!;
        Assert.Equal(GetRowsToken, mGetRows.MetadataToken);
        Assert.Equal(MathcadMvid, mGetRows.Module.ModuleVersionId);

        var qGetCols = qMatDim.GetMethod("get_Cols", DeclaredInstance, null, Type.EmptyTypes, null)!;
        var mGetCols = mMatDim.GetMethod("get_Cols", DeclaredInstance, null, Type.EmptyTypes, null)!;
        Assert.Equal(GetColsToken, mGetCols.MetadataToken);
        Assert.Equal(MathcadMvid, mGetCols.Module.ModuleVersionId);

        var qCMult = qMatDim.GetMethod("CMult", DeclaredStatic, null, [qMatDim, qMatDim], null)!;
        var mCMult = mMatDim.GetMethod("CMult", DeclaredStatic, null, [mMatDim, mMatDim], null)!;
        Assert.Equal(CMultToken, mCMult.MetadataToken);
        Assert.Equal(MathcadMvid, mCMult.Module.ModuleVersionId);

        var qPush = qMatDim.GetMethod("Push", DeclaredStatic, null, [Side.Quire.Dim, Array.CreateInstance(Side.Quire.Dim, 0).GetType()], null)!;
        var mPush = mMatDim.GetMethod("Push", DeclaredStatic, null, [Side.Mathcad.Dim, Array.CreateInstance(Side.Mathcad.Dim, 0).GetType()], null)!;
        Assert.Equal(PushToken, mPush.MetadataToken);
        Assert.Equal(MathcadMvid, mPush.Module.ModuleVersionId);

        var qCtor = qMatDim.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, null, [typeof(int), typeof(int)], null)!;
        var mCtor = mMatDim.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, null, [typeof(int), typeof(int)], null)!;
        Assert.Equal(CtorToken, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        object qCrazyTarget = MakeCrazy(Side.Quire, 2, 3);
        object mCrazyTarget = MakeCrazy(Side.Mathcad, 2, 3);

        MethodComparison.Compare("MatDim.get_Rows", Contract,
            qGetRows, qCrazyTarget, [],
            mGetRows, mCrazyTarget, [],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.get_Cols", Contract,
            qGetCols, qCrazyTarget, [],
            mGetCols, mCrazyTarget, [],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v),
            t => State(Side.Quire, t), t => State(Side.Mathcad, t));

        MethodComparison.Compare("MatDim.CMult.null", Contract,
            qCMult, null, [null, null],
            mCMult, null, [null, null],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.CMult.instances", Contract,
            qCMult, null, [qCrazyTarget, qCrazyTarget],
            mCMult, null, [mCrazyTarget, mCrazyTarget],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Push
        var qDimArr2 = Array.CreateInstance(Side.Quire.Dim, 2);
        qDimArr2.SetValue(Side.Quire.D("T"), 0);
        qDimArr2.SetValue(Side.Quire.D("v"), 1);

        var mDimArr2 = Array.CreateInstance(Side.Mathcad.Dim, 2);
        mDimArr2.SetValue(Side.Mathcad.D("T"), 0);
        mDimArr2.SetValue(Side.Mathcad.D("v"), 1);

        MethodComparison.Compare("MatDim.Push.two", Contract,
            qPush, null, [Side.Quire.D("L"), qDimArr2],
            mPush, null, [Side.Mathcad.D("L"), mDimArr2],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        var qDimArr0 = Array.CreateInstance(Side.Quire.Dim, 0);
        var mDimArr0 = Array.CreateInstance(Side.Mathcad.Dim, 0);

        MethodComparison.Compare("MatDim.Push.empty", Contract,
            qPush, null, [Side.Quire.D("L"), qDimArr0],
            mPush, null, [Side.Mathcad.D("L"), mDimArr0],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));
    }

    [SkippableFact]
    public void Error_paths_and_pick_loser_match_mathcad()
    {
        Runtime.Sync();

        var qMatDim = MatDim(Side.Quire);
        var mMatDim = MatDim(Side.Mathcad);

        var qOnErrorStringDimUInt = qMatDim.GetMethod("OnError", DeclaredStatic, null, [typeof(string), Side.Quire.Dim, typeof(uint)], null)!;
        var mOnErrorStringDimUInt = mMatDim.GetMethod("OnError", DeclaredStatic, null, [typeof(string), Side.Mathcad.Dim, typeof(uint)], null)!;
        Assert.Equal(OnErrorStringDimUIntToken, mOnErrorStringDimUInt.MetadataToken);
        Assert.Equal(MathcadMvid, mOnErrorStringDimUInt.Module.ModuleVersionId);

        var qOnErrorDimUInt = qMatDim.GetMethod("OnError", DeclaredStatic, null, [Side.Quire.Dim, typeof(uint)], null)!;
        var mOnErrorDimUInt = mMatDim.GetMethod("OnError", DeclaredStatic, null, [Side.Mathcad.Dim, typeof(uint)], null)!;
        Assert.Equal(OnErrorDimUIntToken, mOnErrorDimUInt.MetadataToken);
        Assert.Equal(MathcadMvid, mOnErrorDimUInt.Module.ModuleVersionId);

        var qOnErrorStringDimIntIntUInt = qMatDim.GetMethod("OnError", DeclaredStatic, null, [typeof(string), Side.Quire.Dim, typeof(int), typeof(int), typeof(uint)], null)!;
        var mOnErrorStringDimIntIntUInt = mMatDim.GetMethod("OnError", DeclaredStatic, null, [typeof(string), Side.Mathcad.Dim, typeof(int), typeof(int), typeof(uint)], null)!;
        Assert.Equal(OnErrorStringDimIntIntUIntToken, mOnErrorStringDimIntIntUInt.MetadataToken);
        Assert.Equal(MathcadMvid, mOnErrorStringDimIntIntUInt.Module.ModuleVersionId);

        var qOnErrorStringDimIntUInt = qMatDim.GetMethod("OnError", DeclaredStatic, null, [typeof(string), Side.Quire.Dim, typeof(int), typeof(uint)], null)!;
        var mOnErrorStringDimIntUInt = mMatDim.GetMethod("OnError", DeclaredStatic, null, [typeof(string), Side.Mathcad.Dim, typeof(int), typeof(uint)], null)!;
        Assert.Equal(OnErrorStringDimIntUIntToken, mOnErrorStringDimIntUInt.MetadataToken);
        Assert.Equal(MathcadMvid, mOnErrorStringDimIntUInt.Module.ModuleVersionId);

        var qPickLoser = qMatDim.GetMethod("PickLoser", DeclaredStatic, null, [qMatDim, qMatDim, typeof(uint)], null)!;
        var mPickLoser = mMatDim.GetMethod("PickLoser", DeclaredStatic, null, [mMatDim, mMatDim, typeof(uint)], null)!;
        Assert.Equal(PickLoserToken, mPickLoser.MetadataToken);
        Assert.Equal(MathcadMvid, mPickLoser.Module.ModuleVersionId);

        var qEqMatDim = qMatDim.GetMethod("Eq", DeclaredStatic, null, [qMatDim, qMatDim, typeof(uint)], null)!;
        var mEqMatDim = mMatDim.GetMethod("Eq", DeclaredStatic, null, [mMatDim, mMatDim, typeof(uint)], null)!;


        // 2. Vector dimension mismatches (PickLoser directly and through Eq)
        // Vector mismatch at index 0
        object qVec0Loser = MakeRDim(Side.Quire, ["T", "L"], 1);
        object mVec0Loser = MakeRDim(Side.Mathcad, ["T", "L"], 1);
        object qVec0Winner = MakeRDim(Side.Quire, ["L", "L"], 1);
        object mVec0Winner = MakeRDim(Side.Mathcad, ["L", "L"], 1);

        MethodComparison.Compare("MatDim.PickLoser.vector.idx0", Contract,
            qPickLoser, null, [qVec0Loser, qVec0Winner, 11u],
            mPickLoser, null, [mVec0Loser, mVec0Winner, 11u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Eq.vector.idx0", Contract,
            qEqMatDim, null, [qVec0Loser, qVec0Winner, 11u],
            mEqMatDim, null, [mVec0Loser, mVec0Winner, 11u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Vector mismatch at index 1
        object qVec1Loser = MakeRDim(Side.Quire, ["L", "T", "N"], 1);
        object mVec1Loser = MakeRDim(Side.Mathcad, ["L", "T", "N"], 1);
        object qVec1Winner = MakeRDim(Side.Quire, ["L", "L", "N"], 1);
        object mVec1Winner = MakeRDim(Side.Mathcad, ["L", "L", "N"], 1);

        MethodComparison.Compare("MatDim.PickLoser.vector.idx1", Contract,
            qPickLoser, null, [qVec1Loser, qVec1Winner, 12u],
            mPickLoser, null, [mVec1Loser, mVec1Winner, 12u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Vector mismatch at index 2
        object qVec2Loser = MakeRDim(Side.Quire, ["L", "L", "T"], 1);
        object mVec2Loser = MakeRDim(Side.Mathcad, ["L", "L", "T"], 1);
        object qVec2Winner = MakeRDim(Side.Quire, ["L", "L", "L"], 1);
        object mVec2Winner = MakeRDim(Side.Mathcad, ["L", "L", "L"], 1);

        MethodComparison.Compare("MatDim.PickLoser.vector.idx2", Contract,
            qPickLoser, null, [qVec2Loser, qVec2Winner, 13u],
            mPickLoser, null, [mVec2Loser, mVec2Winner, 13u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 3. Matrix dimension mismatches (PickLoser directly and through Eq)
        // Matrix mismatch at (0, 1)
        object qMat01Loser = MakeCDim(Side.Quire, ["L", "T"], 2);
        object mMat01Loser = MakeCDim(Side.Mathcad, ["L", "T"], 2);
        object qMat01Winner = MakeCDim(Side.Quire, ["L", "L"], 2);
        object mMat01Winner = MakeCDim(Side.Mathcad, ["L", "L"], 2);

        MethodComparison.Compare("MatDim.PickLoser.matrix.r0c1", Contract,
            qPickLoser, null, [qMat01Loser, qMat01Winner, 21u],
            mPickLoser, null, [mMat01Loser, mMat01Winner, 21u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Eq.matrix.r0c1", Contract,
            qEqMatDim, null, [qMat01Loser, qMat01Winner, 21u],
            mEqMatDim, null, [mMat01Loser, mMat01Winner, 21u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Matrix mismatch at (1, 0)
        object qMat10Loser = MakeRDim(Side.Quire, ["L", "T"], 2);
        object mMat10Loser = MakeRDim(Side.Mathcad, ["L", "T"], 2);
        object qMat10Winner = MakeRDim(Side.Quire, ["L", "L"], 2);
        object mMat10Winner = MakeRDim(Side.Mathcad, ["L", "L"], 2);

        MethodComparison.Compare("MatDim.PickLoser.matrix.r1c0", Contract,
            qPickLoser, null, [qMat10Loser, qMat10Winner, 22u],
            mPickLoser, null, [mMat10Loser, mMat10Winner, 22u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 4. Final mismatch path ("array unit mismatch")
        // Matrix where element dimensions match but eq_b returns false (r_dim vs c_dim both with uniform "L" entries)
        object qMatFallthroughLoser = MakeRDim(Side.Quire, ["L", "L"], 2);
        object mMatFallthroughLoser = MakeRDim(Side.Mathcad, ["L", "L"], 2);
        object qMatFallthroughWinner = MakeCDim(Side.Quire, ["L", "L"], 2);
        object mMatFallthroughWinner = MakeCDim(Side.Mathcad, ["L", "L"], 2);

        MethodComparison.Compare("MatDim.PickLoser.matrix.fallthrough", Contract,
            qPickLoser, null, [qMatFallthroughLoser, qMatFallthroughWinner, 31u],
            mPickLoser, null, [mMatFallthroughLoser, mMatFallthroughWinner, 31u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("MatDim.Eq.matrix.fallthrough", Contract,
            qEqMatDim, null, [qMatFallthroughLoser, qMatFallthroughWinner, 31u],
            mEqMatDim, null, [mMatFallthroughLoser, mMatFallthroughWinner, 31u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // Vector where element dimensions match but eq_b returns false (r_dim(2,1) vs uniform_dim(2,1))
        object qVecFallthroughLoser = MakeRDim(Side.Quire, ["L", "L"], 1);
        object mVecFallthroughLoser = MakeRDim(Side.Mathcad, ["L", "L"], 1);
        object qVecFallthroughWinner = MakeUniform(Side.Quire, "L", 2, 1);
        object mVecFallthroughWinner = MakeUniform(Side.Mathcad, "L", 2, 1);

        MethodComparison.Compare("MatDim.PickLoser.vector.fallthrough", Contract,
            qPickLoser, null, [qVecFallthroughLoser, qVecFallthroughWinner, 32u],
            mPickLoser, null, [mVecFallthroughLoser, mVecFallthroughWinner, 32u],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));
    }
}

