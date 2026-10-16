using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimEqualityDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int EqToken = 0x060007ba;
    private const int Eq2Token = 0x060007bb;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record RcSnapshot(object Value, Array Rows, Array Cols,
        IReadOnlyList<object?> RowItems, IReadOnlyList<object?> ColItems, int RowCount, int ColCount);

    private static System.Type Rc(Side side) => Nested(side.T("Mcd.MatDim"), "rc_dim");
    private static System.Type Uniform(Side side) => Nested(side.T("Mcd.MatDim"), "uniform_dim");

    private static Array Dims(Side side, params string[] names)
    {
        var array = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) array.SetValue(side.D(names[i]), i);
        return array;
    }

    private static object MakeRc(Side side, Array rows, Array cols)
    {
        var method = Rc(side).GetMethod("make_rc", DeclaredStatic, null,
            [side.Dim.MakeArrayType(), side.Dim.MakeArrayType()], null)
            ?? throw new HarnessException($"Missing {side.Name} rc_dim.make_rc(Dim[],Dim[])");
        return method.Invoke(null, [rows, cols])!;
    }

    private static object MakeUniform(Side side, string dimension, int rows = 1, int cols = 1)
    {
        var method = Uniform(side).GetMethod("make", DeclaredStatic, null,
            [side.Dim, typeof(int), typeof(int)], null)
            ?? throw new HarnessException($"Missing {side.Name} uniform_dim.make(Dim,int,int)");
        return method.Invoke(null, [side.D(dimension), rows, cols])!;
    }

    private static MethodInfo Method(Side side, string name)
    {
        System.Type mapped = name == "eq_b" ? side.T("Mcd.MatDim") : Rc(side);
        return Rc(side).GetMethod(name, DeclaredInstance, null, [mapped], null)
            ?? throw new HarnessException($"Missing {side.Name} rc_dim.{name}");
    }

    private static string DimArray(Side side, object? value) => value is Array array
        ? $"[{string.Join(",", array.Cast<object?>().Select(side.DimText))}]"
        : "null";

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is bool) return side.Show(value);
        if (value.GetType().Name == "rc_dim")
            return $"rc_dim({Get(value, "Rows")}x{Get(value, "Cols")};rows={DimArray(side, Get(value, "m_rowdim"))};cols={DimArray(side, Get(value, "m_coldim"))})";
        if (value.GetType().Name == "uniform_dim")
            return $"uniform_dim({Get(value, "Rows")}x{Get(value, "Cols")};dim={side.DimText(Get(value, "m_dim"))})";
        return side.Show(value);
    }

    private static string State(Side side, object? value) => Show(side, value);

    private static RcSnapshot Capture(object value)
    {
        var rows = (Array)Get(value, "m_rowdim")!;
        var cols = (Array)Get(value, "m_coldim")!;
        return new RcSnapshot(value, rows, cols, rows.Cast<object?>().ToArray(), cols.Cast<object?>().ToArray(),
            (int)Get(value, "Rows")!, (int)Get(value, "Cols")!);
    }

    private static void AssertUnchanged(RcSnapshot before)
    {
        Assert.Same(before.Rows, Get(before.Value, "m_rowdim"));
        Assert.Same(before.Cols, Get(before.Value, "m_coldim"));
        Assert.Equal(before.RowCount, Get(before.Value, "Rows"));
        Assert.Equal(before.ColCount, Get(before.Value, "Cols"));
        for (int i = 0; i < before.Rows.Length; i++) Assert.Same(before.RowItems[i], before.Rows.GetValue(i));
        for (int i = 0; i < before.Cols.Length; i++) Assert.Same(before.ColItems[i], before.Cols.GetValue(i));
    }

    private static void Compare(string caseId, string methodName, int token,
        object? qLeft, object? qRight, object? mLeft, object? mRight)
    {
        var qMethod = Method(Side.Quire, methodName);
        var mMethod = Method(Side.Mathcad, methodName);
        Assert.Equal(token, mMethod.MetadataToken);
        RcSnapshot? qLeftBefore = qLeft == null ? null : Capture(qLeft);
        RcSnapshot? qRightBefore = qRight?.GetType().Name == "rc_dim" ? Capture(qRight) : null;
        RcSnapshot? mLeftBefore = mLeft == null ? null : Capture(mLeft);
        RcSnapshot? mRightBefore = mRight?.GetType().Name == "rc_dim" ? Capture(mRight) : null;
        string qRightBeforeText = Show(Side.Quire, qRight), mRightBeforeText = Show(Side.Mathcad, mRight);
        MethodComparison.Compare(caseId, Contract, qMethod, qLeft, [qRight], mMethod, mLeft, [mRight],
            quireShow: value => Show(Side.Quire, value), originalShow: value => Show(Side.Mathcad, value),
            quireState: target => State(Side.Quire, target), originalState: target => State(Side.Mathcad, target));
        Assert.Equal(qRightBeforeText, Show(Side.Quire, qRight));
        Assert.Equal(mRightBeforeText, Show(Side.Mathcad, mRight));
        if (qLeftBefore != null) AssertUnchanged(qLeftBefore);
        if (qRightBefore != null) AssertUnchanged(qRightBefore);
        if (mLeftBefore != null) AssertUnchanged(mLeftBefore);
        if (mRightBefore != null) AssertUnchanged(mRightBefore);
    }

    private static void CompareBoth(string caseId, string[] rowLeft, string[] colLeft,
        string[] rowRight, string[] colRight)
    {
        object qLeft = MakeRc(Side.Quire, Dims(Side.Quire, rowLeft), Dims(Side.Quire, colLeft));
        object qRight = MakeRc(Side.Quire, Dims(Side.Quire, rowRight), Dims(Side.Quire, colRight));
        object mLeft = MakeRc(Side.Mathcad, Dims(Side.Mathcad, rowLeft), Dims(Side.Mathcad, colLeft));
        object mRight = MakeRc(Side.Mathcad, Dims(Side.Mathcad, rowRight), Dims(Side.Mathcad, colRight));
        Compare(caseId + " via eq_b", "eq_b", EqToken, qLeft, qRight, mLeft, mRight);
        Compare(caseId + " via eq2_b", "eq2_b", Eq2Token, qLeft, qRight, mLeft, mRight);
    }

    [SkippableFact]
    public void Rc_dimension_equality_double_dispatch_compares_each_axis_and_preserves_inputs()
    {
        Runtime.Sync();

        CompareBoth("equal heterogeneous row and column factors", ["L", "T"], ["v", "identity"],
            ["L", "T"], ["v", "identity"]);
        CompareBoth("row factor mismatch", ["L", "T"], ["v", "identity"],
            ["L", "N"], ["v", "identity"]);
        CompareBoth("column factor mismatch", ["L", "T"], ["v", "identity"],
            ["L", "T"], ["v", "N"]);
        CompareBoth("row factor length mismatch", ["L", "T"], ["v", "identity"],
            ["L", "T", "v"], ["v", "identity"]);
        CompareBoth("column factor length mismatch", ["L", "T"], ["v", "identity"],
            ["L", "T"], ["v"]);

        var qRc = MakeRc(Side.Quire, Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "v"));
        var mRc = MakeRc(Side.Mathcad, Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "v"));
        Compare("non-rc uniform RHS false double-dispatch", "eq_b", EqToken,
            qRc, MakeUniform(Side.Quire, "L", 2, 1), mRc, MakeUniform(Side.Mathcad, "L", 2, 1));

        var qNullRc = MakeRc(Side.Quire, Dims(Side.Quire, "L"), Dims(Side.Quire, "T"));
        var mNullRc = MakeRc(Side.Mathcad, Dims(Side.Mathcad, "L"), Dims(Side.Mathcad, "T"));
        Compare("eq_b null RHS dispatch throws", "eq_b", EqToken, qNullRc, null, mNullRc, null);
        Compare("eq2_b null RHS shape read throws", "eq2_b", Eq2Token, qNullRc, null, mNullRc, null);
    }
}
