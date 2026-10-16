using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCDimConstructorDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int CtorToken = 0x060007d5;
    private const int MakeToken = 0x060007d6;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private sealed record Snapshot(object Array, object?[] Items);

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");

    private static Array Dims(Side side, string?[] values)
    {
        var array = Array.CreateInstance(side.Dim, values.Length);
        for (int i = 0; i < values.Length; i++) array.SetValue(values[i] == null ? null : side.D(values[i]!), i);
        return array;
    }

    private static string ArrayText(Side side, object? value) => value is not Array array
        ? "null"
        : "[" + string.Join(",", array.Cast<object?>().Select(x => x == null ? "null" : side.DimText(x))) + "]";

    private static string ResultText(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is int) return value.ToString()!;
        if (value is Array) return ArrayText(side, value);
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" coldim={ArrayText(side, Get(value, "m_coldim"))}";
    }

    private static Snapshot Capture(Array values) => new(values, values.Cast<object?>().ToArray());

    private static void AssertUnchanged(Array values, Snapshot before)
    {
        Assert.Same(before.Array, values);
        Assert.Equal(before.Items.Length, values.Length);
        for (int i = 0; i < before.Items.Length; i++) Assert.Same(before.Items[i], values.GetValue(i));
    }

    private static ConstructorInfo Ctor(Side side)
    {
        var arrayType = Array.CreateInstance(side.Dim, 0).GetType();
        return CDim(side).GetConstructor(DeclaredInstance, null, [arrayType, typeof(int)], null)
            ?? throw new HarnessException($"Missing {side.Name} private c_dim constructor");
    }

    private static MethodInfo Make(Side side)
    {
        var arrayType = Array.CreateInstance(side.Dim, 0).GetType();
        return CDim(side).GetMethod("make", DeclaredStatic, null, [arrayType, typeof(int)], null)
            ?? throw new HarnessException($"Missing {side.Name} c_dim.make");
    }

    private static void CompareCtor(string caseId, string?[]? values, int rows)
    {
        Array? qArray = values == null ? null : Dims(Side.Quire, values);
        Array? mArray = values == null ? null : Dims(Side.Mathcad, values);
        Snapshot? qBefore = qArray == null ? null : Capture(qArray), mBefore = mArray == null ? null : Capture(mArray);
        ConstructorInfo qCtor = Ctor(Side.Quire), mCtor = Ctor(Side.Mathcad);
        Assert.Equal(CtorToken, mCtor.MetadataToken);
        object? qResult = null, mResult = null;
        MethodComparison.Compare(caseId, Contract, qCtor, null, [qArray, rows], mCtor, null, [mArray, rows],
            value => { if (value != null && CDim(Side.Quire).IsInstanceOfType(value)) qResult = value; return ResultText(Side.Quire, value); },
            value => { if (value != null && CDim(Side.Mathcad).IsInstanceOfType(value)) mResult = value; return ResultText(Side.Mathcad, value); });
        if (values == null)
        {
            Assert.Null(qResult); Assert.Null(mResult);
            return;
        }
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(CDim(Side.Quire).IsInstanceOfType(qResult)); Assert.True(CDim(Side.Mathcad).IsInstanceOfType(mResult));
        Assert.Equal(rows, Get(qResult!, "Rows")); Assert.Equal(rows, Get(mResult!, "Rows"));
        Assert.Equal(values.Length, Get(qResult!, "Cols")); Assert.Equal(values.Length, Get(mResult!, "Cols"));
        Assert.Same(qArray, Get(qResult!, "m_coldim")); Assert.Same(mArray, Get(mResult!, "m_coldim"));
        AssertUnchanged(qArray!, qBefore!); AssertUnchanged(mArray!, mBefore!);
    }

    private static void CompareMake(string caseId, string?[]? values, int rows)
    {
        Array? qArray = values == null ? null : Dims(Side.Quire, values);
        Array? mArray = values == null ? null : Dims(Side.Mathcad, values);
        Snapshot? qBefore = qArray == null ? null : Capture(qArray), mBefore = mArray == null ? null : Capture(mArray);
        MethodInfo qMake = Make(Side.Quire), mMake = Make(Side.Mathcad);
        Assert.Equal(MakeToken, mMake.MetadataToken);
        object? qResult = null, mResult = null;
        MethodComparison.Compare(caseId, Contract, qMake, null, [qArray, rows],
            mMake, null, [mArray, rows],
            value => { if (value != null && CDim(Side.Quire).IsInstanceOfType(value)) qResult = value; return ResultText(Side.Quire, value); },
            value => { if (value != null && CDim(Side.Mathcad).IsInstanceOfType(value)) mResult = value; return ResultText(Side.Mathcad, value); });
        if (values == null)
        {
            Assert.Null(qResult); Assert.Null(mResult);
            return;
        }
        Assert.NotNull(qResult); Assert.NotNull(mResult);
        Assert.True(CDim(Side.Quire).IsInstanceOfType(qResult)); Assert.True(CDim(Side.Mathcad).IsInstanceOfType(mResult));
        Assert.Equal(rows, Get(qResult!, "Rows")); Assert.Equal(rows, Get(mResult!, "Rows"));
        Assert.Equal(values.Length, Get(qResult!, "Cols")); Assert.Equal(values.Length, Get(mResult!, "Cols"));
        Assert.Same(qArray, Get(qResult!, "m_coldim")); Assert.Same(mArray, Get(mResult!, "m_coldim"));
        AssertUnchanged(qArray!, qBefore!); AssertUnchanged(mArray!, mBefore!);
    }

    [SkippableFact]
    public void C_dim_private_constructor_uses_array_length_and_retains_exact_input_array()
    {
        Runtime.Sync();
        Assert.Equal(CtorToken, Ctor(Side.Mathcad).MetadataToken);
        CompareCtor("c_dim ctor heterogeneous factors, rows 4", ["L", "T", "N"], 4);
        CompareCtor("c_dim ctor null factor and negative rows", ["L", null, "T"], -3);
        CompareCtor("c_dim ctor empty factors, zero rows", [], 0);
        CompareCtor("c_dim ctor null array throws", null!, 2);
    }

    [SkippableFact]
    public void C_dim_make_forwards_array_and_rows_to_constructor()
    {
        Runtime.Sync();
        Assert.Equal(MakeToken, Make(Side.Mathcad).MetadataToken);
        CompareMake("c_dim.make heterogeneous factors, rows 5", ["N", "T", "L", "v"], 5);
        CompareMake("c_dim.make empty factors, negative rows", [], -2);
        CompareMake("c_dim.make singleton factor, zero rows", ["L"], 0);
        CompareMake("c_dim.make null array throws", null, 3);
    }
}
