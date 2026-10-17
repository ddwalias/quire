using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimArithmeticDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int MMult1Token = 0x060007bc;
    private const int MMult2CDimToken = 0x060007bd;
    private const int MMult2RcDimToken = 0x060007be;
    private const int TransposeToken = 0x060007bf;
    private const int DetToken = 0x060007c0;
    private const int InvertToken = 0x060007c1;
    private const int PwrToken = 0x060007c2;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");
    private static Type RcDim(Side side) => Nested(MatDim(side), "rc_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static MethodInfo MMult1(Side side) =>
        RcDim(side).GetMethod("mmult1", DeclaredInstance, null, [MatDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.mmult1(MatDim)");

    private static MethodInfo MMult2CDim(Side side) =>
        RcDim(side).GetMethod("mmult2", DeclaredInstance, null, [CDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.mmult2(c_dim)");

    private static MethodInfo MMult2RcDim(Side side) =>
        RcDim(side).GetMethod("mmult2", DeclaredInstance, null, [RcDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.mmult2(rc_dim)");

    private static MethodInfo Transpose(Side side) =>
        RcDim(side).GetMethod("transpose", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.transpose()");

    private static MethodInfo Det(Side side) =>
        RcDim(side).GetMethod("det", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.det(int)");

    private static MethodInfo Invert(Side side) =>
        RcDim(side).GetMethod("invert", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.invert()");

    private static MethodInfo Pwr(Side side) =>
        RcDim(side).GetMethod("pwr", DeclaredInstance, null, [typeof(uint)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.pwr(uint)");

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
    public void Rc_dim_arithmetic_operations_match_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(MMult1Token, MMult1(Side.Mathcad).MetadataToken);
        Assert.Equal(MMult2CDimToken, MMult2CDim(Side.Mathcad).MetadataToken);
        Assert.Equal(MMult2RcDimToken, MMult2RcDim(Side.Mathcad).MetadataToken);
        Assert.Equal(TransposeToken, Transpose(Side.Mathcad).MetadataToken);
        Assert.Equal(DetToken, Det(Side.Mathcad).MetadataToken);
        Assert.Equal(InvertToken, Invert(Side.Mathcad).MetadataToken);
        Assert.Equal(PwrToken, Pwr(Side.Mathcad).MetadataToken);

        string[] rows2 = ["L", "T"];
        string[] cols3 = ["identity", "v", "N"];
        string[] cols2 = ["identity", "v"];
        string[] rows3 = ["L", "T", "N"];
        string[] singleton = ["L"];
        string[] empty = [];

        // 1. mmult1(MatDim m2) -> dispatches to m2.mmult2(this)
        void CompareMMult1(string caseId, string[] rNames, string[] cNames, Func<Side, object?> getArg)
        {
            object qTarget = MakeRcDim(Side.Quire, rNames, cNames);
            object mTarget = MakeRcDim(Side.Mathcad, rNames, cNames);
            object? qArg = getArg(Side.Quire);
            object? mArg = getArg(Side.Mathcad);

            MethodComparison.Compare(caseId, Contract,
                MMult1(Side.Quire), qTarget, [qArg],
                MMult1(Side.Mathcad), mTarget, [mArg],
                quireShow: v => Show(Side.Quire, v),
                originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t),
                originalState: t => State(Side.Mathcad, t));
        }

        // r_dim.mmult2(rc_dim) supported
        CompareMMult1("mmult1 dispatching to r_dim with compatible factor", rows2, cols3, side => MakeRDim(side, ["L", "T"], 2));
        CompareMMult1("mmult1 dispatching to r_dim with incompatible factor returns null", rows2, cols3, side => MakeRDim(side, ["L", "N"], 2));
        // c_dim.mmult2(rc_dim) unsupported -> returns null
        CompareMMult1("mmult1 dispatching to c_dim returns null", rows2, cols3, side => MakeCDim(side, ["identity", "v"], 3));
        // uniform_dim.mmult2(rc_dim) unsupported -> returns null
        CompareMMult1("mmult1 dispatching to uniform_dim returns null", rows2, cols3, side => MakeUniform(side, "L", 2, 2));
        // crazy_dim.mmult2(rc_dim) unsupported -> returns null
        CompareMMult1("mmult1 dispatching to crazy_dim returns null", rows2, cols3, MakeCrazy);
        // null m2 throws NullReferenceException
        CompareMMult1("mmult1 with null throws NullReferenceException", rows2, cols3, _ => null);

        // 2. mmult2(c_dim m1)
        void CompareMMult2CDim(string caseId, string[] rNames, string[] cNames, Func<Side, object?> getArg)
        {
            object qTarget = MakeRcDim(Side.Quire, rNames, cNames);
            object mTarget = MakeRcDim(Side.Mathcad, rNames, cNames);
            object? qArg = getArg(Side.Quire);
            object? mArg = getArg(Side.Mathcad);

            MethodComparison.Compare(caseId, Contract,
                MMult2CDim(Side.Quire), qTarget, [qArg],
                MMult2CDim(Side.Mathcad), mTarget, [mArg],
                quireShow: v => Show(Side.Quire, v),
                originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t),
                originalState: t => State(Side.Mathcad, t));
        }

        // Mult(m1.m_coldim, m_rowdim): m1 has coldim, this has rowdim
        // Compatible factor: m1.m_coldim has 2 dims, this.m_rowdim has 2 dims
        // e.g. m1 cols = ["L", "L"], this rows = ["T", "T"] -> factor L*T
        CompareMMult2CDim("mmult2(c_dim) with compatible factor", ["T", "T"], cols3, side => MakeCDim(side, ["L", "L"], 4));
        CompareMMult2CDim("mmult2(c_dim) with incompatible factor returns null", ["T", "N"], cols3, side => MakeCDim(side, ["L", "L"], 4));
        CompareMMult2CDim("mmult2(c_dim) with length mismatch returns null", rows2, cols3, side => MakeCDim(side, ["L", "T", "N"], 4));
        CompareMMult2CDim("mmult2(c_dim) on singleton rc_dim", singleton, cols3, side => MakeCDim(side, ["T"], 2));
        CompareMMult2CDim("mmult2(c_dim) on empty rc_dim", empty, empty, side => MakeCDim(side, [], 0));
        CompareMMult2CDim("mmult2(c_dim) with null m1 throws NullReferenceException", rows2, cols3, _ => null);

        // 3. mmult2(rc_dim m1)
        void CompareMMult2RcDim(string caseId, string[] rNames, string[] cNames, Func<Side, object?> getArg)
        {
            object qTarget = MakeRcDim(Side.Quire, rNames, cNames);
            object mTarget = MakeRcDim(Side.Mathcad, rNames, cNames);
            object? qArg = getArg(Side.Quire);
            object? mArg = getArg(Side.Mathcad);

            MethodComparison.Compare(caseId, Contract,
                MMult2RcDim(Side.Quire), qTarget, [qArg],
                MMult2RcDim(Side.Mathcad), mTarget, [mArg],
                quireShow: v => Show(Side.Quire, v),
                originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t),
                originalState: t => State(Side.Mathcad, t));
        }

        // Mult(m1.m_coldim, m_rowdim)
        CompareMMult2RcDim("mmult2(rc_dim) with compatible factor", ["T", "T"], cols3, side => MakeRcDim(side, ["N", "N"], ["L", "L"]));
        CompareMMult2RcDim("mmult2(rc_dim) with incompatible factor returns null", ["T", "N"], cols3, side => MakeRcDim(side, ["N", "N"], ["L", "L"]));
        CompareMMult2RcDim("mmult2(rc_dim) with length mismatch returns null", rows2, cols3, side => MakeRcDim(side, ["N"], ["L", "T", "N"]));
        CompareMMult2RcDim("mmult2(rc_dim) on singleton rc_dim", singleton, cols3, side => MakeRcDim(side, ["N"], ["T"]));
        CompareMMult2RcDim("mmult2(rc_dim) on empty rc_dim", empty, empty, side => MakeRcDim(side, [], []));
        CompareMMult2RcDim("mmult2(rc_dim) with null m1 throws NullReferenceException", rows2, cols3, _ => null);

        // 4. transpose()
        void CompareTranspose(string caseId, string[] rNames, string[] cNames)
        {
            object qTarget = MakeRcDim(Side.Quire, rNames, cNames);
            object mTarget = MakeRcDim(Side.Mathcad, rNames, cNames);
            MethodComparison.Compare(caseId, Contract,
                Transpose(Side.Quire), qTarget, [],
                Transpose(Side.Mathcad), mTarget, [],
                quireShow: v => Show(Side.Quire, v),
                originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t),
                originalState: t => State(Side.Mathcad, t));
        }

        CompareTranspose("transpose on 2x3 rc_dim", rows2, cols3);
        CompareTranspose("transpose on square 2x2 rc_dim", rows2, cols2);
        CompareTranspose("transpose on singleton rc_dim", singleton, singleton);
        CompareTranspose("transpose on empty rc_dim", empty, empty);

        // 5. det(int n)
        void CompareDet(string caseId, string[] rNames, string[] cNames, int n)
        {
            object qTarget = MakeRcDim(Side.Quire, rNames, cNames);
            object mTarget = MakeRcDim(Side.Mathcad, rNames, cNames);
            MethodComparison.Compare(caseId, Contract,
                Det(Side.Quire), qTarget, [n],
                Det(Side.Mathcad), mTarget, [n],
                quireShow: v => Show(Side.Quire, v),
                originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t),
                originalState: t => State(Side.Mathcad, t));
        }

        CompareDet("det on 2x2 rc_dim with n=2", rows2, cols2, 2);
        CompareDet("det on 2x2 rc_dim with n=0", rows2, cols2, 0);
        CompareDet("det on 2x2 rc_dim with n=-1", rows2, cols2, -1);
        CompareDet("det on 2x3 rc_dim with n=2", rows2, cols3, 2);
        CompareDet("det on singleton rc_dim with n=1", singleton, singleton, 1);
        CompareDet("det on empty rc_dim with n=0", empty, empty, 0);

        // 6. invert()
        void CompareInvert(string caseId, string[] rNames, string[] cNames)
        {
            object qTarget = MakeRcDim(Side.Quire, rNames, cNames);
            object mTarget = MakeRcDim(Side.Mathcad, rNames, cNames);
            MethodComparison.Compare(caseId, Contract,
                Invert(Side.Quire), qTarget, [],
                Invert(Side.Mathcad), mTarget, [],
                quireShow: v => Show(Side.Quire, v),
                originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t),
                originalState: t => State(Side.Mathcad, t));
        }

        CompareInvert("invert on 2x3 rc_dim", rows2, cols3);
        CompareInvert("invert on square 2x2 rc_dim", rows2, cols2);
        CompareInvert("invert on singleton rc_dim", singleton, singleton);
        CompareInvert("invert on empty rc_dim", empty, empty);

        // 7. pwr(uint n)
        void ComparePwr(string caseId, string[] rNames, string[] cNames, uint n)
        {
            object qTarget = MakeRcDim(Side.Quire, rNames, cNames);
            object mTarget = MakeRcDim(Side.Mathcad, rNames, cNames);
            MethodComparison.Compare(caseId, Contract,
                Pwr(Side.Quire), qTarget, [n],
                Pwr(Side.Mathcad), mTarget, [n],
                quireShow: v => Show(Side.Quire, v),
                originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t),
                originalState: t => State(Side.Mathcad, t));
        }

        // Mult(m_coldim, m_rowdim)
        // Compatible factor: coldim = ["L", "L"], rowdim = ["T", "T"] -> factor L*T
        ComparePwr("pwr with compatible factors n=1", ["T", "T"], ["L", "L"], 1u);
        ComparePwr("pwr with compatible factors n=2", ["T", "T"], ["L", "L"], 2u);
        ComparePwr("pwr with compatible factors n=3", ["T", "T"], ["L", "L"], 3u);
        ComparePwr("pwr with compatible factors n=0", ["T", "T"], ["L", "L"], 0u);
        ComparePwr("pwr with incompatible factors returns null", ["T", "N"], ["L", "L"], 2u);
        ComparePwr("pwr with dimension shape mismatch returns null", rows2, cols3, 2u);
        ComparePwr("pwr on singleton rc_dim n=2", singleton, singleton, 2u);
        ComparePwr("pwr on empty rc_dim n=1", empty, empty, 1u);
    }
}
