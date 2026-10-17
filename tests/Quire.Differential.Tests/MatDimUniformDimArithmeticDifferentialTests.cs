using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public sealed class MatDimUniformDimArithmeticDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int MMult1Token = 0x060007a4;
    private const int MMult2UniformToken = 0x060007a5;
    private const int MMult2RDimToken = 0x060007a6;
    private const int DMult1Token = 0x060007a7;
    private const int DMult2UniformToken = 0x060007a8;
    private const int DetToken = 0x060007aa;
    private const int InvertToken = 0x060007ab;
    private const int PwrToken = 0x060007ac;

    private const BindingFlags DeclaredInstance = BindingFlags.Instance | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static Type MatDim(Side side) => side.T("Mcd.MatDim");
    private static Type UniformDim(Side side) => Nested(MatDim(side), "uniform_dim");
    private static Type RDim(Side side) => Nested(MatDim(side), "r_dim");

    private static MethodInfo MMult1(Side side) =>
        UniformDim(side).GetMethod("mmult1", DeclaredInstance, null, [MatDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.mmult1(MatDim)");

    private static MethodInfo MMult2Uniform(Side side) =>
        UniformDim(side).GetMethod("mmult2", DeclaredInstance, null, [UniformDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.mmult2(uniform_dim)");

    private static MethodInfo MMult2RDim(Side side) =>
        UniformDim(side).GetMethod("mmult2", DeclaredInstance, null, [RDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.mmult2(r_dim)");

    private static MethodInfo DMult1(Side side) =>
        UniformDim(side).GetMethod("dmult1", DeclaredInstance, null, [MatDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.dmult1(MatDim)");

    private static MethodInfo DMult2Uniform(Side side) =>
        UniformDim(side).GetMethod("dmult2", DeclaredInstance, null, [UniformDim(side)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.dmult2(uniform_dim)");

    private static MethodInfo Det(Side side) =>
        UniformDim(side).GetMethod("det", DeclaredInstance, null, [typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.det(int)");

    private static MethodInfo Invert(Side side) =>
        UniformDim(side).GetMethod("invert", DeclaredInstance, null, Type.EmptyTypes, null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.invert()");

    private static MethodInfo Pwr(Side side) =>
        UniformDim(side).GetMethod("pwr", DeclaredInstance, null, [typeof(uint)], null)
        ?? throw new HarnessException($"Missing {side.Name} uniform_dim.pwr(uint)");

    private static object MakeUniform(Side side, string dimName, int rows, int cols) =>
        Call(UniformDim(side), "make", side.D(dimName), rows, cols)!;

    private static object MakeRDim(Side side, string[] dimNames, int cols)
    {
        var array = Array.CreateInstance(side.Dim, dimNames.Length);
        for (int i = 0; i < dimNames.Length; i++) array.SetValue(side.D(dimNames[i]), i);
        return Call(RDim(side), "make", array, cols)!;
    }

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
        if (value is uint u) return u.ToString();
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
        if (value.GetType().Name == "crazy_dim")
        {
            return $"crazy_dim({Get(value, "Rows")}x{Get(value, "Cols")})";
        }
        return Reflect.Text(value);
    }

    private static string State(Side side, object target) => Show(side, target);

    [SkippableFact]
    public void Uniform_dim_arithmetic_operations_match_mathcad()
    {
        Runtime.Sync();

        Assert.Equal(MMult1Token, MMult1(Side.Mathcad).MetadataToken);
        Assert.Equal(MMult2UniformToken, MMult2Uniform(Side.Mathcad).MetadataToken);
        Assert.Equal(MMult2RDimToken, MMult2RDim(Side.Mathcad).MetadataToken);
        Assert.Equal(DMult1Token, DMult1(Side.Mathcad).MetadataToken);
        Assert.Equal(DMult2UniformToken, DMult2Uniform(Side.Mathcad).MetadataToken);
        Assert.Equal(DetToken, Det(Side.Mathcad).MetadataToken);
        Assert.Equal(InvertToken, Invert(Side.Mathcad).MetadataToken);
        Assert.Equal(PwrToken, Pwr(Side.Mathcad).MetadataToken);

        // 1. mmult1(MatDim m2)
        {
            // Case 1: uniform_dim operand
            object qTarget1 = MakeUniform(Side.Quire, "L", 2, 3);
            object mTarget1 = MakeUniform(Side.Mathcad, "L", 2, 3);
            object qM2_1 = MakeUniform(Side.Quire, "T", 3, 4);
            object mM2_1 = MakeUniform(Side.Mathcad, "T", 3, 4);
            MethodComparison.Compare("mmult1 with uniform_dim operand", Contract,
                MMult1(Side.Quire), qTarget1, [qM2_1],
                MMult1(Side.Mathcad), mTarget1, [mM2_1],
                quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));

            // Case 2: crazy_dim operand (returns null)
            object qTarget2 = MakeUniform(Side.Quire, "L", 2, 3);
            object mTarget2 = MakeUniform(Side.Mathcad, "L", 2, 3);
            object qCrazy = MakeCrazy(Side.Quire);
            object mCrazy = MakeCrazy(Side.Mathcad);
            MethodComparison.Compare("mmult1 with crazy_dim operand returns null", Contract,
                MMult1(Side.Quire), qTarget2, [qCrazy],
                MMult1(Side.Mathcad), mTarget2, [mCrazy],
                quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
        }

        // 2. mmult2(uniform_dim m1)
        {
            void CompareMMult2Uniform(string caseId, string dim1, int r1, int c1, string dim2, int r2, int c2)
            {
                object qTarget = MakeUniform(Side.Quire, dim2, r2, c2);
                object mTarget = MakeUniform(Side.Mathcad, dim2, r2, c2);
                object qM1 = MakeUniform(Side.Quire, dim1, r1, c1);
                object mM1 = MakeUniform(Side.Mathcad, dim1, r1, c1);
                MethodComparison.Compare(caseId, Contract,
                    MMult2Uniform(Side.Quire), qTarget, [qM1],
                    MMult2Uniform(Side.Mathcad), mTarget, [mM1],
                    quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                    quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
            }

            CompareMMult2Uniform("mmult2 uniform with L and T", "L", 2, 4, "T", 4, 3);
            CompareMMult2Uniform("mmult2 uniform with identity and v", "identity", 1, 1, "v", 1, 5);
            CompareMMult2Uniform("mmult2 uniform with poly and L", "poly", 3, 0, "L", 0, 2);
        }

        // 3. mmult2(r_dim m1)
        {
            void CompareMMult2RDim(string caseId, string[] rdims, int c1, string dim2, int r2, int c2)
            {
                object qTarget = MakeUniform(Side.Quire, dim2, r2, c2);
                object mTarget = MakeUniform(Side.Mathcad, dim2, r2, c2);
                object qM1 = MakeRDim(Side.Quire, rdims, c1);
                object mM1 = MakeRDim(Side.Mathcad, rdims, c1);
                MethodComparison.Compare(caseId, Contract,
                    MMult2RDim(Side.Quire), qTarget, [qM1],
                    MMult2RDim(Side.Mathcad), mTarget, [mM1],
                    quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                    quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
            }

            CompareMMult2RDim("mmult2 r_dim with 2 row elements and v", ["L", "T"], 4, "v", 4, 3);
            CompareMMult2RDim("mmult2 r_dim with single identity element and L", ["identity"], 2, "L", 2, 5);
            CompareMMult2RDim("mmult2 r_dim with poly/v and zero columns", ["poly", "v"], 1, "T", 1, 0);
        }

        // 4. dmult1(MatDim m2)
        {
            // Case 1: uniform_dim operand
            object qTarget1 = MakeUniform(Side.Quire, "L", 2, 3);
            object mTarget1 = MakeUniform(Side.Mathcad, "L", 2, 3);
            object qM2_1 = MakeUniform(Side.Quire, "T", 3, 4);
            object mM2_1 = MakeUniform(Side.Mathcad, "T", 3, 4);
            MethodComparison.Compare("dmult1 with uniform_dim operand", Contract,
                DMult1(Side.Quire), qTarget1, [qM2_1],
                DMult1(Side.Mathcad), mTarget1, [mM2_1],
                quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));

            // Case 2: crazy_dim operand (returns null)
            object qTarget2 = MakeUniform(Side.Quire, "L", 2, 3);
            object mTarget2 = MakeUniform(Side.Mathcad, "L", 2, 3);
            object qCrazy = MakeCrazy(Side.Quire);
            object mCrazy = MakeCrazy(Side.Mathcad);
            MethodComparison.Compare("dmult1 with crazy_dim operand returns null", Contract,
                DMult1(Side.Quire), qTarget2, [qCrazy],
                DMult1(Side.Mathcad), mTarget2, [mCrazy],
                quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
        }

        // 5. dmult2(uniform_dim m1)
        {
            void CompareDMult2Uniform(string caseId, string dim1, string dim2)
            {
                object qTarget = MakeUniform(Side.Quire, dim2, 3, 4);
                object mTarget = MakeUniform(Side.Mathcad, dim2, 3, 4);
                object qM1 = MakeUniform(Side.Quire, dim1, 2, 3);
                object mM1 = MakeUniform(Side.Mathcad, dim1, 2, 3);
                MethodComparison.Compare(caseId, Contract,
                    DMult2Uniform(Side.Quire), qTarget, [qM1],
                    DMult2Uniform(Side.Mathcad), mTarget, [mM1],
                    quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                    quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
            }

            CompareDMult2Uniform("dmult2 uniform with L and T", "L", "T");
            CompareDMult2Uniform("dmult2 uniform with v and identity", "v", "identity");
            CompareDMult2Uniform("dmult2 uniform with L and L", "L", "L");
        }

        // 6. det(int n)
        {
            void CompareDet(string caseId, string dim, int rows, int cols, int n)
            {
                object qTarget = MakeUniform(Side.Quire, dim, rows, cols);
                object mTarget = MakeUniform(Side.Mathcad, dim, rows, cols);
                MethodComparison.Compare(caseId, Contract,
                    Det(Side.Quire), qTarget, [n],
                    Det(Side.Mathcad), mTarget, [n],
                    quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                    quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
            }

            CompareDet("det power 0", "L", 2, 2, 0);
            CompareDet("det power 1", "L", 2, 2, 1);
            CompareDet("det power 2", "L", 2, 2, 2);
            CompareDet("det negative power -1", "L", 2, 2, -1);
            CompareDet("det power 3 with velocity", "v", 3, 3, 3);
        }

        // 7. invert()
        {
            void CompareInvert(string caseId, string dim, int rows, int cols)
            {
                object qTarget = MakeUniform(Side.Quire, dim, rows, cols);
                object mTarget = MakeUniform(Side.Mathcad, dim, rows, cols);
                MethodComparison.Compare(caseId, Contract,
                    Invert(Side.Quire), qTarget, [],
                    Invert(Side.Mathcad), mTarget, [],
                    quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                    quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
            }

            CompareInvert("invert normal square matrix", "L", 2, 2);
            CompareInvert("invert identity dimension", "identity", 1, 1);
            CompareInvert("invert rectangular matrix with velocity", "v", 3, 4);
        }

        // 8. pwr(uint n)
        {
            void ComparePwr(string caseId, string dim, int rows, int cols, uint n)
            {
                object qTarget = MakeUniform(Side.Quire, dim, rows, cols);
                object mTarget = MakeUniform(Side.Mathcad, dim, rows, cols);
                MethodComparison.Compare(caseId, Contract,
                    Pwr(Side.Quire), qTarget, [n],
                    Pwr(Side.Mathcad), mTarget, [n],
                    quireShow: v => Show(Side.Quire, v), originalShow: v => Show(Side.Mathcad, v),
                    quireState: t => State(Side.Quire, t), originalState: t => State(Side.Mathcad, t));
            }

            ComparePwr("pwr 0", "L", 2, 2, 0u);
            ComparePwr("pwr 1", "L", 2, 2, 1u);
            ComparePwr("pwr 2", "L", 2, 2, 2u);
            ComparePwr("pwr 4 rectangular", "T", 3, 4, 4u);
        }
    }
}
