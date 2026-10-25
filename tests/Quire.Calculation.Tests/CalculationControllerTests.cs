using Quire.Calculation;
using Quire.Calculation.Core;

namespace Quire.Calculation.Tests;

internal sealed class FakeCalculatable(string rl) : ICalculatable
{
    private string _rl = rl;
    public string Rl
    {
        get => _rl;
        set { _rl = value; RlChanged?.Invoke(this, EventArgs.Empty); }
    }
    public event EventHandler? RlChanged;
    public SynchronizationStatus SynchronizationStatus { get; private set; }
    public CalculationResult? Result { get; private set; }
    public void SetSynchronizationStatus(SynchronizationStatus status) => SynchronizationStatus = status;
    public void SetResult(CalculationResult result) => Result = result;
    public double Value => ((IUnitedValueResultNode)Result!.Value).DisplayValue;

    // Mathcad's ICalculatable members; the controller under test does not use them.
    event Action<ICalculatable> ICalculatable.RequestEvaluation { add { } remove { } }
    event Func<ICalculatable, string, IResultNode> ICalculatable.GetValueOfIdentifierBefore { add { } remove { } }
    event Action<ICalculatable> ICalculatable.TreatAsUpToDate { add { } remove { } }
    void ICalculatable.Commit() { }
    void ICalculatable.SetStatusSynchronized() { }
    void ICalculatable.SetCalculatingStatus(CalculatingStatus computingStatus) { }
    void ICalculatable.SetResult(IResultNode result) { }
    void ICalculatable.SetError(EngineError error) { }
    void ICalculatable.SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap) { }
    void ICalculatable.SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap) { }
    void ICalculatable.SetRedefinitionWarnings(IList<EngineRedefInfo> warnings) { }
}

internal sealed class FakeContext : ICalculatableContext
{
    private readonly List<ICalculatable> _items = [];
    public IReadOnlyList<ICalculatable> Calculatables => _items;
    public event EventHandler<CalculatableEventArgs>? CalculatableAdded;
    public event EventHandler<CalculatableEventArgs>? CalculatableRemoved;
    public event EventHandler<CalculatableEventArgs>? CalculatableMoved;
    public event EventHandler? RecalculateAllRequested;
    public event EventHandler? CalculateNowRequested;
    public event EventHandler? AutoCalcChanged;
    public event EventHandler? UnitSystemUpdated;
    public bool IsAutoCalcOn { get; private set; } = true;
    public string UnitSystem { get; private set; } = "si";

    public FakeCalculatable Add(string rl, int? index = null)
    {
        var c = new FakeCalculatable(rl);
        int i = index ?? _items.Count;
        _items.Insert(i, c);
        CalculatableAdded?.Invoke(this, new CalculatableEventArgs(c, i == 0 ? null : _items[i - 1]));
        return c;
    }

    public void Remove(ICalculatable c)
    {
        int i = _items.IndexOf(c);
        _items.RemoveAt(i);
        CalculatableRemoved?.Invoke(this, new CalculatableEventArgs(c, i == 0 ? null : _items[i - 1]));
    }

    public void Move(ICalculatable c, int index)
    {
        _items.Remove(c);
        _items.Insert(index, c);
        CalculatableMoved?.Invoke(this, new CalculatableEventArgs(c, index == 0 ? null : _items[index - 1]));
    }

    public void SetUnitSystem(string name) { UnitSystem = name; UnitSystemUpdated?.Invoke(this, EventArgs.Empty); }
    public void SetAutoCalc(bool on) { IsAutoCalcOn = on; AutoCalcChanged?.Invoke(this, EventArgs.Empty); }
    public void CalculateNow() => CalculateNowRequested?.Invoke(this, EventArgs.Empty);
    public void RecalculateAll() => RecalculateAllRequested?.Invoke(this, EventArgs.Empty);

    // Mathcad's ICalculatableContext/ICalculatableContainer members; the controller under test does not use them.
    bool ICalculatableContext.IsCalculating { get; set; }
    bool ICalculatableContext.EvaluatingEnabled { get; set; }
    event Func<string> ICalculatableContext.GetEngineVersion { add { } remove { } }
    event Action ICalculatableContext.StartEvaluating { add { } remove { } }
    event Action ICalculatableContext.StopEvaluating { add { } remove { } }
    event Action<bool> ICalculatableContext.SimplifyUnitsFlagUpdated { add { } remove { } }
    event Action<string, object> ICalculatableContext.CalculationOptionUpdated { add { } remove { } }
    event Action<string, object> ICalculatableContext.SystemVariableUpdated { add { } remove { } }
    event Action ICalculatableContext.StartAutoCalc { add { } remove { } }
    event Action ICalculatableContext.StopAutoCalc { add { } remove { } }
    event Action<IEnumerable<ICalculatable>> ICalculatableContext.CommitAndNotifyDependentCalculatables { add { } remove { } }
    event Action ICalculatableContext.ContextUnloaded { add { } remove { } }
    event Action<int, System.Windows.Threading.DispatcherPriority> ICalculatableContext.WaitForCalculationIdle { add { } remove { } }
    event Func<string, object> ICalculatableContext.GetEngineOption { add { } remove { } }
    IEnumerable<ICalculatable> ICalculatableContainer.CalculatableChildren => _items;
    event Action<ICalculatableContainer, ICalculatable> ICalculatableContainer.CalculatableAdded { add { } remove { } }
    event Action<ICalculatable> ICalculatableContainer.CalculatableRemoved { add { } remove { } }
    event Action<ICalculatable> ICalculatableContainer.CalculatableMoved { add { } remove { } }
}

public class CalculationControllerTests
{
    private static async Task<(FakeContext, CalculationController)> Bound()
    {
        var ctx = new FakeContext();
        var ctl = new CalculationController(uiContext: null);
        ctl.BindTo(ctx);
        await ctl.WhenIdle();
        return (ctx, ctl);
    }

    [Fact]
    public async Task Results_flow_back_to_calculatables()
    {
        var (ctx, ctl) = await Bound();
        ctx.Add("(define a (op_mult 2 km))");
        var e = ctx.Add("(eval a)");
        await ctl.WhenIdle();

        Assert.Equal(SynchronizationStatus.Synchronized, e.SynchronizationStatus);
        Assert.Equal(2000, e.Value);
        Assert.Equal("m", Assert.Single(((UnitedValueResultNode)e.Result!.Value).Units).Name);
    }

    [Fact]
    public async Task Editing_and_moving_recalculate()
    {
        var (ctx, ctl) = await Bound();
        var def = ctx.Add("(define a 1)");
        var e = ctx.Add("(eval a)");
        await ctl.WhenIdle();

        def.Rl = "(define a 5)";
        await ctl.WhenIdle();
        Assert.Equal(5, e.Value);

        ctx.Move(e, 0);
        await ctl.WhenIdle();
        Assert.NotNull(e.Result!.Error);
    }

    [Fact]
    public async Task An_unchanged_edit_does_not_stay_pending()
    {
        var (ctx, ctl) = await Bound();
        var e = ctx.Add("(eval 3)");
        await ctl.WhenIdle();

        e.Rl = "(eval 3)";
        Assert.Equal(SynchronizationStatus.Pending, e.SynchronizationStatus);
        await ctl.WhenIdle();
        Assert.Equal(SynchronizationStatus.Synchronized, e.SynchronizationStatus);
        Assert.Equal(3, e.Value);
    }

    [Fact]
    public async Task Removing_a_definition_reports_undefined()
    {
        var (ctx, ctl) = await Bound();
        var def = ctx.Add("(define a 1)");
        var e = ctx.Add("(eval a)");
        await ctl.WhenIdle();
        ctx.Remove(def);
        await ctl.WhenIdle();
        Assert.Equal("a", e.Result!.Error!.Symbol);
    }

    [Fact]
    public async Task Unit_system_change_reformats_without_recalculation()
    {
        var (ctx, ctl) = await Bound();
        var e = ctx.Add("(eval (op_mult 1 m))");
        await ctl.WhenIdle();
        ctx.SetUnitSystem("uscs");
        await ctl.WhenIdle();
        Assert.Equal(1 / 0.3048, e.Value, 9);
        Assert.Equal("ft", Assert.Single(((UnitedValueResultNode)e.Result!.Value).Units).Name);
    }

    [Fact]
    public async Task Manual_mode_waits_for_calculate_now()
    {
        var (ctx, ctl) = await Bound();
        ctx.SetAutoCalc(false);
        var e = ctx.Add("(eval 7)");
        await ctl.WhenIdle();
        Assert.Null(e.Result);
        Assert.Equal(SynchronizationStatus.Pending, e.SynchronizationStatus);

        ctx.CalculateNow();
        await ctl.WhenIdle();
        Assert.Equal(7, e.Value);
    }

    [Fact]
    public async Task Long_calculation_is_cancelled_by_a_new_edit()
    {
        var (ctx, ctl) = await Bound();
        var slow = ctx.Add("(eval (op_sigmasum (op_apply sin k) k 1 900000))");
        slow.Rl = "(eval 1)";
        await ctl.WhenIdle();
        Assert.Equal(1, slow.Value);
    }
}

public class ControllerDisposalTests
{
    [Fact]
    public async Task Edits_after_dispose_are_ignored()
    {
        var ctx = new FakeContext();
        var ctl = new CalculationController(uiContext: null);
        ctl.BindTo(ctx);
        var c = ctx.Add("(eval 1)");
        await ctl.WhenIdle();
        ctl.Dispose();

        c.Rl = "(eval 2)"; // must not throw
        ctx.Add("(eval 3)");
        await ctl.WhenIdle(); // completes immediately
    }
}
