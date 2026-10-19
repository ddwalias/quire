using System.Reflection;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValDimensionDifferentialTests
{
    private static Array Units(Side side, params string[] names)
    {
        var units = Array.CreateInstance(side.Dim, names.Length);
        for (int i = 0; i < names.Length; i++) units.SetValue(side.D(names[i]), i);
        return units;
    }
    private static object Matrix(Side side, string dimension, int kind)
    {
        var matrix = New(side.T("Mcd.MatVal"), new double[] { 99, 1, 2, 99, 3, 4, 99 }, new double[7], 2, 2, 3, null, Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 1);
        Set(matrix, "m_dim", side.D(dimension));
        if (kind > 0) CallSig(matrix, "set_2", ["Int32", "Int32", "Object"], 1, 1, side.Num(5, 6, "T"));
        if (kind > 1) Call(matrix, "set_ptrs_2", 0, 1, New(side.T("Mcd.MatVal"), new double[] { 3 }));
        if (kind > 2) Call(matrix, "set_ptrs_2", 1, 0, "text");
        return matrix;
    }
    private static void State(Side side, List<string> log, string label, object matrix) => log.Add(label + " " + MatValDifferentialTests.Storage(side, matrix));

    [SkippableFact]
    public void Casts_aliases_and_pointer_conversions() => Scenario.Compare((side, log) =>
    {
        var type = side.T("Mcd.MatVal");
        foreach (var dimension in Side.DimNames)
        foreach (int kind in Enumerable.Range(0, 4))
        foreach (var operation in new[] { "StripDimension", "AsUniform", "AsDimless", "AsPolymorphic", "SetDim" })
        {
            var input = Matrix(side, dimension, kind);
            log.Add(dimension + " kind " + kind + " " + operation);
            object? result = null;
            log.Log(side, "cast", () => result = operation == "SetDim" ? Call(input, operation, side.D("L")) : Call(type, operation, input));
            if (result != null)
            {
                State(side, log, "output", result);
                log.Add("same " + ReferenceEquals(input, result));
                log.Add("arrays alias " + ReferenceEquals(Get(input, "m_rarr"), Get(result, "m_rarr")));
            }
            State(side, log, "input", input);
        }
    });

    [SkippableFact]
    public void Dimension_application_and_division_mutations() => Scenario.Compare((side, log) =>
    {
        var type = side.T("Mcd.MatVal");
        var descriptor = side.T("Mcd.MatDim");
        foreach (int kind in Enumerable.Range(0, 4))
        foreach (var operation in new[] { "ApplyLinearDimension", "ApplyDimension", "ApplyCrazyDimension", "ApplyRowDimensions", "ApplyColDimensions", "MDimCast" })
        foreach (bool uniform in new[] { false, true })
        {
            var matrix = Matrix(side, "L", kind);
            var units = Units(side, "identity", uniform ? "identity" : "T");
            var axis = Enum.Parse(Nested(descriptor, "RC"), "ROW");
            var dim = Call(descriptor, "make", axis, units, 2);
            log.Add($"kind {kind} {operation} uniform {uniform}");
            object? output = null;
            log.Log(side, "apply", () => output = operation switch
            {
                "MDimCast" => Call(type, operation, matrix, dim),
                "ApplyRowDimensions" or "ApplyColDimensions" => Call(matrix, operation, units),
                _ => Call(matrix, operation, dim),
            });
            if (output != null) State(side, log, "output", output);
            State(side, log, "input", matrix);
        }
        foreach (var operation in new[] { "ApplyRowDimensions", "ApplyColDimensions" })
        {
            var matrix = Matrix(side, "L", 0);
            log.Log(side, operation + " pair", () => Call(matrix, operation, side.D("L"), side.D("T")));
            State(side, log, "paired", matrix);
        }
        foreach (int kind in Enumerable.Range(0, 4))
        foreach (var dimension in Side.DimNames)
        {
            var matrix = Matrix(side, "L", kind);
            log.Log(side, "divide " + dimension + " kind " + kind, () => Call(matrix, "DivideByDimension", side.D(dimension)));
            State(side, log, "divided", matrix);
            matrix = Matrix(side, dimension, kind);
            log.Log(side, "default divide", () => Call(matrix, "DivideByDefaultDimension"));
            State(side, log, "default divided", matrix);
        }
        foreach (object? value in new object?[] { null, "text", side.Num(1, 0, "poly") })
            log.Log(side, "nonmatrix cast", () => Call(type, "MDimCast", value, Call(descriptor, "make", side.D("T"))));
        var input = Matrix(side, "L", 1);
        log.Log(side, "null descriptor", () => ReferenceEquals(input, Call(type, "MDimCast", input, null)));
    });

    [SkippableFact]
    public void Initial_condition_order_dimension_rules() => Scenario.Compare((side, log) =>
    {
        var type = side.T("Mcd.MatVal");
        foreach (var order in new[] { new double[] { 0, 0, 0 }, new double[] { 0, 0, 2 }, new double[] { 0, 1, 2 }, new double[] { 0, -1, 0 } })
        foreach (var valueUnits in new[] { new[] { "L", "L", "L" }, new[] { "L", "T", "L" }, new[] { "identity", "identity", "identity" }, new[] { "poly", "poly", "poly" } })
        foreach (var pointUnit in new[] { "poly", "identity", "T" })
        {
            var orderMatrix = New(type, order);
            var value = New(type, new double[] { 2, 3, 4 });
            Set(value, "m_dim", side.D(valueUnits[0]));
            for (int i = 1; i < 3; i++) CallSig(value, "set_1", ["Int32", "Object"], i, side.Num(i + 2, 0, valueUnits[i]));
            log.Add("order " + side.Show(order) + " units " + string.Join(",", valueUnits) + " point " + pointUnit);
            object? result = null;
            log.Log(side, "create", () => result = Call(type, "MatCreateDim", orderMatrix, side.Num(1, 0, pointUnit), value));
            if (result != null)
            {
                State(side, log, "output", result);
                log.Add("same " + ReferenceEquals(value, result));
            }
            State(side, log, "input", value);
        }
        log.Log(side, "nonmatrix order", () => Call(type, "MatCreateDim", "other", side.Num(1, 0, "T"), "value"));
    });

    private const string Contract = "Mcd.MatVal";
    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");
    private const int ApplyLinearDimensionToken = 0x0600044a;
    private const int MatCreateDimToken = 0x06000456;

    private static string ShowValue(Side side, object? value)
    {
        if (value == null) return "null";
        if (value.GetType().FullName?.EndsWith(".MatVal", StringComparison.Ordinal) == true)
            return MatValDifferentialTests.Storage(side, value);
        if (value.GetType() == side.Dim) return $"Dim({side.DimText(value)})";
        if (side.T("Mcd.MatDim").IsInstanceOfType(value))
        {
            string text = $"{value.GetType().Name}:{Get(value, "Rows")}x{Get(value, "Cols")}" +
                $" unit={side.DimText(Call(value, "get_UniformDim"))}";
            int rows = (int)Get(value, "Rows")!, cols = (int)Get(value, "Cols")!;
            for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++) text += $" ({r},{c})={side.DimText(Call(value, "dim", r, c))}";
            return text;
        }
        return Text(value);
    }

    [SkippableFact]
    public void Direct_linear_dimensions_and_create_dim_match_mathcad()
    {
        Runtime.Sync();

        var qType = sideT("Mcd.MatVal", Side.Quire);
        var mType = sideT("Mcd.MatVal", Side.Mathcad);
        var qMatDim = sideT("Mcd.MatDim", Side.Quire);
        var mMatDim = sideT("Mcd.MatDim", Side.Mathcad);

        var qApplyLinear = qType.GetMethod("ApplyLinearDimension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [qMatDim], modifiers: null)!;
        var mApplyLinear = mType.GetMethod("ApplyLinearDimension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [mMatDim], modifiers: null)!;

        Assert.Equal(ApplyLinearDimensionToken, mApplyLinear.MetadataToken);
        Assert.Equal(MathcadMvid, mApplyLinear.Module.ModuleVersionId);

        var qMatCreateDim = qType.GetMethod("MatCreateDim", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [typeof(object), typeof(object), typeof(object)], modifiers: null)!;
        var mMatCreateDim = mType.GetMethod("MatCreateDim", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, [typeof(object), typeof(object), typeof(object)], modifiers: null)!;

        Assert.Equal(MatCreateDimToken, mMatCreateDim.MetadataToken);
        Assert.Equal(MathcadMvid, mMatCreateDim.Module.ModuleVersionId);

        // 1. ApplyLinearDimension
        // Uniform dimension
        var qMatUniform = Matrix(Side.Quire, "L", 0);
        var mMatUniform = Matrix(Side.Mathcad, "L", 0);
        var qDimUniform = Call(qMatDim, "make", Side.Quire.D("T"))!;
        var mDimUniform = Call(mMatDim, "make", Side.Mathcad.D("T"))!;

        MethodComparison.Compare("ApplyLinearDimension.uniform", Contract,
            qApplyLinear, qMatUniform, [qDimUniform],
            mApplyLinear, mMatUniform, [mDimUniform],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v),
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));

        // Non-uniform row dimension on real matrix (m_iarr == null)
        var qMatReal = New(qType, [new double[] { 1, 2, 3, 4 }, null, 2, 2]);
        var mMatReal = New(mType, [new double[] { 1, 2, 3, 4 }, null, 2, 2]);
        var qRowUnits = Units(Side.Quire, "L", "T");
        var mRowUnits = Units(Side.Mathcad, "L", "T");
        var qAxis = Enum.Parse(Nested(qMatDim, "RC"), "ROW");
        var mAxis = Enum.Parse(Nested(mMatDim, "RC"), "ROW");
        var qRowDim = Call(qMatDim, "make", qAxis, qRowUnits, 2)!;
        var mRowDim = Call(mMatDim, "make", mAxis, mRowUnits, 2)!;

        MethodComparison.Compare("ApplyLinearDimension.real_row_dim", Contract,
            qApplyLinear, qMatReal, [qRowDim],
            mApplyLinear, mMatReal, [mRowDim],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v),
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));

        // Non-uniform row dimension on complex matrix (m_iarr != null)
        var qMatComplex = Matrix(Side.Quire, "L", 1);
        var mMatComplex = Matrix(Side.Mathcad, "L", 1);

        MethodComparison.Compare("ApplyLinearDimension.complex_row_dim", Contract,
            qApplyLinear, qMatComplex, [qRowDim],
            mApplyLinear, mMatComplex, [mRowDim],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v),
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));

        // 2. MatCreateDim
        // Null point unit
        MethodComparison.Compare("MatCreateDim.null_point", Contract,
            qMatCreateDim, null, [qMatReal, null, qMatReal],
            mMatCreateDim, null, [mMatReal, null, mMatReal],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));

        // Nonmatrix order
        MethodComparison.Compare("MatCreateDim.nonmatrix_order", Contract,
            qMatCreateDim, null, ["nonmatrix", Side.Quire.Num(1, 0, "T"), qMatReal],
            mMatCreateDim, null, ["nonmatrix", Side.Mathcad.Num(1, 0, "T"), mMatReal],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));

        // Dimensionless value matrix
        var qDimlessVal = New(qType, new double[] { 2, 3, 4 });
        var mDimlessVal = New(mType, new double[] { 2, 3, 4 });
        var qOrderMat = New(qType, new double[] { 0, 0, 0 });
        var mOrderMat = New(mType, new double[] { 0, 0, 0 });

        MethodComparison.Compare("MatCreateDim.dimless_value", Contract,
            qMatCreateDim, null, [qOrderMat, Side.Quire.Num(1, 0, "T"), qDimlessVal],
            mMatCreateDim, null, [mOrderMat, Side.Mathcad.Num(1, 0, "T"), mDimlessVal],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));

        // Dimensioned value with power expansion (orders { 0, 0, 0 })
        var qValPower = New(qType, new double[] { 2, 3, 4 });
        var mValPower = New(mType, new double[] { 2, 3, 4 });
        Set(qValPower, "m_dim", Side.Quire.D("L"));
        Set(mValPower, "m_dim", Side.Mathcad.D("L"));
        for (int i = 1; i < 3; i++)
        {
            CallSig(qValPower, "set_1", ["Int32", "Object"], i, Side.Quire.Num(i + 2, 0, "L"));
            CallSig(mValPower, "set_1", ["Int32", "Object"], i, Side.Mathcad.Num(i + 2, 0, "L"));
        }

        MethodComparison.Compare("MatCreateDim.power_expansion", Contract,
            qMatCreateDim, null, [qOrderMat, Side.Quire.Num(1, 0, "T"), qValPower],
            mMatCreateDim, null, [mOrderMat, Side.Mathcad.Num(1, 0, "T"), mValPower],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));

        // Incompatible repeated order (early return reset)
        var qOrderIncompat = New(qType, new double[] { 0, 1, 0 });
        var mOrderIncompat = New(mType, new double[] { 0, 1, 0 });
        var qValIncompat = New(qType, new double[] { 2, 3, 4 });
        var mValIncompat = New(mType, new double[] { 2, 3, 4 });
        Set(qValIncompat, "m_dim", Side.Quire.D("L"));
        Set(mValIncompat, "m_dim", Side.Mathcad.D("L"));
        CallSig(qValIncompat, "set_1", ["Int32", "Object"], 1, Side.Quire.Num(3, 0, "T"));
        CallSig(mValIncompat, "set_1", ["Int32", "Object"], 1, Side.Mathcad.Num(3, 0, "T"));
        CallSig(qValIncompat, "set_1", ["Int32", "Object"], 2, Side.Quire.Num(4, 0, "L"));
        CallSig(mValIncompat, "set_1", ["Int32", "Object"], 2, Side.Mathcad.Num(4, 0, "L"));

        MethodComparison.Compare("MatCreateDim.incompatible_reset", Contract,
            qMatCreateDim, null, [qOrderIncompat, Side.Quire.Num(1, 0, "T"), qValIncompat],
            mMatCreateDim, null, [mOrderIncompat, Side.Mathcad.Num(1, 0, "T"), mValIncompat],
            v => ShowValue(Side.Quire, v), v => ShowValue(Side.Mathcad, v));
    }

    private static System.Type sideT(string name, Side side) => side.T(name);
}
