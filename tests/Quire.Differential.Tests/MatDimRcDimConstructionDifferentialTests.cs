using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimConstructionDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int ConstructorToken = 0x060007b3;
    private const int FactoryToken = 0x060007b4;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static System.Type Rc(Side side) => Nested(side.T("Mcd.MatDim"), "rc_dim");

    private static ConstructorInfo Constructor(Side side) =>
        Rc(side).GetConstructor(DeclaredInstance, null, [side.Dim.MakeArrayType(), side.Dim.MakeArrayType()], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim..ctor(Dim[],Dim[])");

    private static MethodInfo Factory(Side side) =>
        Rc(side).GetMethod("make_rc", DeclaredStatic, null,
            [side.Dim.MakeArrayType(), side.Dim.MakeArrayType()], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.make_rc(Dim[],Dim[])");

    private static Array Dims(Side side, params string[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(side.D(names[i]), i);
        return result;
    }

    private static string DimArrayState(Side side, object? value) => value is Array array
        ? $"Dim[{array.Length}]({string.Join(",", array.Cast<object?>().Select(side.DimText))})"
        : "null";

    private static string Show(Side side, object? value, object? rows, object? cols)
    {
        if (value == null) return "null";
        if (value.GetType().Name != "rc_dim") return DimArrayState(side, value);
        Assert.Equal(Rc(side), value.GetType());
        Assert.Equal(((Array)rows!).Length, Get(value, "Rows"));
        Assert.Equal(((Array)cols!).Length, Get(value, "Cols"));
        Assert.Same(rows, Get(value, "m_rowdim"));
        Assert.Same(cols, Get(value, "m_coldim"));
        return $"rc_dim({Get(value, "Rows")}x{Get(value, "Cols")};rows-ref={ReferenceEquals(rows, Get(value, "m_rowdim"))}:" +
            $"{DimArrayState(side, Get(value, "m_rowdim"))};cols-ref={ReferenceEquals(cols, Get(value, "m_coldim"))}:" +
            $"{DimArrayState(side, Get(value, "m_coldim"))})";
    }

    private static object?[] Capture(object? value) => value is Array array
        ? array.Cast<object?>().ToArray()
        : [];

    private static void AssertUnchanged(object? array, IReadOnlyList<object?> before)
    {
        if (array is not Array values) return;
        Assert.Equal(before.Count, values.Length);
        for (int i = 0; i < values.Length; i++) Assert.Same(before[i], values.GetValue(i));
    }

    private static void CompareOne(string caseId, object? qRows, object? qCols, object? mRows, object? mCols,
        MethodBase qMethod, MethodBase mMethod, int token)
    {
        Assert.Equal(token, mMethod.MetadataToken);
        var qRowsBefore = Capture(qRows); var qColsBefore = Capture(qCols);
        var mRowsBefore = Capture(mRows); var mColsBefore = Capture(mCols);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qRows, qCols],
            mMethod, null, [mRows, mCols],
            quireShow: value => Show(Side.Quire, value, qRows, qCols),
            originalShow: value => Show(Side.Mathcad, value, mRows, mCols));
        AssertUnchanged(qRows, qRowsBefore); AssertUnchanged(qCols, qColsBefore);
        AssertUnchanged(mRows, mRowsBefore); AssertUnchanged(mCols, mColsBefore);
    }

    private static void ComparePair(string caseId, string[]? rowNames, string[]? colNames)
    {
        Array? qRows = rowNames == null ? null : Dims(Side.Quire, rowNames);
        Array? qCols = colNames == null ? null : Dims(Side.Quire, colNames);
        Array? mRows = rowNames == null ? null : Dims(Side.Mathcad, rowNames);
        Array? mCols = colNames == null ? null : Dims(Side.Mathcad, colNames);
        CompareOne(caseId + " private constructor", qRows, qCols, mRows, mCols,
            Constructor(Side.Quire), Constructor(Side.Mathcad), ConstructorToken);
        CompareOne(caseId + " public make_rc factory", qRows, qCols, mRows, mCols,
            Factory(Side.Quire), Factory(Side.Mathcad), FactoryToken);
    }

    [SkippableFact]
    public void Rc_dimension_construction_preserves_axes_order_identity_shape_and_null_failures()
    {
        Runtime.Sync();

        ComparePair("heterogeneous nontrivial row and column dimensions preserve identity and order",
            ["L", "T", "identity"], ["v", "poly"]);
        ComparePair("zero rows with non-empty columns", [], ["T", "L"]);
        ComparePair("non-empty rows with zero columns", ["v", "identity", "T"], []);
        ComparePair("both axes empty", [], []);
        ComparePair("null row array throws while reading row length", null, ["T", "v"]);
        ComparePair("null column array throws while reading column length", ["L", "identity"], null);
    }
}
