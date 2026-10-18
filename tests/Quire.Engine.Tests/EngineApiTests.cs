using System.Collections;
using Quire.Engine.Internal;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Engine.Tests;

/// <summary>The engine through Mathcad's <see cref="IEngine"/> surface, as the calculation controller uses it.</summary>
public class EngineApiTests
{
    private readonly IEngine _engine = new Factory().NewFactory().NewEngine();

    [Fact]
    public void Factory_creates_engines_and_tracks_them()
    {
        using var factory = new Factory().NewFactory();
        var e = factory.NewEngine();
        Assert.Contains(e, factory.GetEngines().Cast<IEngine>());
    }

    [Fact]
    public void Results_come_back_as_dictionaries()
    {
        int a = _engine.AddAfter(0, "(define x (op_mult 3 m))", 0, 0);
        int b = _engine.AddAfter(a, "(eval x km)", 0, 0);
        var r = _engine.GetValueSync(b, 0);
        Assert.Equal(NodeStatus.NODE_READY, r[ResultKeys.Status]);
        Assert.Equal(3.0, ((NumVal)r[ResultKeys.Value]!).Real);
        Assert.Equal(1000.0, r[ResultKeys.DisplayScale]);
        Assert.Equal(["x"], (string[])_engine.GetValueSync(a, 0)[ResultKeys.Defined]!);
        Assert.Equal(["x"], _engine.GetDefined(a).Cast<string>());
    }

    [Fact]
    public void Errors_and_children_are_reported()
    {
        int h = _engine.AddAfter(0, "(eval nope)", 0, 0);
        var r = _engine.GetValueSync(h, 0);
        Assert.Equal(NodeStatus.NODE_INVALID, r[ResultKeys.Status]);
        Assert.Equal("nope", r[ResultKeys.Symbol]);

        int s = _engine.AddAfter(h, "(solveblock (guesses (define x 1)) (constraints (op_eq (op_mult 2 x) 6)) (solver (eval (op_apply find x))))", 0, 0);
        var children = (IList)_engine.GetValueSync(s, 0)[ResultKeys.Children]!;
        Assert.Equal(3, children.Count);
        Assert.Equal(3.0, ((NumVal)((IDictionary)children[2]!)[ResultKeys.Value]!).Real, 6);
    }

    [Fact]
    public void Callbacks_carry_status_and_client_cookie()
    {
        var seen = new List<(int H, NodeStatus S, int Cookie)>();
        _engine.AddNodeCallback((h, s, cookie, _, _) => seen.Add((h, s, cookie)));
        int a = _engine.AddAfter(0, "(eval 1)", 0, 41);
        int b = _engine.AddAfter(a, "(eval undefined_name)", 0, 42);
        Assert.Contains((a, NodeStatus.NODE_READY, 41), seen);
        Assert.Contains((b, NodeStatus.NODE_INVALID, 42), seen);
    }

    [Fact]
    public void Busy_and_up_to_date_events_bracket_a_calculation()
    {
        var events = new List<string>();
        _engine.OnBusy += busy => events.Add(busy ? "busy" : "idle");
        _engine.OnUpToDate += _ => events.Add("up-to-date");
        _engine.AddAfter(0, "(eval 1)", 0, 0);
        Assert.Equal(["busy", "idle", "up-to-date"], events);
        Assert.False(_engine.IsBusy);
    }

    [Fact]
    public void Manual_mode_defers_and_replace_forces_recalculation()
    {
        _engine.SetAutoCalc(false);
        Assert.False(_engine.GetAutoCalc());
        int h = _engine.AddAfter(0, "(eval 2)", 7, 0);
        Assert.Equal(NodeStatus.NODE_NOT_COMPUTING, _engine.GetValueSync(h, 0)[ResultKeys.Status]);
        Assert.Equal(7, _engine.GetPriority(h));

        int updates = 0;
        _engine.AddNodeCallback((_, _, _, _, _) => updates++);
        _engine.CalculateNow(0);
        _engine.Update(h, "(eval 2)", 7, 0);
        _engine.CalculateNow(0);
        Assert.Equal(1, updates);
        _engine.Update(h, "(eval 3)", 7, 0);
        var stale = _engine.GetValueSync(h, 0);
        Assert.Equal(NodeStatus.NODE_NOT_COMPUTING, stale[ResultKeys.Status]);
        Assert.Equal(2.0, ((NumVal)stale[ResultKeys.Value]!).Real);
        _engine.CalculateNow(0);
        Assert.Equal(NodeStatus.NODE_READY, _engine.GetValueSync(h, 0)[ResultKeys.Status]);
        Assert.Equal(2, updates);
        _engine.Replace(h, "(eval 3)", 7, 0);
        _engine.CalculateNow(0);
        Assert.Equal(3, updates);
    }

    [Fact]
    public void Calculation_stops_when_the_thread_is_cancelled()
    {
        _engine.SetAutoCalc(false);
        int h = _engine.AddAfter(0, "(eval (op_sigmasum k k 1 900000))", 0, 0);
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        Rts.RegisterThreadForCancellationChecking(cts);
        try
        {
            Assert.Throws<OperationCanceledException>(() => _engine.CalculateNow(0));
        }
        finally { Rts.RegisterThreadForCancellationChecking(null!); }
        _engine.CalculateNow(0);
        Assert.Equal(405000450000.0, ((NumVal)_engine.GetValueSync(h, 0)[ResultKeys.Value]!).Real);
    }

    [Fact]
    public void An_edit_on_a_cancelled_thread_still_takes_effect()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        Rts.RegisterThreadForCancellationChecking(cts);
        int h;
        try { h = _engine.AddAfter(0, "(eval 5)", 0, 0); }
        finally { Rts.RegisterThreadForCancellationChecking(null!); }
        Assert.Equal([h], _engine.GetTopNodes().Cast<int>());
        Assert.Equal(NodeStatus.NODE_NOT_COMPUTING, _engine.GetValueSync(h, 0)[ResultKeys.Status]);
        _engine.CalculateNow(0);
        Assert.Equal(5.0, ((NumVal)_engine.GetValueSync(h, 0)[ResultKeys.Value]!).Real);
    }

    [Fact]
    public void Top_nodes_follow_worksheet_order()
    {
        int a = _engine.AddAfter(0, "(eval 1)", 0, 0);
        int b = _engine.AddBefore(0, "(eval 2)", 0, 0);
        int c = _engine.AddBefore(b, "(eval 3)", 0, 0);
        _engine.MoveAfter(a, b, 0);
        Assert.Equal([c, b, a], _engine.GetTopNodes().Cast<int>());
        _engine.Remove(c, 0);
        Assert.Equal([b, a], _engine.GetTopNodes().Cast<int>());
    }

    [Fact]
    public void Factory_AppendPath_appends_to_assembly_path()
    {
        string original = Rts.AssemblyPath;
        try
        {
            Rts.AssemblyPath = "";
            var factory = new Factory();
            factory.AppendPath("dir1");
            Assert.Equal("dir1", Rts.AssemblyPath);
            factory.AppendPath("dir2");
            Assert.Equal("dir1;dir2", Rts.AssemblyPath);
            factory.AppendPath(null!);
            Assert.Equal("dir1;dir2", Rts.AssemblyPath);
        }
        finally
        {
            Rts.AssemblyPath = original;
        }
    }

    [Fact]
    public void Factory_Initialize_and_VerifyIntegrity()
    {
        using var factory = new Factory().NewFactory();
        factory.Initialize("testAppData");
        Assert.True(factory.VerifyIntegrity());
    }

    [Fact]
    public void Engine_IsUpToDate_and_RecalculateAll()
    {
        var engine = new Factory().NewFactory().NewEngine();
        Assert.False(engine.IsUpToDate);
        engine.CalculateNow(0);
        Assert.True(engine.IsUpToDate);

        engine.SetAutoCalc(false);
        int h = engine.AddAfter(0, "(eval 42)", 0, 0);
        Assert.False(engine.IsUpToDate);
        // Under manual calc, RecalculateAll marks dirty without computing
        engine.RecalculateAll();
        Assert.False(engine.IsUpToDate);
        // CalculateNow brings it up to date
        engine.CalculateNow(0);
        Assert.True(engine.IsUpToDate);
        // Under auto calc, RecalculateAll recomputes immediately
        engine.SetAutoCalc(true);
        engine.RecalculateAll();
        Assert.True(engine.IsUpToDate);
        var r = engine.GetValueSync(h, 0);
        Assert.Equal(NodeStatus.NODE_READY, r[ResultKeys.Status]);
        Assert.Equal(42.0, ((NumVal)r[ResultKeys.Value]!).Real);
    }
}
