using System.Collections;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValViewDifferentialTests
{
    private static object Matrix(Side side, int kind = 0)
    {
        var matrix = New(side.T("Mcd.MatVal"), new double[] { 99, 1, 2, 99, 3, 4, 99 }, new double[7], 2, 2, 3, null, Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 1);
        Set(matrix, "m_dim", side.D("L"));
        if (kind > 0) Call(matrix, "set_ptrs_2", 0, 0, "outside");
        if (kind > 1) CallSig(matrix, "set_2", ["Int32", "Int32", "Object"], 1, 1, side.Num(5, 2, "T"));
        if (kind > 2) Call(matrix, "set_ptrs_2", 0, 1, New(side.T("Mcd.MatVal"), new double[] { 6 }));
        return matrix;
    }
    private static void State(Side side, List<string> log, string label, object matrix) => log.Add(label + " " + MatValDifferentialTests.Storage(side, matrix));

    [SkippableFact]
    public void Column_row_and_half_open_views() => Scenario.Compare((side, log) =>
    {
        var type = side.T("Mcd.MatVal");
        foreach (int kind in Enumerable.Range(0, 4))
        foreach (bool immutable in new[] { false, true })
        foreach (int index in new[] { -1, 0, 1, 2 })
        foreach (var operation in new[] { "Column", "Row", "real_column", "mcd_column", "mcd_sym_column", "mcd_row", "mcd_sym_row" })
        {
            var matrix = Matrix(side, kind);
            Set(matrix, "Immutable", immutable);
            log.Add($"kind {kind} immutable {immutable} {operation} {index}");
            object? output = null;
            log.Log(side, "view", () => output = operation.StartsWith("mcd_") ? Call(type, operation, matrix, index) : Call(matrix, operation, index));
            if (output?.GetType() == type)
            {
                State(side, log, "output", output);
                log.Add("real aliases " + ReferenceEquals(Get(output, "m_rarr"), Get(matrix, "m_rarr")));
            }
            State(side, log, "input", matrix);
        }
        foreach (uint[] bounds in new uint[][] { [0, 2, 0, 2], [1, 2, 1, 2], [0, 0, 0, 1], [0, 3, 0, 1], [1, 0, 0, 1], [0, 1, 1, 3] })
        foreach (int kind in Enumerable.Range(0, 4))
        {
            var matrix = Matrix(side, kind);
            object? output = null;
            log.Log(side, "submatrix " + string.Join(",", bounds), () => output = Call(matrix, "SubMatrix", bounds.Cast<object?>().ToArray()));
            if (output != null) State(side, log, "output", output);
            State(side, log, "input", matrix);
        }
        foreach (var operation in new[] { "mcd_column", "mcd_sym_column", "mcd_row", "mcd_sym_row" })
            log.Log(side, operation + " wrong type", () => Call(type, operation, "text", 0));
        var source = Matrix(side, 1);
        var destination = New(type, (double[])Get(source, "m_rarr")!, null, 2, 1, 3, null, Enum.ToObject(side.T("Mcd.Flags"), 2146697216), 1);
        log.Log(side, "TrimPtrs", () => Call(destination, "TrimPtrs", Get(source, "m_parr")));
        State(side, log, "trimmed", destination);
    });

    [SkippableFact]
    public void Transpose_and_original_reverse_pointer_mapping() => Scenario.Compare((side, log) =>
    {
        var type = side.T("Mcd.MatVal");
        foreach (int kind in Enumerable.Range(0, 4))
        {
            var matrix = Matrix(side, kind);
            log.Log(side, "TransposeReal", () => Call(matrix, "TransposeReal"));
            log.Log(side, "TransposeImag", () => Call(matrix, "TransposeImag"));
            var output = Call(matrix, "Transpose")!;
            State(side, log, "transposed", output);
            State(side, log, "input", matrix);
            Set(matrix, "m_iarr", null);
            log.Log(side, "absent imaginary", () => Call(matrix, "TransposeImag"));
            var source = New(type, new double[] { 1, 2 });
            Call(source, "set_ptrs_1", 0, "first");
            foreach (int stride in new[] { 2, 3 })
            {
                var target = New(type, 2, 1, false, stride);
                log.Log(side, "ReversePtrs stride " + stride, () => Call(target, "ReversePtrs", source));
                State(side, log, "reversed", target);
            }
        }
    });

    [SkippableFact]
    public void Tuples_equals_text_and_export_lifecycles() => Scenario.Compare((side, log) =>
    {
        var type = side.T("Mcd.MatVal");
        foreach (int kind in Enumerable.Range(0, 4))
        {
            var matrix = Matrix(side, kind);
            log.Log(side, "elements", () => Call(matrix, "get_elements"));
            foreach (uint[] shape in new uint[][] { [1, 2], [2, 1], [0, 2], [3, 1] })
            {
                log.Log(side, "tuple", () => Call(matrix, "asTuple", shape[0], shape[1], 7u));
                log.Log(side, "elements prefix", () => Call(matrix, "get_elements", shape[0], shape[1]));
            }
            var copy = New(type, matrix);
            foreach (object? other in new object?[] { null, "other", copy, New(type, 1), side.Num(1, 0, "L") })
                log.Log(side, "Equals", () => matrix.Equals(other));
            foreach (var fuzz in new[] { 0.0, 0.001 })
                log.Log(side, "nearly_equal", () => Call(matrix, "nearly_equal", copy, fuzz));
            log.Add("text " + matrix);
            foreach (var operation in new[] { "asImmutable", "Fix", "asResult", "FixForExport" })
            {
                var input = Matrix(side, kind);
                Set(input, "m_dim", null);
                object? result = null;
                log.Log(side, operation, () => result = operation == "FixForExport" ? Call(type, operation, input) : CallSig(input, operation, []));
                if (result != null) State(side, log, "output", result);
                log.Add("same " + ReferenceEquals(input, result));
                State(side, log, "input", input);
            }
        }
        foreach (int cols in new[] { 0, 1, 2, 3, 4 })
            log.Add("text " + New(type, 12, cols));
        foreach (object? value in new object?[] { null, "text", 7, side.Num(1, 0, "poly"), side.Str("text") })
        {
            log.Log(side, "Fix object", () => CallSig(type, "Fix", ["Object"], value));
            log.Log(side, "export", () => Call(type, "FixForExport", value));
        }
    });
}
