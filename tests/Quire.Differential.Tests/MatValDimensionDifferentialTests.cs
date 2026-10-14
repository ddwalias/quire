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
}
