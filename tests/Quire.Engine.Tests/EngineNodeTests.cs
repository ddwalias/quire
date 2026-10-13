using Quire.Engine;
using Quire.Mpl.Rl;
using Quire.Values;

namespace Quire.Engine.Tests;

public class EngineNodeTests : EngineTestBase
{
    [Fact]
    public void Editing_a_region_recomputes_only_it_and_below()
    {
        int a = Engine.AddRegionAfter(0, "(define a 1)");
        int b = Engine.AddRegionAfter(a, "(eval a)");
        int c = Engine.AddRegionAfter(b, "(eval 5)");
        Engine.CalculateNow();
        var updated = new List<int>();
        Engine.NodeUpdated += (h, _) => updated.Add(h);

        Engine.UpdateRegion(b, "(eval (op_mult a 2))");

        Assert.Equal([b, c], updated);
        Assert.Equal(2, ((NumVal)Engine.GetResult(b)!.Value!).V);
    }

    [Fact]
    public void Moving_a_region_changes_visibility()
    {
        int use = Engine.AddRegionAfter(0, "(eval q)");
        int def = Engine.AddRegionAfter(use, "(define q 7)");
        Assert.NotNull(Engine.GetResult(use)!.Error);

        Engine.MoveBefore(def, use);

        Assert.Equal(7, ((NumVal)Engine.GetResult(use)!.Value!).V);
    }

    [Fact]
    public void Removing_a_definition_invalidates_users()
    {
        int def = Engine.AddRegionAfter(0, "(define q 7)");
        int use = Engine.AddRegionAfter(def, "(eval q)");
        Engine.RemoveRegion(def);
        Assert.NotNull(Engine.GetResult(use)!.Error);
    }

    [Fact]
    public void Changing_a_global_recomputes_regions_above_it()
    {
        int use = Engine.AddRegionAfter(0, "(eval G)");
        int g = Engine.AddRegionAfter(use, "(global G 1)");
        Engine.UpdateRegion(g, "(global G 2)");
        Assert.Equal(2, ((NumVal)Engine.GetResult(use)!.Value!).V);
    }

    [Fact]
    public void Auto_calc_off_defers_until_CalculateNow()
    {
        Engine.AutoCalc = false;
        int h = Engine.AddRegionAfter(0, "(eval 1)");
        Assert.Null(Engine.GetResult(h));
        Assert.False(Engine.IsUpToDate);
        Engine.CalculateNow();
        Assert.True(Engine.IsUpToDate);
        Assert.NotNull(Engine.GetResult(h));
    }

    [Fact]
    public void GetValueBefore_reads_scope_at_a_region()
    {
        int a = Engine.AddRegionAfter(0, "(define w 3)");
        int b = Engine.AddRegionAfter(a, "(define w 4)");
        Assert.Equal(3, ((NumVal)Engine.GetValueBefore(b, "w")!).V);
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
        Assert.Equal(Dim.Length, tr.YDim);
    }

    [Fact]
    public void Sexpr_round_trips_through_reader_and_writer()
    {
        const string src = "(define |a b| (op_div km/h hr) \"text\" 1.5)";
        Assert.Equal(src, SexprReader.Read(src).ToString());
    }
}
