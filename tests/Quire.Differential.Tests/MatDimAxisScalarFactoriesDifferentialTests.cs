using System.Reflection;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimAxisScalarFactoriesDifferentialTests
{
    private const string Contract = "Mcd.MatDim";
    private const int RowToken = 0x06000623;
    private const int ColToken = 0x06000625;
    private const BindingFlags StaticDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static MethodInfo Method(Side side, string name) =>
        side.T("Mcd.MatDim").GetMethod(name, StaticDeclared, null,
            [side.T("Mcd.Dim"), side.T("Mcd.Dim"), typeof(int)], null)
        ?? throw new HarnessException($"Missing {side.Name} MatDim.{name}(Dim, Dim, int)");

    private static string Result(Side side, object? value, object d1, object d2, object? common)
    {
        if (value == null) return "null";
        string result = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}";
        object? uniform = Get(value, "UniformDim");
        result += $" uniform={side.DimText(uniform)} uniform-ref-common={(ReferenceEquals(uniform, common) ? "yes" : "no")}";
        if (value.GetType().Name == "r_dim")
        {
            var dims = (Array)Get(value, "RowDimensions")!;
            result += $" row-dim-count={dims.Length} row0-ref-d1={ReferenceEquals(dims.GetValue(0), d1)}" +
                $" row1-ref-d2={ReferenceEquals(dims.GetValue(1), d2)} row-values={side.DimText(dims.GetValue(0))},{side.DimText(dims.GetValue(1))}";
        }
        else if (value.GetType().Name == "c_dim")
        {
            var dims = (Array)Get(value, "ColDimensions")!;
            result += $" col-dim-count={dims.Length} col0-ref-d1={ReferenceEquals(dims.GetValue(0), d1)}" +
                $" col1-ref-d2={ReferenceEquals(dims.GetValue(1), d2)} col-values={side.DimText(dims.GetValue(0))},{side.DimText(dims.GetValue(1))}";
        }
        return result;
    }

    private static string Show(Side side, object? value, object d1, object d2, object? common)
    {
        if (value != null && side.T("Mcd.MatDim").IsInstanceOfType(value)) return Result(side, value, d1, d2, common);
        if (value != null && side.T("Mcd.Dim").IsInstanceOfType(value)) return side.DimText(value);
        return value is IFormattable scalar ? scalar.ToString(null, System.Globalization.CultureInfo.InvariantCulture) : side.Show(value);
    }

    private static void Compare(string caseId, string methodName, int token, string d1Name, string d2Name, int n)
    {
        object qD1 = Side.Quire.D(d1Name)!, qD2 = Side.Quire.D(d2Name)!;
        object mD1 = Side.Mathcad.D(d1Name)!, mD2 = Side.Mathcad.D(d2Name)!;
        object? qCommon = d1Name == d2Name ? qD1 : null;
        object? mCommon = d1Name == d2Name ? mD1 : null;
        var qMethod = Method(Side.Quire, methodName); var mMethod = Method(Side.Mathcad, methodName);
        Assert.Equal(token, mMethod.MetadataToken);
        MethodComparison.Compare(caseId, Contract, qMethod, null, [qD1, qD2, n], mMethod, null, [mD1, mD2, n],
            quireShow: value => Show(Side.Quire, value, qD1, qD2, qCommon),
            originalShow: value => Show(Side.Mathcad, value, mD1, mD2, mCommon));
    }

    [SkippableFact]
    public void Scalar_axis_factories_forward_factors_and_use_original_uniform_precedence()
    {
        Runtime.Sync();
        Assert.Equal(RowToken, Method(Side.Mathcad, "makeRowDimension").MetadataToken);
        Assert.Equal(ColToken, Method(Side.Mathcad, "makeColDimension").MetadataToken);

        Compare("makeRowDimension uniform factors ignore requested column count", "makeRowDimension", RowToken, "L", "L", 7);
        Compare("makeRowDimension nonuniform factors retain row dimension references", "makeRowDimension", RowToken, "L", "T", 3);
        Compare("makeRowDimension preserves a negative requested column count", "makeRowDimension", RowToken, "L", "T", -1);
        Compare("makeColDimension uniform factors ignore requested row count", "makeColDimension", ColToken, "L", "L", 7);
        Compare("makeColDimension nonuniform factors retain column dimension references", "makeColDimension", ColToken, "L", "T", 4);
        Compare("makeColDimension preserves a negative requested row count", "makeColDimension", ColToken, "L", "T", -2);
    }
}
