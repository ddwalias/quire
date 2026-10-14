using Quire.Engine.Internal;
using Quire.Mpl.Run;
using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Engine.Tests;

public class EngineNodeTests : EngineTestBase
{
    [Fact]
    public void Editing_a_region_recomputes_only_it_and_below()
    {
        int a = Engine.AddAfter(0, "(define a 1)", 0, 0);
        int b = Engine.AddAfter(a, "(eval a)", 0, 0);
        int c = Engine.AddAfter(b, "(eval 5)", 0, 0);
        Engine.CalculateNow(0);
        var updated = new List<int>();
        Engine.AddNodeCallback((h, _, _, _, _) => updated.Add(h));

        Engine.Update(b, "(eval (op_mult a 2))", 0, 0);

        Assert.Equal([b, c], updated);
        Assert.Equal(2, ((NumVal)Engine.GetResult(b)!.Value!).Real);
    }

    [Fact]
    public void Moving_a_region_changes_visibility()
    {
        int use = Engine.AddAfter(0, "(eval q)", 0, 0);
        int def = Engine.AddAfter(use, "(define q 7)", 0, 0);
        Assert.NotNull(Engine.GetResult(use)!.Error);

        Engine.MoveBefore(def, use, 0);

        Assert.Equal(7, ((NumVal)Engine.GetResult(use)!.Value!).Real);
    }

    [Fact]
    public void Removing_a_definition_invalidates_users()
    {
        int def = Engine.AddAfter(0, "(define q 7)", 0, 0);
        int use = Engine.AddAfter(def, "(eval q)", 0, 0);
        Engine.Remove(def, 0);
        Assert.NotNull(Engine.GetResult(use)!.Error);
    }

    [Fact]
    public void Changing_a_global_recomputes_regions_above_it()
    {
        int use = Engine.AddAfter(0, "(eval G)", 0, 0);
        int g = Engine.AddAfter(use, "(global G 1)", 0, 0);
        Engine.Update(g, "(global G 2)", 0, 0);
        Assert.Equal(2, ((NumVal)Engine.GetResult(use)!.Value!).Real);
    }

    [Fact]
    public void Auto_calc_off_defers_until_CalculateNow()
    {
        Engine.SetAutoCalc(false);
        int h = Engine.AddAfter(0, "(eval 1)", 0, 0);
        Assert.Null(Engine.GetResult(h));
        Assert.False(Engine.IsUpToDate);
        Engine.CalculateNow(0);
        Assert.True(Engine.IsUpToDate);
        Assert.NotNull(Engine.GetResult(h));
    }

    [Fact]
    public void GetValueBefore_reads_scope_at_a_region()
    {
        int a = Engine.AddAfter(0, "(define w 3)", 0, 0);
        int b = Engine.AddAfter(a, "(define w 4)", 0, 0);
        Assert.Equal(3, ((NumVal)Engine.GetValueBeforeAsync(b, "w")[ResultKeys.Value]!).Real);
        Assert.Equal(1, Engine.IsBoundAt(b, "w"));
        Assert.Equal(0, Engine.IsBoundAt(a, "w"));
    }

    [Fact]
    public void QuickPlot_samples_an_undefined_variable()
    {
        var r = Last("(plot2d (x t) (y (op_power t 2)))");
        var tr = Assert.Single(Assert.IsType<TraceVal>(r.Value).Traces);
        Assert.Equal(Interpreter.QuickPlotPoints, tr.X.Length);
        Assert.Equal(100, tr.Y[0], 9);
    }

    [Fact]
    public void Plot_over_range_with_axis_units()
    {
        var r = Last("(define t (op_range3 (op_mult 0 s) (op_mult 1 s) (op_mult 3 s)))",
                     "(plot2d (x t) (y (op_mult t (op_div m s))) (xunit ms))");
        var tr = Assert.Single(Assert.IsType<TraceVal>(r.Value).Traces);
        Assert.Equal([0.0, 1000, 2000, 3000], tr.X);
        Assert.Equal(Dim.Parse("length"), tr.YDim);
    }

    [Fact]
    public void Sexpr_round_trips_through_reader_and_writer()
    {
        const string src = "(define |a b| (op_div km/h hr) \"text\" 1.5)";
        Assert.Equal(src, Rl.Write(RlReader.Read(src)));
    }
}
