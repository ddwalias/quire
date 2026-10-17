using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimRcDimCoordinatesDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int DimToken = 0x060007b5;
    private const int MatDimToken = 0x060007b6;
    private const int IsLinearToken = 0x060007b7;
    private const int RowDimensionArrayToken = 0x060007b8;
    private const int ColDimensionArrayToken = 0x060007b9;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type RcDim(Side side) => Nested(MatDim(side), "rc_dim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static MethodInfo MakeMethod(Side side) =>
        RcDim(side).GetMethod("make_rc", DeclaredStatic, null, [side.Dim.MakeArrayType(), side.Dim.MakeArrayType()], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.make_rc(Dim[], Dim[])");

    private static MethodInfo DimMethod(Side side) =>
        RcDim(side).GetMethod("dim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.dim(int, int)");

    private static MethodInfo MatDimMethod(Side side) =>
        RcDim(side).GetMethod("matdim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.matdim(int, int)");

    private static MethodInfo IsLinearGetter(Side side) =>
        RcDim(side).GetMethod("get_IsLinear", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.get_IsLinear()");

    private static MethodInfo RowDimensionArrayMethod(Side side) =>
        RcDim(side).GetMethod("RowDimensionArray", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.RowDimensionArray(int)");

    private static MethodInfo ColDimensionArrayMethod(Side side) =>
        RcDim(side).GetMethod("ColDimensionArray", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} rc_dim.ColDimensionArray(int)");

    private static object CreateRcDim(Side side, string[] rowDims, string[] colDims)
    {
        var rArray = Array.CreateInstance(side.Dim, rowDims.Length);
        for (int i = 0; i < rowDims.Length; i++) rArray.SetValue(side.D(rowDims[i]), i);
        var cArray = Array.CreateInstance(side.Dim, colDims.Length);
        for (int i = 0; i < colDims.Length; i++) cArray.SetValue(side.D(colDims[i]), i);
        return MakeMethod(side).Invoke(null, [rArray, cArray])!;
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
        if (value.GetType() == RcDim(side))
        {
            var rowdim = (Array)Get(value, "m_rowdim")!;
            var coldim = (Array)Get(value, "m_coldim")!;
            var rItems = string.Join(",", rowdim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            var cItems = string.Join(",", coldim.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"rc_dim({Get(value, "Rows")}x{Get(value, "Cols")},rowdim=[{rItems}],coldim=[{cItems}])";
        }
        if (value is Array arr && arr.GetType().GetElementType() == side.Dim)
        {
            var items = string.Join(",", arr.Cast<object?>().Select(d => d == null ? "null" : side.DimText(d)));
            return $"Dim[][{items}]";
        }
        return Reflect.Text(value);
    }

    private static string State(Side side, object target) => Show(side, target);

    private static void CompareDim(string caseId, string[] rowDims, string[] colDims, int i, int j)
    {
        object qTarget = CreateRcDim(Side.Quire, rowDims, colDims);
        object mTarget = CreateRcDim(Side.Mathcad, rowDims, colDims);
        MethodComparison.Compare(caseId, Contract,
            DimMethod(Side.Quire), qTarget, [i, j],
            DimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareMatDim(string caseId, string[] rowDims, string[] colDims, int i, int j)
    {
        object qTarget = CreateRcDim(Side.Quire, rowDims, colDims);
        object mTarget = CreateRcDim(Side.Mathcad, rowDims, colDims);
        MethodComparison.Compare(caseId, Contract,
            MatDimMethod(Side.Quire), qTarget, [i, j],
            MatDimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareIsLinear(string caseId, string[] rowDims, string[] colDims)
    {
        object qTarget = CreateRcDim(Side.Quire, rowDims, colDims);
        object mTarget = CreateRcDim(Side.Mathcad, rowDims, colDims);
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

    private static void CompareRowDimensionArray(string caseId, string[] rowDims, string[] colDims, int r)
    {
        object qTarget = CreateRcDim(Side.Quire, rowDims, colDims);
        object mTarget = CreateRcDim(Side.Mathcad, rowDims, colDims);
        MethodComparison.Compare(caseId, Contract,
            RowDimensionArrayMethod(Side.Quire), qTarget, [r],
            RowDimensionArrayMethod(Side.Mathcad), mTarget, [r],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareColDimensionArray(string caseId, string[] rowDims, string[] colDims, int c)
    {
        object qTarget = CreateRcDim(Side.Quire, rowDims, colDims);
        object mTarget = CreateRcDim(Side.Mathcad, rowDims, colDims);
        MethodComparison.Compare(caseId, Contract,
            ColDimensionArrayMethod(Side.Quire), qTarget, [c],
            ColDimensionArrayMethod(Side.Mathcad), mTarget, [c],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    [SkippableFact]
    public void Rc_dim_coordinate_and_property_access_matches_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(DimToken, DimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(MatDimToken, MatDimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(IsLinearToken, IsLinearGetter(Side.Mathcad).MetadataToken);
        Assert.Equal(RowDimensionArrayToken, RowDimensionArrayMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(ColDimensionArrayToken, ColDimensionArrayMethod(Side.Mathcad).MetadataToken);

        string[] rows2 = ["L", "T"];
        string[] cols3 = ["identity", "v", "N"];
        string[] singleton = ["L"];
        string[] empty = [];

        // dim(i, j)
        CompareDim("dim at origin", rows2, cols3, 0, 0);
        CompareDim("dim at (1, 1)", rows2, cols3, 1, 1);
        CompareDim("dim at (1, 2)", rows2, cols3, 1, 2);
        CompareDim("dim with out-of-bounds row throws IndexOutOfRangeException", rows2, cols3, 2, 0);
        CompareDim("dim with out-of-bounds col throws IndexOutOfRangeException", rows2, cols3, 0, 3);
        CompareDim("dim with negative row throws IndexOutOfRangeException", rows2, cols3, -1, 0);
        CompareDim("dim with negative col throws IndexOutOfRangeException", rows2, cols3, 0, -1);
        CompareDim("dim on singleton rc_dim", singleton, singleton, 0, 0);
        CompareDim("dim on empty rc_dim throws IndexOutOfRangeException", empty, empty, 0, 0);

        // matdim(i, j)
        CompareMatDim("matdim at origin", rows2, cols3, 0, 0);
        CompareMatDim("matdim at (1, 1)", rows2, cols3, 1, 1);
        CompareMatDim("matdim at (1, 2)", rows2, cols3, 1, 2);
        CompareMatDim("matdim with out-of-bounds row throws IndexOutOfRangeException", rows2, cols3, 2, 0);
        CompareMatDim("matdim with out-of-bounds col throws IndexOutOfRangeException", rows2, cols3, 0, 3);
        CompareMatDim("matdim with negative row throws IndexOutOfRangeException", rows2, cols3, -1, 0);
        CompareMatDim("matdim with negative col throws IndexOutOfRangeException", rows2, cols3, 0, -1);
        CompareMatDim("matdim on singleton rc_dim", singleton, singleton, 0, 0);
        CompareMatDim("matdim on empty rc_dim throws IndexOutOfRangeException", empty, empty, 0, 0);

        // get_IsLinear
        CompareIsLinear("get_IsLinear on normal rc_dim", rows2, cols3);
        CompareIsLinear("get_IsLinear on singleton rc_dim", singleton, singleton);
        CompareIsLinear("get_IsLinear on empty rc_dim", empty, empty);

        // RowDimensionArray(r)
        CompareRowDimensionArray("RowDimensionArray with r=0", rows2, cols3, 0);
        CompareRowDimensionArray("RowDimensionArray with r=1", rows2, cols3, 1);
        CompareRowDimensionArray("RowDimensionArray with out-of-bounds r throws IndexOutOfRangeException", rows2, cols3, 2);
        CompareRowDimensionArray("RowDimensionArray with negative r throws IndexOutOfRangeException", rows2, cols3, -1);
        CompareRowDimensionArray("RowDimensionArray on singleton rc_dim", singleton, singleton, 0);
        CompareRowDimensionArray("RowDimensionArray on empty rc_dim throws IndexOutOfRangeException", empty, empty, 0);

        // ColDimensionArray(c)
        CompareColDimensionArray("ColDimensionArray with c=0", rows2, cols3, 0);
        CompareColDimensionArray("ColDimensionArray with c=1", rows2, cols3, 1);
        CompareColDimensionArray("ColDimensionArray with c=2", rows2, cols3, 2);
        CompareColDimensionArray("ColDimensionArray with out-of-bounds c throws IndexOutOfRangeException", rows2, cols3, 3);
        CompareColDimensionArray("ColDimensionArray with negative c throws IndexOutOfRangeException", rows2, cols3, -1);
        CompareColDimensionArray("ColDimensionArray on singleton rc_dim", singleton, singleton, 0);
        CompareColDimensionArray("ColDimensionArray on empty rc_dim throws IndexOutOfRangeException", empty, empty, 0);
    }
}
