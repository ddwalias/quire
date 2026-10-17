using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformDimConstructionDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int CtorToken = 0x0600079c;
    private const int MakeToken = 0x0600079d;
    private const int DimToken = 0x0600079e;
    private const int MatDimToken = 0x0600079f;
    private const int UniformDimToken = 0x060007a0;
    private const int IsLinearToken = 0x060007a1;
    private const int TransposeToken = 0x060007a9;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private const BindingFlags DeclaredStatic = BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");

    private static ConstructorInfo Ctor(Side side) =>
        UniformDim(side).GetConstructor(DeclaredInstance, null, [side.Dim, typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim..ctor(Dim, int, int)");

    private static MethodInfo Make(Side side) =>
        UniformDim(side).GetMethod("make", DeclaredStatic, null, [side.Dim, typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.make(Dim, int, int)");

    private static MethodInfo DimMethod(Side side) =>
        UniformDim(side).GetMethod("dim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.dim(int, int)");

    private static MethodInfo MatDimMethod(Side side) =>
        UniformDim(side).GetMethod("matdim", DeclaredInstance, null, [typeof(int), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.matdim(int, int)");

    private static MethodInfo UniformDimGetter(Side side) =>
        UniformDim(side).GetMethod("get_UniformDim", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.get_UniformDim()");

    private static MethodInfo IsLinearGetter(Side side) =>
        UniformDim(side).GetMethod("get_IsLinear", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.get_IsLinear()");

    private static MethodInfo TransposeMethod(Side side) =>
        UniformDim(side).GetMethod("transpose", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.transpose()");

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is int i) return i.ToString();
        if (value.GetType() == side.Dim) return $"Dim({side.DimText(value)})";
        if (value.GetType() == UniformDim(side))
        {
            return $"uniform_dim({Get(value, "Rows")}x{Get(value, "Cols")},dim={side.DimText(Get(value, "m_dim"))})";
        }
        return Reflect.Text(value);
    }

    private static string State(Side side, object target) =>
        $"{target.GetType().Name}:{Get(target, "Rows")}x{Get(target, "Cols")},dim={side.DimText(Get(target, "m_dim"))}";

    private static object CreateUniformDim(Side side, string? dimName, int rows, int cols)
    {
        object? d = dimName != null ? side.D(dimName) : null;
        return Make(side).Invoke(null, [d, rows, cols])!;
    }

    private static void CompareCtor(string caseId, string? dimName, int rows, int cols)
    {
        object? qDim = dimName != null ? Side.Quire.D(dimName) : null;
        object? mDim = dimName != null ? Side.Mathcad.D(dimName) : null;
        MethodComparison.Compare(caseId, Contract,
            Ctor(Side.Quire), null, [qDim, rows, cols],
            Ctor(Side.Mathcad), null, [mDim, rows, cols],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v));
    }

    private static void CompareMake(string caseId, string? dimName, int rows, int cols)
    {
        object? qDim = dimName != null ? Side.Quire.D(dimName) : null;
        object? mDim = dimName != null ? Side.Mathcad.D(dimName) : null;
        MethodComparison.Compare(caseId, Contract,
            Make(Side.Quire), null, [qDim, rows, cols],
            Make(Side.Mathcad), null, [mDim, rows, cols],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v));
    }

    private static void CompareDim(string caseId, string? dimName, int rows, int cols, int i, int j)
    {
        object qTarget = CreateUniformDim(Side.Quire, dimName, rows, cols);
        object mTarget = CreateUniformDim(Side.Mathcad, dimName, rows, cols);
        MethodComparison.Compare(caseId, Contract,
            DimMethod(Side.Quire), qTarget, [i, j],
            DimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareMatDim(string caseId, string? dimName, int rows, int cols, int i, int j)
    {
        object qTarget = CreateUniformDim(Side.Quire, dimName, rows, cols);
        object mTarget = CreateUniformDim(Side.Mathcad, dimName, rows, cols);
        MethodComparison.Compare(caseId, Contract,
            MatDimMethod(Side.Quire), qTarget, [i, j],
            MatDimMethod(Side.Mathcad), mTarget, [i, j],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        Assert.Same(qTarget, MatDimMethod(Side.Quire).Invoke(qTarget, [i, j]));
        Assert.Same(mTarget, MatDimMethod(Side.Mathcad).Invoke(mTarget, [i, j]));
    }

    private static void CompareUniformDimProperty(string caseId, string? dimName, int rows, int cols)
    {
        object qTarget = CreateUniformDim(Side.Quire, dimName, rows, cols);
        object mTarget = CreateUniformDim(Side.Mathcad, dimName, rows, cols);
        MethodComparison.Compare(caseId, Contract,
            UniformDimGetter(Side.Quire), qTarget, [],
            UniformDimGetter(Side.Mathcad), mTarget, [],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));
    }

    private static void CompareIsLinearProperty(string caseId, string? dimName, int rows, int cols)
    {
        object qTarget = CreateUniformDim(Side.Quire, dimName, rows, cols);
        object mTarget = CreateUniformDim(Side.Mathcad, dimName, rows, cols);
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

    private static void CompareTranspose(string caseId, string? dimName, int rows, int cols)
    {
        object qTarget = CreateUniformDim(Side.Quire, dimName, rows, cols);
        object mTarget = CreateUniformDim(Side.Mathcad, dimName, rows, cols);
        MethodComparison.Compare(caseId, Contract,
            TransposeMethod(Side.Quire), qTarget, [],
            TransposeMethod(Side.Mathcad), mTarget, [],
            quireShow: v => Show(Side.Quire, v),
            originalShow: v => Show(Side.Mathcad, v),
            quireState: t => State(Side.Quire, t),
            originalState: t => State(Side.Mathcad, t));

        Assert.Same(qTarget, TransposeMethod(Side.Quire).Invoke(qTarget, []));
        Assert.Same(mTarget, TransposeMethod(Side.Mathcad).Invoke(mTarget, []));
    }

    [SkippableFact]
    public void Uniform_dim_construction_and_coordinate_access_match_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(CtorToken, Ctor(Side.Mathcad).MetadataToken);
        Assert.Equal(MakeToken, Make(Side.Mathcad).MetadataToken);
        Assert.Equal(DimToken, DimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(MatDimToken, MatDimMethod(Side.Mathcad).MetadataToken);
        Assert.Equal(UniformDimToken, UniformDimGetter(Side.Mathcad).MetadataToken);
        Assert.Equal(IsLinearToken, IsLinearGetter(Side.Mathcad).MetadataToken);
        Assert.Equal(TransposeToken, TransposeMethod(Side.Mathcad).MetadataToken);

        // Constructor tests
        CompareCtor("ctor with non-null dimension and normal shape", "L", 2, 3);
        CompareCtor("ctor with non-null dimension and single-element row", "T", 0, 1);
        CompareCtor("ctor with identity dimension", "identity", 4, 0);
        CompareCtor("ctor with null dimension defaults to identity", null, 1, 1);
        CompareCtor("ctor with negative shape arguments", null, -2, -3);

        // Factory tests
        CompareMake("make with non-null dimension and normal shape", "L", 2, 3);
        CompareMake("make with non-null dimension and single-element row", "T", 0, 1);
        CompareMake("make with identity dimension", "identity", 4, 0);
        CompareMake("make with null dimension defaults to identity", null, 1, 1);
        CompareMake("make with negative shape arguments", null, -2, -3);

        // dim(i, j) coordinate tests
        CompareDim("dim at origin", "L", 2, 3, 0, 0);
        CompareDim("dim at valid non-zero coordinates", "L", 2, 3, 1, 2);
        CompareDim("dim at out-of-bounds coordinates", "L", 2, 3, 99, -5);
        CompareDim("dim with null-defaulted identity dimension", null, 1, 1, 0, 0);

        // matdim(i, j) coordinate tests
        CompareMatDim("matdim at origin", "L", 2, 3, 0, 0);
        CompareMatDim("matdim at valid non-zero coordinates", "L", 2, 3, 1, 2);
        CompareMatDim("matdim at negative and out-of-bounds coordinates", "L", 2, 3, -10, 10);
        CompareMatDim("matdim with null-defaulted identity dimension", null, 1, 1, 0, 0);

        // UniformDim getter tests
        CompareUniformDimProperty("UniformDim getter with dimension L", "L", 2, 3);
        CompareUniformDimProperty("UniformDim getter with dimension T", "T", 0, 1);
        CompareUniformDimProperty("UniformDim getter with null-defaulted identity dimension", null, 1, 1);

        // IsLinear getter tests
        CompareIsLinearProperty("IsLinear getter with dimension L", "L", 2, 3);
        CompareIsLinearProperty("IsLinear getter with null-defaulted identity dimension", null, 1, 1);

        // transpose() tests
        CompareTranspose("transpose with dimension L", "L", 2, 3);
        CompareTranspose("transpose with dimension T and single-element shape", "T", 0, 1);
        CompareTranspose("transpose with null-defaulted identity dimension", null, 1, 1);
    }
}
