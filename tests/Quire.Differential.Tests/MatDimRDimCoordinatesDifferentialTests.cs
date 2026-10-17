using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRDimCoordinatesDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007c5;
    private const int MatDimToken = 0x060007c6;
    private const int IsLinearToken = 0x060007c7;
    private const int RowDimensionsToken = 0x060007c8;
    private const int ColDimensionArrayToken = 0x060007c9;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static MethodInfo MakeMethod(Side side) =>
        RDim(side).GetMethod("make", DeclaredStatic, null, [side.Dim.MakeArrayType(), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.make(Dim[], int)");

    private static MethodInfo DimMethod(Side side) =>
        RDim(side).GetMethod("dim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.dim(int, int)");

    private static MethodInfo MatDimMethod(Side side) =>
        RDim(side).GetMethod("matdim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.matdim(int, int)");

    private static MethodInfo IsLinearGetter(Side side) =>
        RDim(side).GetMethod("get_IsLinear", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.get_IsLinear()");

    private static MethodInfo RowDimensionsGetter(Side side) =>
        RDim(side).GetMethod("get_RowDimensions", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.get_RowDimensions()");

    private static MethodInfo ColDimensionArrayMethod(Side side) =>
        RDim(side).GetMethod("ColDimensionArray", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} r_dim.ColDimensionArray(int)");

    private static object CreateRDim(Side side, string[] dimNames, int cols)
    {
        var array = Array.CreateInstance(side.Dim, dimNames.Length);
        for (int i = 0; i < dimNames.Length; i++)
        {
            array.SetValue(side.D(dimNames[i]), i);
        }
        return MakeMethod(side).Invoke(null, [array, cols])!;
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
        if (value is Array arr && arr.GetType().GetElementType() == side.Dim)
        {
            var items = string.Join(",", arr.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"Dim[][{items}]";
        }
        return Reflect.Text(value);
    }

    private static string State(Side side, object target) => Show(side, target);

    private static void CompareDim(string caseId, string[] dimNames, int cols, int i, int j)
    {
        object qTarget = CreateRDim(Side.Quire, dimNames, cols);
        object mTarget = CreateRDim(Side.Mathcad, dimNames, cols);
        MethodComparison.Compare(caseId, Contract,
            DimMethod(Side.Quire), qTarget, [i, j],
            DimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareMatDim(string caseId, string[] dimNames, int cols, int i, int j)
    {
        object qTarget = CreateRDim(Side.Quire, dimNames, cols);
        object mTarget = CreateRDim(Side.Mathcad, dimNames, cols);
        MethodComparison.Compare(caseId, Contract,
            MatDimMethod(Side.Quire), qTarget, [i, j],
            MatDimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareIsLinear(string caseId, string[] dimNames, int cols)
    {
        object qTarget = CreateRDim(Side.Quire, dimNames, cols);
        object mTarget = CreateRDim(Side.Mathcad, dimNames, cols);
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

    private static void CompareRowDimensions(string caseId, string[] dimNames, int cols)
    {
        object qTarget = CreateRDim(Side.Quire, dimNames, cols);
        object mTarget = CreateRDim(Side.Mathcad, dimNames, cols);
        MethodComparison.Compare(caseId, Contract,
            RowDimensionsGetter(Side.Quire), qTarget, [],
            RowDimensionsGetter(Side.Mathcad), mTarget, [],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        Assert.Same(Get(qTarget, "m_rowdim"), RowDimensionsGetter(Side.Quire).Invoke(qTarget, []));
        Assert.Same(Get(mTarget, "m_rowdim"), RowDimensionsGetter(Side.Mathcad).Invoke(mTarget, []));
    }

    private static void CompareColDimensionArray(string caseId, string[] dimNames, int cols, int c)
    {
        object qTarget = CreateRDim(Side.Quire, dimNames, cols);
        object mTarget = CreateRDim(Side.Mathcad, dimNames, cols);
        MethodComparison.Compare(caseId, Contract,
            ColDimensionArrayMethod(Side.Quire), qTarget, [c],
            ColDimensionArrayMethod(Side.Mathcad), mTarget, [c],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        Assert.Same(Get(qTarget, "m_rowdim"), ColDimensionArrayMethod(Side.Quire).Invoke(qTarget, [c]));
        Assert.Same(Get(mTarget, "m_rowdim"), ColDimensionArrayMethod(Side.Mathcad).Invoke(mTarget, [c]));
    }

    [SkippableFact]
    public void R_dim_coordinate_and_property_access_matches_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(DimToken, DimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(MatDimToken, MatDimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(IsLinearToken, IsLinearGetter(Side.Mathcad).MetadataToken);
        Assert.Equal(RowDimensionsToken, RowDimensionsGetter(Side.Mathcad).MetadataToken);
        Assert.Equal(ColDimensionArrayToken, ColDimensionArrayMethod(Side.Mathcad).MetadataToken);

        string[] threeDims = ["L", "T", "v"];
        string[] singletonDim = ["identity"];
        string[] emptyDims = [];

        // dim(i, j)
        CompareDim("dim at origin", threeDims, 4, 0, 0);
        CompareDim("dim at second row", threeDims, 4, 1, 2);
        CompareDim("dim at last row with varied column index", threeDims, 4, 2, 99);
        CompareDim("dim with negative column index ignored", threeDims, 4, 0, -5);
        CompareDim("dim with out-of-bounds row index throws IndexOutOfRangeException", threeDims, 4, 3, 0);
        CompareDim("dim with negative row index throws IndexOutOfRangeException", threeDims, 4, -1, 0);
        CompareDim("dim on empty row array throws IndexOutOfRangeException", emptyDims, 2, 0, 0);
        CompareDim("dim on singleton row array", singletonDim, 1, 0, 0);

        // matdim(i, j)
        CompareMatDim("matdim at origin", threeDims, 4, 0, 0);
        CompareMatDim("matdim at second row", threeDims, 4, 1, 2);
        CompareMatDim("matdim at last row with varied column index", threeDims, 4, 2, 99);
        CompareMatDim("matdim with negative column index ignored", threeDims, 4, 0, -5);
        CompareMatDim("matdim with out-of-bounds row index throws IndexOutOfRangeException", threeDims, 4, 3, 0);
        CompareMatDim("matdim with negative row index throws IndexOutOfRangeException", threeDims, 4, -1, 0);
        CompareMatDim("matdim on empty row array throws IndexOutOfRangeException", emptyDims, 2, 0, 0);
        CompareMatDim("matdim on singleton row array", singletonDim, 1, 0, 0);

        // get_IsLinear
        CompareIsLinear("get_IsLinear on normal r_dim", threeDims, 4);
        CompareIsLinear("get_IsLinear on empty r_dim", emptyDims, 0);
        CompareIsLinear("get_IsLinear on singleton r_dim", singletonDim, 1);

        // get_RowDimensions
        CompareRowDimensions("get_RowDimensions on normal r_dim", threeDims, 4);
        CompareRowDimensions("get_RowDimensions on empty r_dim", emptyDims, 0);
        CompareRowDimensions("get_RowDimensions on singleton r_dim", singletonDim, 1);

        // ColDimensionArray(c)
        CompareColDimensionArray("ColDimensionArray with c=0", threeDims, 4, 0);
        CompareColDimensionArray("ColDimensionArray with c=2", threeDims, 4, 2);
        CompareColDimensionArray("ColDimensionArray with negative c ignored", threeDims, 4, -1);
        CompareColDimensionArray("ColDimensionArray with large c ignored", threeDims, 4, 100);
        CompareColDimensionArray("ColDimensionArray on empty r_dim", emptyDims, 0, 0);
        CompareColDimensionArray("ColDimensionArray on singleton r_dim", singletonDim, 1, 0);
    }
}
