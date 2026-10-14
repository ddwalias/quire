using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValGrowthDifferentialTests
{
    private static void State(Side s, List<string> log, string label, object m) => log.Add(label + " " + MatValDifferentialTests.Storage(s, m));

    [SkippableFact]
    public void Growth_capacity_and_copy_on_write() => Scenario.Compare((s, log) =>
    {
        var type = s.T("Mcd.MatVal");
        var vector = New(type, new double[] { 1, 2 });
        Call(vector, "mcd_set_1", 2, New(s.T("Mcd.NumVal"), 3.0));
        State(s, log, "vector growth", vector);
        Set(vector, "Immutable", true);
        Set(vector, "Shared", true);
        var copy = Call(vector, "mcd_set_1", 4, New(s.T("Mcd.NumVal"), 5.0))!;
        State(s, log, "original", vector);
        State(s, log, "copy", copy);
        CallSig(vector, "mcd_upd_1", ["Int32", "Double"], [0, 9.0]);
        State(s, log, "update immutable", vector);
        var matrix = New(type, 2, 2);
        Call(matrix, "set_ptrs_2", 1, 1, "pointer");
        Call(matrix, "mcd_set_2", 2, 1, New(s.T("Mcd.NumVal"), 7.0));
        State(s, log, "row growth", matrix);
        Call(matrix, "mcd_set_2", 3, 2, New(s.T("Mcd.NumVal"), 8.0));
        State(s, log, "column growth", matrix);
        State(s, log, "capacity only", Call(type, "realloc", matrix, 5, 4)!);
        State(s, log, "empty capacity", Call(type, "realloc", null, -1, -1)!);
        foreach (var dim in Side.DimNames)
        {
            var pointer = New(type, 1, 1);
            Set(pointer, "m_dim", s.D(dim));
            Call(pointer, "set_ptrs_2", 0, 0, "pointer");
            Call(pointer, "extend_2", 1, 1);
            State(s, log, "unit growth " + dim, pointer);
        }
    });

    [SkippableFact]
    public void Assignment_blocks_columns_and_errors() => Scenario.Compare((s, log) =>
    {
        var type = s.T("Mcd.MatVal");
        foreach (var dim in Side.DimNames)
        {
            var source = New(type, new double[] { 1, 2, 3, 4 }, new double[] { 5, 6, 7, 8 }, 2, 2);
            Set(source, "m_dim", s.D(dim));
            Call(source, "set_ptrs_2", 1, 1, "nested");
            var destination = New(type, 1, 1);
            Call(destination, "setblock", 1, 1, source);
            State(s, log, "block " + dim, destination);
            Call(destination, "setcolumn", 1, New(type, new double[] { 10, 11 }));
            State(s, log, "column " + dim, destination);
            State(s, log, "row " + dim, Call(type, "mcd_setrow", null, 1, source)!);
        }
        var scalar = New(s.T("Mcd.NumVal"), 6.0);
        var immutable = New(type, 1, 1);
        Set(immutable, "Immutable", true);
        State(s, log, "column copy", Call(type, "mcd_setcolumn", immutable, 2, scalar)!);
        State(s, log, "original", immutable);
        var fill = New(type, new double[] { 2, 3 });
        Call(fill, "StretchWithColumnFill", 4, 3, scalar);
        State(s, log, "fill new columns", fill);
        log.Log(s, "vector shape error", () => Call(New(type, 2, 2), "extend_1", 0));
        log.Log(s, "row error", () => Call(immutable, "mcd_set_1", -1, scalar));
        log.Log(s, "column error", () => Call(immutable, "mcd_set_2", 0, -1, scalar));
        log.Log(s, "source vector error", () => Call(immutable, "setcolumn", 0, New(type, 1, 2)));
    });
}
