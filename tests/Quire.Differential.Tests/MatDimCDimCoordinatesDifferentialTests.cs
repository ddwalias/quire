using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimCDimCoordinatesDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007d7;
    private const int MatDimToken = 0x060007d8;
    private const int IsLinearToken = 0x060007d9;
    private const int ColDimensionsToken = 0x060007da;
    private const int RowDimensionArrayToken = 0x060007db;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type CDim(Side side) => Nested(MatDim(side), "c_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static MethodInfo MakeMethod(Side side) =>
        CDim(side).GetMethod("make", DeclaredStatic, null, [side.Dim.MakeArrayType(), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.make(Dim[], int)");

    private static MethodInfo DimMethod(Side side) =>
        CDim(side).GetMethod("dim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.dim(int, int)");

    private static MethodInfo MatDimMethod(Side side) =>
        CDim(side).GetMethod("matdim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.matdim(int, int)");

    private static MethodInfo IsLinearGetter(Side side) =>
        CDim(side).GetMethod("get_IsLinear", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.get_IsLinear()");

    private static MethodInfo ColDimensionsGetter(Side side) =>
        CDim(side).GetMethod("get_ColDimensions", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.get_ColDimensions()");

    private static MethodInfo RowDimensionArrayMethod(Side side) =>
        CDim(side).GetMethod("RowDimensionArray", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} c_dim.RowDimensionArray(int)");

    private static object CreateCDim(Side side, string[] dimNames, int rows)
    {
        var array = Array.CreateInstance(side.Dim, dimNames.Length);
        for (int i = 0; i < dimNames.Length; i++)
        {
            array.SetValue(side.D(dimNames[i]), i);
        }
        return MakeMethod(side).Invoke(null, [array, rows])!;
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
        if (value.GetType() == CDim(side))
        {
            var coldim = (Array)Get(value, "m_coldim")!;
            var items = string.Join(",", coldim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"c_dim({Get(value, "Rows")}x{Get(value, "Cols")},coldim=[{items}])";
        }
        if (value is Array arr && arr.GetType().GetElementType() == side.Dim)
        {
            var items = string.Join(",", arr.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"Dim[][{items}]";
        }
        return Reflect.Text(value);
    }

    private static string State(Side side, object target) => Show(side, target);

    private static void CompareDim(string caseId, string[] dimNames, int rows, int i, int j)
    {
        object qTarget = CreateCDim(Side.Quire, dimNames, rows);
        object mTarget = CreateCDim(Side.Mathcad, dimNames, rows);
        MethodComparison.Compare(caseId, Contract,
            DimMethod(Side.Quire), qTarget, [i, j],
            DimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareMatDim(string caseId, string[] dimNames, int rows, int i, int j)
    {
        object qTarget = CreateCDim(Side.Quire, dimNames, rows);
        object mTarget = CreateCDim(Side.Mathcad, dimNames, rows);
        MethodComparison.Compare(caseId, Contract,
            MatDimMethod(Side.Quire), qTarget, [i, j],
            MatDimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareIsLinear(string caseId, string[] dimNames, int rows)
    {
        object qTarget = CreateCDim(Side.Quire, dimNames, rows);
        object mTarget = CreateCDim(Side.Mathcad, dimNames, rows);
        MethodComparison.Compare(caseId, Contract,
            IsLinearGetter(Side.Quire), qTarget, [],
            IsLinearGetter(Side.Mathcad), mTarget, [],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        Assert.Same(qTarget, IsLinearGetter(Side.Quire).Invoke(qTarget, []));
        Assert.Same(mTarget, IsLinearGetter(Side.Mathcad).Invoke(mTarget, []));
    }

    private static void CompareColDimensions(string caseId, string[] dimNames, int rows)
    {
        object qTarget = CreateCDim(Side.Quire, dimNames, rows);
        object mTarget = CreateCDim(Side.Mathcad, dimNames, rows);
        MethodComparison.Compare(caseId, Contract,
            ColDimensionsGetter(Side.Quire), qTarget, [],
            ColDimensionsGetter(Side.Mathcad), mTarget, [],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        Assert.Same(Get(qTarget, "m_coldim"), ColDimensionsGetter(Side.Quire).Invoke(qTarget, []));
        Assert.Same(Get(mTarget, "m_coldim"), ColDimensionsGetter(Side.Mathcad).Invoke(mTarget, []));
    }

    private static void CompareRowDimensionArray(string caseId, string[] dimNames, int rows, int r)
    {
        object qTarget = CreateCDim(Side.Quire, dimNames, rows);
        object mTarget = CreateCDim(Side.Mathcad, dimNames, rows);
        MethodComparison.Compare(caseId, Contract,
            RowDimensionArrayMethod(Side.Quire), qTarget, [r],
            RowDimensionArrayMethod(Side.Mathcad), mTarget, [r],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        Assert.Same(Get(qTarget, "m_coldim"), RowDimensionArrayMethod(Side.Quire).Invoke(qTarget, [r]));
        Assert.Same(Get(mTarget, "m_coldim"), RowDimensionArrayMethod(Side.Mathcad).Invoke(mTarget, [r]));
    }

    [SkippableFact]
    public void C_dim_coordinate_and_property_access_matches_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(DimToken, DimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(MatDimToken, MatDimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(IsLinearToken, IsLinearGetter(Side.Mathcad).MetadataToken);
        Assert.Equal(ColDimensionsToken, ColDimensionsGetter(Side.Mathcad).MetadataToken);
        Assert.Equal(RowDimensionArrayToken, RowDimensionArrayMethod(Side.Mathcad).MetadataToken);

        string[] threeDims = ["L", "T", "v"];
        string[] singletonDim = ["identity"];
        string[] emptyDims = [];

        // dim(i, j)
        CompareDim("dim at origin", threeDims, 4, 0, 0);
        CompareDim("dim at second column", threeDims, 4, 2, 1);
        CompareDim("dim at last column with varied row index", threeDims, 4, 99, 2);
        CompareDim("dim with negative row index ignored", threeDims, 4, -5, 0);
        CompareDim("dim with out-of-bounds col index throws IndexOutOfRangeException", threeDims, 4, 0, 3);
        CompareDim("dim with negative col index throws IndexOutOfRangeException", threeDims, 4, 0, -1);
        CompareDim("dim on empty col array throws IndexOutOfRangeException", emptyDims, 2, 0, 0);
        CompareDim("dim on singleton col array", singletonDim, 1, 0, 0);

        // matdim(i, j)
        CompareMatDim("matdim at origin", threeDims, 4, 0, 0);
        CompareMatDim("matdim at second column", threeDims, 4, 2, 1);
        CompareMatDim("matdim at last column with varied row index", threeDims, 4, 99, 2);
        CompareMatDim("matdim with negative row index ignored", threeDims, 4, -5, 0);
        CompareMatDim("matdim with out-of-bounds col index throws IndexOutOfRangeException", threeDims, 4, 0, 3);
        CompareMatDim("matdim with negative col index throws IndexOutOfRangeException", threeDims, 4, 0, -1);
        CompareMatDim("matdim on empty col array throws IndexOutOfRangeException", emptyDims, 2, 0, 0);
        CompareMatDim("matdim on singleton col array", singletonDim, 1, 0, 0);

        // get_IsLinear
        CompareIsLinear("get_IsLinear on normal c_dim", threeDims, 4);
        CompareIsLinear("get_IsLinear on empty c_dim", emptyDims, 0);
        CompareIsLinear("get_IsLinear on singleton c_dim", singletonDim, 1);

        // get_ColDimensions
        CompareColDimensions("get_ColDimensions on normal c_dim", threeDims, 4);
        CompareColDimensions("get_ColDimensions on empty c_dim", emptyDims, 0);
        CompareColDimensions("get_ColDimensions on singleton c_dim", singletonDim, 1);

        // RowDimensionArray(r)
        CompareRowDimensionArray("RowDimensionArray with r=0", threeDims, 4, 0);
        CompareRowDimensionArray("RowDimensionArray with r=2", threeDims, 4, 2);
        CompareRowDimensionArray("RowDimensionArray with negative r ignored", threeDims, 4, -1);
        CompareRowDimensionArray("RowDimensionArray with large r ignored", threeDims, 4, 100);
        CompareRowDimensionArray("RowDimensionArray on empty c_dim", emptyDims, 0, 0);
        CompareRowDimensionArray("RowDimensionArray on singleton c_dim", singletonDim, 1, 0);
    }
}
