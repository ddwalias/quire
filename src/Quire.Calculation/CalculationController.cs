using System.Collections.Concurrent;
using Quire.Calculation.Core;
using Quire.Engine;
using Quire.Values;

namespace Quire.Calculation;

/// <summary>
/// Binds an <see cref="ICalculatableContext"/> to an <see cref="IEngine"/>. Engine work runs on a dedicated
/// worker thread; results are marshalled back through the captured <see cref="SynchronizationContext"/>.
/// A new edit cancels an in-flight calculation, which resumes from the first out-of-date region.
/// </summary>
public sealed partial class CalculationController : IDisposable
{
    private readonly IEngine _engine;
    private readonly SynchronizationContext? _ui;
    private readonly BlockingCollection<Action> _queue = new();
    private readonly Thread _worker;
    private readonly List<TaskCompletionSource> _idleWaiters = [];
    private readonly object _gate = new();
    private readonly HashSet<ICalculatable> _subscribed = []; // UI thread only
    private bool _disposed;

    // Owned by the worker thread.
    private readonly Dictionary<ICalculatable, int> _handles = [];
    private readonly Dictionary<int, ICalculatable> _calcs = [];
    private readonly Dictionary<int, NodeResult> _lastResults = [];

    private CancellationTokenSource _calcCts = new();
    private ICalculatableContext? _context;
    private volatile UnitSystem _unitSystem = UnitSystem.SI;
    private volatile bool _autoCalc = true;
    private bool _calcRequested;
    private bool _forceRequested; // a manual "calculate now" waiting to run

    public CalculationController(IEngine? engine = null, SynchronizationContext? uiContext = null)
    {
        _engine = engine ?? EngineFactory.CreateEngine();
        _engine.AutoCalc = false;
        _ui = uiContext ?? SynchronizationContext.Current;
        _engine.NodeUpdated += HandleEngineNodeCallback;
        _worker = new Thread(WorkerLoop, maxStackSize: 64 * 1024 * 1024) { IsBackground = true, Name = "Quire calculation" };
        _worker.Start();
    }

    /// <summary>Raised on the UI context when the engine starts (true) or stops (false) calculating.</summary>
    public event EventHandler<bool>? BusyChanged;

    public void BindTo(ICalculatableContext context)
    {
        if (_context != null) throw new InvalidOperationException("Already bound");
        _context = context;
        _unitSystem = UnitSystem.ByName(context.UnitSystem);
        _autoCalc = context.IsAutoCalcOn;

        ICalculatable? prev = null;
        foreach (var c in context.Calculatables)
        {
            HandleCalculatableAdded(context, new CalculatableEventArgs(c, prev));
            prev = c;
        }

        context.CalculatableAdded += HandleCalculatableAdded;
        context.CalculatableRemoved += HandleCalculatableRemoved;
        context.CalculatableMoved += HandleCalculatableMoved;
        context.RecalculateAllRequested += HandleRecalculateAll;
        context.CalculateNowRequested += HandleCalculateNow;
        context.AutoCalcChanged += HandleAutoCalcChanged;
        context.UnitSystemUpdated += HandleUnitSystemUpdated;
    }

    /// <summary>Completes once all queued work (and any automatic calculation) has finished.</summary>
    public Task WhenIdle()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (_gate) _idleWaiters.Add(tcs);
        if (!Enqueue(() => { }, calc: false)) tcs.TrySetResult();
        return tcs.Task;
    }

    private bool Enqueue(Action action, bool calc = true)
    {
        lock (_gate)
        {
            if (_disposed || _queue.IsAddingCompleted) return false;
            if (calc) _calcCts.Cancel();
            _queue.Add(() =>
            {
                action();
                if (calc) _calcRequested = true;
            });
            return true;
        }
    }

    private void HandleCalculatableAdded(object? sender, CalculatableEventArgs e)
    {
        var calc = e.Calculatable;
        var prev = e.Previous;
        var rl = calc.Rl;
        if (_subscribed.Add(calc)) calc.RlChanged += HandleCalculatableUpdated;
        calc.SetSynchronizationStatus(SynchronizationStatus.Pending);
        Enqueue(() =>
        {
            int after = prev != null && _handles.TryGetValue(prev, out var p) ? p : 0;
            int h = prev == null ? _engine.AddRegionAfter(0, rl) : after == 0 ? _engine.AddRegionBefore(0, rl) : _engine.AddRegionAfter(after, rl);
            _handles[calc] = h;
            _calcs[h] = calc;
        });
    }

    private void HandleCalculatableRemoved(object? sender, CalculatableEventArgs e)
    {
        var calc = e.Calculatable;
        calc.RlChanged -= HandleCalculatableUpdated;
        _subscribed.Remove(calc);
        Enqueue(() =>
        {
            if (!_handles.Remove(calc, out var h)) return;
            _calcs.Remove(h);
            _lastResults.Remove(h);
            _engine.RemoveRegion(h);
        });
    }

    private void HandleCalculatableMoved(object? sender, CalculatableEventArgs e)
    {
        var calc = e.Calculatable;
        var prev = e.Previous;
        Enqueue(() =>
        {
            if (!_handles.TryGetValue(calc, out var h)) return;
            int after = prev != null && _handles.TryGetValue(prev, out var p) ? p : 0;
            _engine.MoveAfter(h, after);
        });
    }

    private void HandleCalculatableUpdated(object? sender, EventArgs e)
    {
        if (sender is not ICalculatable calc) return;
        var rl = calc.Rl;
        calc.SetSynchronizationStatus(SynchronizationStatus.Pending);
        Enqueue(() =>
        {
            if (_handles.TryGetValue(calc, out var h)) _engine.UpdateRegion(h, rl);
        });
    }

    private void HandleRecalculateAll(object? sender, EventArgs e) => Enqueue(() => _engine.RecalculateAll());

    private void HandleCalculateNow(object? sender, EventArgs e) =>
        Enqueue(() => RunCalculation(force: true), calc: false);

    private void HandleAutoCalcChanged(object? sender, EventArgs e)
    {
        _autoCalc = _context!.IsAutoCalcOn;
        Enqueue(() => { });
    }

    private void HandleUnitSystemUpdated(object? sender, EventArgs e)
    {
        _unitSystem = UnitSystem.ByName(_context!.UnitSystem);
        Enqueue(() =>
        {
            foreach (var (h, r) in _lastResults) Post(h, r);
        }, calc: false);
    }

    private void WorkerLoop()
    {
        foreach (var action in _queue.GetConsumingEnumerable())
        {
            // the worker must survive anything a single request throws
            try { action(); }
            catch (Exception) { /* stale handle or engine fault: the next calculation re-reports node errors */ }

            if (_queue.Count > 0) continue;
            try
            {
                if (_calcRequested || _forceRequested) RunCalculation(force: _forceRequested);
            }
            catch (Exception) { _calcRequested = _forceRequested = false; }
            if (_queue.Count > 0) continue;

            List<TaskCompletionSource> waiters;
            lock (_gate)
            {
                waiters = [.. _idleWaiters];
                _idleWaiters.Clear();
            }
            foreach (var w in waiters) w.TrySetResult();
        }
    }

    private void RunCalculation(bool force)
    {
        if (!force && !_autoCalc) return;
        if (_engine.IsUpToDate)
        {
            _calcRequested = _forceRequested = false;
            return;
        }
        CancellationToken token;
        lock (_gate)
        {
            // an edit queued after we were scheduled is applied first; it would cancel this pass anyway
            if (_queue.Count > 0)
            {
                if (force) _forceRequested = true;
                else _calcRequested = true;
                return;
            }
            _calcRequested = false;
            _forceRequested = false;
            _calcCts = new CancellationTokenSource();
            token = _calcCts.Token;
        }
        PostBusy(true);
        try { _engine.CalculateNow(token); }
        catch (OperationCanceledException)
        {
            if (force) _forceRequested = true;
            else _calcRequested = true;
        }
        finally { PostBusy(false); }
    }

    private void HandleEngineNodeCallback(int handle, NodeResult result)
    {
        _lastResults[handle] = result;
        Post(handle, result);
    }

    private void Post(int handle, NodeResult result)
    {
        if (!_calcs.TryGetValue(handle, out var calc)) return;
        var transformed = ValueToIResultNodeTransform.Transform(result, _unitSystem);
        void Apply()
        {
            calc.SetResult(transformed);
            calc.SetSynchronizationStatus(SynchronizationStatus.Synchronized);
        }
        if (_ui != null) _ui.Post(_ => Apply(), null);
        else Apply();
    }

    private void PostBusy(bool busy)
    {
        if (_ui != null) _ui.Post(_ => BusyChanged?.Invoke(this, busy), null);
        else BusyChanged?.Invoke(this, busy);
    }

    public void Dispose()
    {
        if (_context != null)
        {
            _context.CalculatableAdded -= HandleCalculatableAdded;
            _context.CalculatableRemoved -= HandleCalculatableRemoved;
            _context.CalculatableMoved -= HandleCalculatableMoved;
            _context.RecalculateAllRequested -= HandleRecalculateAll;
            _context.CalculateNowRequested -= HandleCalculateNow;
            _context.AutoCalcChanged -= HandleAutoCalcChanged;
            _context.UnitSystemUpdated -= HandleUnitSystemUpdated;
        }
        foreach (var calc in _subscribed) calc.RlChanged -= HandleCalculatableUpdated;
        _subscribed.Clear();
        lock (_gate)
        {
            _disposed = true;
            _calcCts.Cancel();
            _queue.CompleteAdding();
            foreach (var w in _idleWaiters) w.TrySetResult();
            _idleWaiters.Clear();
        }
    }
}
