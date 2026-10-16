using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimSpecializedDimensionArraysDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const string OriginalMvid = "17a0933b-4988-4c18-963d-563998321cc8";
    private const int RowDimensionsToken = 0x060007c8;
    private const int ColDimensionsToken = 0x060007da;
    private const BindingFlags DeclaredInstance = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Target, object Argument, object StoredArray, object?[] Values);

    private static Array Dimensions(Side side, params string[] names)
    {
        var values = Array.CreateInstance(side.Dim, names.Length);
        for (int index = 0; index < names.Length; index++) values.SetValue(side.D(names[index]), index);
        return values;
    }

    private static object Make(Side side, string nested, Array dimensions, int invariantSize) =>
        CallSig(Nested(side.T("Mcd.MatDim"), nested), "make", ["Dim[]", "Int32"], dimensions, invariantSize)!;

    private static MethodInfo Method(Side side, string owner, string name) =>
        Nested(side.T("Mcd.MatDim"), owner).GetMethod(name, DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{owner}.{name}()");

    private static object?[] CaptureValues(object array) => ((Array)array).Cast<object?>().ToArray();
    private static string Ref(object? before, object? after) => ReferenceEquals(before, after) ? "same" : "changed";

    private static string DescribeValues(Side side, IEnumerable<object?> values, IReadOnlyList<object?> before)
    {
        return string.Join(";", values.Select((value, index) =>
        {
            string aliases = string.Join(",", before.Select((oldValue, oldIndex) =>
                ReferenceEquals(value, oldValue) ? $"input[{oldIndex}]" : null).Where(alias => alias != null));
            return $"{index}:{side.DimText(value)}:{(aliases.Length == 0 ? "other-ref" : aliases)}";
        }));
    }

    private static string State(Side side, Snapshot before, object target, string field)
    {
        object current = Get(target, field)!;
        object retained = before.StoredArray;
        int rows = (int)Get(target, "Rows")!;
        int cols = (int)Get(target, "Cols")!;
        return $"{target.GetType().Name}:{rows}x{cols}:argument-ref={Ref(before.Argument, Get(target, field))}" +
            $" field-ref={Ref(retained, current)} field=[{DescribeValues(side, CaptureValues(current), before.Values)}]" +
            $" retained-before-array-after=[{DescribeValues(side, CaptureValues(retained), before.Values)}]";
    }

    private static string Result(Side side, object? result, Snapshot before)
    {
        if (result is not Array array) return Text(result);
        return $"{result.GetType().Name}:return-argument={ReferenceEquals(result, before.Argument)}" +
            $":return-stored={ReferenceEquals(result, before.StoredArray)}:[{DescribeValues(side, array.Cast<object?>(), before.Values)}]";
    }

    private static void ComparePair(string caseId, string owner, string getter, string field, int token,
        object qTarget, object mTarget, object qArgument, object mArgument)
    {
        var qMethod = Method(Side.Quire, owner, getter);
        var mMethod = Method(Side.Mathcad, owner, getter);
        Assert.Equal(token, mMethod.MetadataToken);
        Assert.Equal(OriginalMvid, mMethod.Module.ModuleVersionId.ToString());
        var qBefore = new Snapshot(qTarget, qArgument, Get(qTarget, field)!, CaptureValues(Get(qTarget, field)!));
        var mBefore = new Snapshot(mTarget, mArgument, Get(mTarget, field)!, CaptureValues(Get(mTarget, field)!));
        MethodComparison.Compare(caseId, Contract, qMethod, qTarget, [], mMethod, mTarget, [],
            result => Result(Side.Quire, result, qBefore), result => Result(Side.Mathcad, result, mBefore),
            target => State(Side.Quire, qBefore, target, field), target => State(Side.Mathcad, mBefore, target, field));
    }

    [SkippableFact]
    public void RDim_get_RowDimensions_returns_the_stored_array_reference()
    {
        Runtime.Sync();
        var qValues = Dimensions(Side.Quire, "T", "L", "poly", "N");
        var mValues = Dimensions(Side.Mathcad, "T", "L", "poly", "N");
        var q = Make(Side.Quire, "r_dim", qValues, 3);
        var m = Make(Side.Mathcad, "r_dim", mValues, 3);
        ComparePair("r_dim.get_RowDimensions returns the supplied array in order", "r_dim", "get_RowDimensions", "m_rowdim",
            RowDimensionsToken, q, m, qValues, mValues);

        qValues = Dimensions(Side.Quire);
        mValues = Dimensions(Side.Mathcad);
        q = Make(Side.Quire, "r_dim", qValues, 2);
        m = Make(Side.Mathcad, "r_dim", mValues, 2);
        ComparePair("r_dim.get_RowDimensions returns the retained empty array", "r_dim", "get_RowDimensions", "m_rowdim",
            RowDimensionsToken, q, m, qValues, mValues);
    }

    [SkippableFact]
    public void CDim_get_ColDimensions_returns_the_stored_array_reference()
    {
        Runtime.Sync();
        var qValues = Dimensions(Side.Quire, "N", "poly", "T", "L");
        var mValues = Dimensions(Side.Mathcad, "N", "poly", "T", "L");
        var q = Make(Side.Quire, "c_dim", qValues, 3);
        var m = Make(Side.Mathcad, "c_dim", mValues, 3);
        ComparePair("c_dim.get_ColDimensions returns the supplied array in order", "c_dim", "get_ColDimensions", "m_coldim",
            ColDimensionsToken, q, m, qValues, mValues);

        qValues = Dimensions(Side.Quire);
        mValues = Dimensions(Side.Mathcad);
        q = Make(Side.Quire, "c_dim", qValues, 2);
        m = Make(Side.Mathcad, "c_dim", mValues, 2);
        ComparePair("c_dim.get_ColDimensions returns the retained empty array", "c_dim", "get_ColDimensions", "m_coldim",
            ColDimensionsToken, q, m, qValues, mValues);
    }
}
