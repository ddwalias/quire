using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRDimConstructorDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int ConstructorToken = 0x060007c3;
    private const int FactoryToken = 0x060007c4;
    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type RDim(Side side) => Nested(side.T("Mcd.MatDim"), "r_dim");

    private static ConstructorInfo Constructor(Side side) =>
        RDim(side).GetConstructor(DeclaredInstance, null, [side.Dim.MakeArrayType(), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim..ctor(Dim[], int)");

    private static MethodInfo Factory(Side side) =>
        RDim(side).GetMethod("make", DeclaredStatic, null, [side.Dim.MakeArrayType(), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.make(Dim[], int)");

    private static Array Dims(Side side, params string?[] names)
    {
        var result = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) result.SetValue(names[i] == null ? null : side.D(names[i]!), i);
        return result;
    }

    private static object[] Elements(object array) => ((Array)array).Cast<object?>().Select(x => x!).ToArray();

    private static string ArrayText(Side side, object? value) => value is Array array
        ? "[" + string.Join(",", array.Cast<object?>().Select(side.DimText)) + "]"
        : "null";

    private static string ResultText(Side side, object? value, object? input)
    {
        if (value == null) return "null";
        return $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
            $" rows-alias-input={ReferenceEquals(Get(value, "m_rowdim"), input)}" +
            $" dims={ArrayText(side, Get(value, "m_rowdim"))}";
    }

    private static void AssertCreated(Side side, object result, object input, int cols, object?[] expectedItems)
    {
        Assert.True(RDim(side).IsInstanceOfType(result));
        Assert.Equal(expectedItems.Length, Get(result, "Rows"));
        Assert.Equal(cols, Get(result, "Cols"));
        Assert.Same(input, Get(result, "m_rowdim"));
        for (int i = 0; i < expectedItems.Length; i++) Assert.Same(expectedItems[i], ((Array)input).GetValue(i));
    }

    private static void CompareConstructor(string caseId, string?[]? names, int cols)
    {
        Array? qInput = names == null ? null : Dims(Side.Quire, names);
        Array? mInput = names == null ? null : Dims(Side.Mathcad, names);
        object?[] qItems = qInput == null ? [] : Elements(qInput);
        object?[] mItems = mInput == null ? [] : Elements(mInput);
        var qConstructor = Constructor(Side.Quire);
        var mConstructor = Constructor(Side.Mathcad);
        Assert.Equal(ConstructorToken, mConstructor.MetadataToken);
        object? qResult = null, mResult = null;

        MethodComparison.Compare(caseId, Contract, qConstructor, null, [qInput, cols], mConstructor, null, [mInput, cols],
            value =>
            {
                if (value != null && value is not Array && value is not int) qResult = value;
                return value is Array ? ArrayText(Side.Quire, value) : value is int ? Text(value) : ResultText(Side.Quire, value, qInput);
            },
            value =>
            {
                if (value != null && value is not Array && value is not int) mResult = value;
                return value is Array ? ArrayText(Side.Mathcad, value) : value is int ? Text(value) : ResultText(Side.Mathcad, value, mInput);
            });

        if (names == null)
        {
            Assert.Null(qResult); Assert.Null(mResult);
            return;
        }
        Assert.NotNull(qInput); Assert.NotNull(mInput); Assert.NotNull(qResult); Assert.NotNull(mResult);
        AssertCreated(Side.Quire, qResult!, qInput!, cols, qItems);
        AssertCreated(Side.Mathcad, mResult!, mInput!, cols, mItems);
    }

    private static void CompareFactory(string caseId, string?[]? names, int cols)
    {
        Array? qInput = names == null ? null : Dims(Side.Quire, names);
        Array? mInput = names == null ? null : Dims(Side.Mathcad, names);
        object?[] qItems = qInput == null ? [] : Elements(qInput);
        object?[] mItems = mInput == null ? [] : Elements(mInput);
        var qFactory = Factory(Side.Quire);
        var mFactory = Factory(Side.Mathcad);
        Assert.Equal(FactoryToken, mFactory.MetadataToken);
        object? qResult = null, mResult = null;

        MethodComparison.Compare(caseId, Contract, qFactory, null, [qInput, cols], mFactory, null, [mInput, cols],
            value =>
            {
                if (value != null && !(value is Array) && !(value is int)) qResult = value;
                return value is Array ? ArrayText(Side.Quire, value) : value is int ? Text(value) : ResultText(Side.Quire, value, qInput);
            },
            value =>
            {
                if (value != null && !(value is Array) && !(value is int)) mResult = value;
                return value is Array ? ArrayText(Side.Mathcad, value) : value is int ? Text(value) : ResultText(Side.Mathcad, value, mInput);
            });

        if (names == null)
        {
            Assert.Null(qResult); Assert.Null(mResult);
            return;
        }
        Assert.NotNull(qInput); Assert.NotNull(mInput); Assert.NotNull(qResult); Assert.NotNull(mResult);
        AssertCreated(Side.Quire, qResult!, qInput!, cols, qItems);
        AssertCreated(Side.Mathcad, mResult!, mInput!, cols, mItems);
    }

    [SkippableFact]
    public void R_dim_constructor_retains_the_exact_array_and_uses_its_length_for_rows()
    {
        Runtime.Sync();
        Assert.Equal(ConstructorToken, Constructor(Side.Mathcad).MetadataToken);

        CompareConstructor("r_dim constructor heterogeneous dimensions and positive columns", ["L", "T"], 3);
        CompareConstructor("r_dim constructor null entries and negative columns", ["identity", null, "N"], -2);
        CompareConstructor("r_dim constructor empty dimensions and zero columns", [], 0);
        CompareConstructor("r_dim constructor null array throws before base construction", null, 3);
    }

    [SkippableFact]
    public void R_dim_factory_forwards_the_same_array_and_preserves_constructor_behavior()
    {
        Runtime.Sync();
        Assert.Equal(FactoryToken, Factory(Side.Mathcad).MetadataToken);

        CompareFactory("r_dim factory heterogeneous dimensions and positive columns", ["L", "T"], 3);
        CompareFactory("r_dim factory null entries and negative columns", ["identity", null, "N"], -2);
        CompareFactory("r_dim factory empty dimensions and zero columns", [], 0);
        CompareFactory("r_dim factory null array propagates constructor exception", null, 3);
    }
}
