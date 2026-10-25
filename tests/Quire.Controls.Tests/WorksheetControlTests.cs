using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Controls.Plots;
using Quire.Controls.Worksheet;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;
using Quire.PersistentDataObjects;
using MathRegionPersistentData = Quire.PersistentDataObjects.MathRegionPersistentData;
using PlotRegionPersistentData = Quire.PersistentDataObjects.PlotRegionPersistentData;

namespace Quire.Controls.Tests;

/// <summary>Runs a test body on an STA thread with a WPF dispatcher, as the UI does.</summary>
internal static class Sta
{
    public static void Run(Func<Task> body)
    {
        ExceptionDispatchInfo? failure = null;
        var thread = new Thread(() =>
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            var frame = new DispatcherFrame();
            var task = body();
            task.ContinueWith(_ => frame.Continue = false, TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.PushFrame(frame);
            if (task.Exception != null) failure = ExceptionDispatchInfo.Capture(task.Exception.InnerException!);
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (!thread.Join(TimeSpan.FromSeconds(60))) throw new TimeoutException("UI test timed out");
        failure?.Throw();
    }
}

public class WorksheetControlTests
{
    /// <summary>Waits for the engine, then lets the dispatcher deliver results.</summary>
    private static async Task Settle(WorksheetControl ws)
    {
        await ws.WhenCalculated();
        await Dispatcher.Yield(DispatcherPriority.Background);
    }

    private static EquationControl Type(WorksheetControl ws, double x, double y, string text)
    {
        ws.SetCursor(new Point(x, y));
        var region = (EquationControl)ws.InsertMath(text);
        region.EndEdit(commit: true);
        return region;
    }

    private static double Value(CalculatableRegionControl r) =>
        Assert.IsType<UnitedValueResultNode>(r.Result!.Value).DisplayValue;

    [Fact]
    public void Typed_regions_calculate_in_position_order() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        Type(ws, 0, 0, "a := 3 m");
        var eval = Type(ws, 0, 40, "a*2 =");
        await Settle(ws);

        Assert.Equal(6, Value(eval));
        Assert.Equal("m", Assert.Single(((UnitedValueResultNode)((CalculatableRegionControl)eval).Result!.Value).Units).Name);
        Assert.Equal(2, ws.RegionCount);
    });

    [Fact]
    public void Left_to_right_on_the_same_row() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        Type(ws, 0, 0, "q := 2");
        var right = Type(ws, 200, 0, "q =");
        await Settle(ws);
        Assert.Equal(2, Value(right));
    });

    [Fact]
    public void Moving_a_region_above_its_definition_breaks_it() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        Type(ws, 0, 80, "k := 7");
        var use = Type(ws, 0, 160, "k =");
        await Settle(ws);
        Assert.Equal(7, Value(use));

        ws.MoveRegion(use, new Point(0, 0));
        await Settle(ws);
        Assert.Equal("k", ((CalculatableRegionControl)use).Result!.Error!.Symbol);
        Assert.True(use.HasError);
        Assert.Equal(1, ws.ErrorCount);
    });

    [Fact]
    public void Syntax_errors_keep_the_text_and_send_nothing() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        var bad = Type(ws, 0, 0, "x := (1 + ");
        await Settle(ws);
        Assert.True(bad.HasError);
        Assert.Equal("(noop)", bad.Rl);
        Assert.Equal("x := (1 +", bad.SourceText);

        var saved = Assert.IsType<MathRegionPersistentData>(Assert.Single(ws.ToDocument().Regions));
        Assert.Equal("x := (1 +", saved.Source);
    });

    [Fact]
    public void Empty_regions_disappear_after_editing() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        Type(ws, 0, 0, "   ");
        await Dispatcher.Yield(DispatcherPriority.Background);
        Assert.Equal(0, ws.RegionCount);
    });

    [Fact]
    public void Undo_and_redo_restore_the_worksheet() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        Type(ws, 0, 0, "a := 1");
        Type(ws, 0, 40, "b := 2");
        Assert.Equal(2, ws.RegionCount);

        ws.SelectAll();
        ws.DeleteSelection();
        Assert.Equal(0, ws.RegionCount);

        ws.Undo();
        Assert.Equal(2, ws.RegionCount);
        ws.Redo();
        Assert.Equal(0, ws.RegionCount);
        ws.Undo();
        await Settle(ws);
        Assert.Equal(2, ws.ToDocument().Regions.Count);
    });

    [Fact]
    public void Save_and_reload_recalculates() => Sta.Run(async () =>
    {
        WorksheetDocument doc;
        using (var ws = new WorksheetControl())
        {
            Type(ws, 0, 0, "L := 2 ft");
            Type(ws, 0, 40, "L = in");
            ws.SetUnitSystem("uscs");
            doc = ws.ToDocument();
        }
        Assert.Equal("uscs", doc.Settings.UnitSystem);

        using var ws2 = new WorksheetControl();
        ws2.Load(doc);
        await Settle(ws2);
        var eval = ws2.Model.Items.OfType<EquationControl>().Single(e => e.SourceText.StartsWith("L ="));
        Assert.Equal(24, Value(eval), 9);
    });

    [Fact]
    public void Unit_system_switch_redisplays_results() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        Type(ws, 0, 0, "d := 1 m");
        var e = Type(ws, 0, 40, "d =");
        await Settle(ws);
        Assert.Equal(1, Value(e));

        ws.SetUnitSystem("uscs");
        await Settle(ws);
        Assert.Equal(1 / 0.3048, Value(e), 9);
    });

    [Fact]
    public void Solve_block_region_solves_and_exports() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        var data = new SolveBlockRegionPersistentData { Solver = LinearParser.ParseStatement("r := find(x)") };
        data.Guesses.Add(LinearParser.ParseStatement("x := 3"));
        data.Constraints.Add(LinearParser.ParseStatement("x^2 == 2"));
        var doc = new WorksheetDocument();
        data.Top = 0;
        doc.Regions.Add(data);
        doc.Regions.Add(new MathRegionPersistentData { Top = 200, Statement = LinearParser.ParseStatement("r =") });
        ws.Load(doc);
        await Settle(ws);

        var block = ws.Model.Items.OfType<SolveBlockControl>().Single();
        Assert.False(block.HasError);
        var eval = ws.Model.Items.OfType<EquationControl>().Single();
        Assert.Equal(Math.Sqrt(2), Value(eval), 6);
    });

    [Fact]
    public void Plot_region_receives_traces() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        var plot = new PlotRegionPersistentData { X = LinearParser.ParseExpression("z") };
        plot.Y.Add(LinearParser.ParseExpression("z^2"));
        var doc = new WorksheetDocument();
        doc.Regions.Add(plot);
        ws.Load(doc);
        await Settle(ws);

        var control = ws.Model.Items.OfType<XYPlotControl>().Single();
        var traces = Assert.IsAssignableFrom<ITrace2DResultNode>(control.Result!.Value).Traces;
        Assert.Equal(100, Assert.Single(traces).Y[0], 9);
    });

    [Fact]
    public void Text_regions_round_trip_their_content() => Sta.Run(() =>
    {
        using var ws = new WorksheetControl();
        var doc = new WorksheetDocument();
        doc.Regions.Add(new TextRegionPersistentData { Top = 8, SectionXaml = TextRegionPersistentData.PlainSection("Load case 1") });
        ws.Load(doc);
        var text = ws.Model.Items.OfType<TextRegionControl>().Single();
        Assert.Equal("Load case 1", text.PlainText);
        var saved = Assert.IsType<TextRegionPersistentData>(Assert.Single(ws.ToDocument().Regions));
        Assert.Contains("Load case 1", saved.SectionXaml);
        return Task.CompletedTask;
    });

    [Fact]
    public void Result_format_applies_to_existing_results() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        var e = Type(ws, 0, 0, "1/3 =");
        await Settle(ws);
        ws.SetResultFormat(ws.Model.ResultFormat with { Decimals = 6 });
        Assert.Equal(6, e.ResultFormat.Decimals);
        Assert.Equal(6, ws.ToDocument().Settings.Decimals);
    });
}

public class ReviewRegressionControlTests
{
    private static async Task Settle(WorksheetControl ws)
    {
        await ws.WhenCalculated();
        await Dispatcher.Yield(DispatcherPriority.Background);
    }

    [Fact]
    public void Second_solve_block_line_is_kept() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        var block = (SolveBlockControl)ws.InsertSolveBlock();
        var first = block.AddLine(SolveBlockEquationTypes.Definition);
        first.InsertText("x := 1");
        first.EndEdit(commit: true);
        var second = block.AddLine(SolveBlockEquationTypes.Definition);
        second.InsertText("y := 2");
        second.EndEdit(commit: true);
        await Dispatcher.Yield(DispatcherPriority.Background);

        Assert.Contains("(define x 1)", block.Rl);
        Assert.Contains("(define y 2)", block.Rl);

        // ending edit on the whole block with an empty open line must not throw
        block.AddLine(SolveBlockEquationTypes.Constraint).BeginEdit();
        block.EndEdit(commit: true);
        await Dispatcher.Yield(DispatcherPriority.Background);
        await Settle(ws);
    });

    [Fact]
    public void Solve_block_line_with_syntax_error_is_kept_and_flagged() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        var block = (SolveBlockControl)ws.InsertSolveBlock();
        var line = block.AddLine(SolveBlockEquationTypes.Constraint);
        line.InsertText("x + ==");
        line.EndEdit(commit: true);
        await Settle(ws);

        Assert.True(line.HasSyntaxError);
        Assert.Contains("(unparsed)", block.Rl);
        var saved = Assert.IsType<SolveBlockRegionPersistentData>(block.Save());
        Assert.Equal(new UnparsedStatement("x + =="), Assert.Single(saved.Constraints));
    });

    [Fact]
    public void Whitespace_only_edit_keeps_the_result() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        ws.SetCursor(new Point(0, 0));
        var e = (EquationControl)ws.InsertMath("2+3 =");
        e.EndEdit(commit: true);
        await Settle(ws);
        Assert.NotNull(e.DisplayedResult);

        e.Commit("2 + 3 =");
        await Settle(ws);
        Assert.NotNull(e.DisplayedResult);
    });
}

public class StaleResultTests
{
    [Fact]
    public void Recommit_while_pending_does_not_show_an_old_result() => Sta.Run(async () =>
    {
        using var ws = new WorksheetControl();
        ws.SetCursor(new Point(0, 0));
        var def = (EquationControl)ws.InsertMath("x := 5");
        def.EndEdit(commit: true);
        ws.SetCursor(new Point(0, 40));
        var e = (EquationControl)ws.InsertMath("x =");
        e.EndEdit(commit: true);
        await ws.WhenCalculated();
        await Dispatcher.Yield(DispatcherPriority.Background);
        Assert.NotNull(e.DisplayedResult);

        ws.SetAutoCalc(false);
        e.Commit("x+1 =");
        e.Commit("x + 1 =");
        Assert.Null(e.DisplayedResult);
    });
}
