using System;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Wpf;
namespace Quire.Calculation;

// Compile before Generated/: original placeholders retain the callback's Mathcad member ordinal.
partial class CalculationController
{
    private static global::Quire.Calculation.EngineBuilder _engineBuilder;
    private string customUnitSystemRl;
    private static global::System.Collections.Generic.List<global::Quire.Calculation.CalculationController> _boundControllers;
    private static readonly global::Quire.Calculation.Core.EmptySimpleResultNode _emptyResult;
    private static bool _enableDebugWrites;
    private readonly global::Quire.Calculation.Core.ICalculatableContext _context;
    private readonly global::Quire.Engine.IEngine _engine;
    private readonly global::Quire.Engine.IBuiltin _builtin;
    private readonly global::System.Windows.Threading.Dispatcher _contextThreadDispatcher;
    private readonly int _importRequestHandle;
    private readonly int _initialHandle;
    private readonly global::System.Collections.Generic.Dictionary<int, global::Quire.Calculation.Core.ICalculatable> _handleToICalculatableMap;
    private readonly global::System.Collections.Generic.Dictionary<global::Quire.Calculation.Core.ICalculatable, global::Quire.Calculation.CalculationController.CalculatableMapInfo> _iCalculatableToHandleMap;
    protected global::Quire.Calculation.CalculationController.EngineOptions _engineOptions;
    private bool _simplifyUnitSystem;
    private bool _evaluatingEnabled;
    private readonly global::System.Threading.ManualResetEvent _waitForCalc;
    private readonly global::System.Threading.ManualResetEvent _interruptWaitForCalc;
    internal static readonly global::System.Windows.Threading.DispatcherPriority WaitForCalculationInterruptPriority_Send;
    [global::System.Diagnostics.Conditional("DEBUG")]
    internal static void EnableDebugWrites(bool enable)
    {
        _enableDebugWrites = enable;
    }
    public static global::Quire.Calculation.CalculationController BindTo(global::Quire.Calculation.Core.ICalculatableContext context) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static void Dispose() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static void UNITTEST_InitBuilder(global::Quire.Calculation.EngineBuilder engineBuilder)
    {
        if (_engineBuilder != null)
        {
            _engineBuilder.Dispose();
        }
        _engineBuilder = engineBuilder;
    }
    private void HookupAndInitializeEngine() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HookupContextEvents()
    {
        _context.GetEngineVersion += HandleGetEngineVersion;
        _context.StartEvaluating += HandleStartEvaluating;
        _context.StopEvaluating += HandleStopEvaluating;
        _context.ContextUnloaded += HandleContextUnloaded;
        _context.RecalculateAllRequested += HandleRecalculateAll;
        _context.CalculateNowRequested += HandleCalculateNow;
        _context.StartAutoCalc += HandleStartAutoCalc;
        _context.StopAutoCalc += HandleStopAutoCalc;
        _context.CommitAndNotifyDependentCalculatables += HandleCommitAndNotifyDependentCalculatables;
        _context.UnitSystemUpdated += HandleUnitSystemUpdated;
        _context.SimplifyUnitsFlagUpdated += HandleSimplifyUnitSystemUpdated;
        _context.CalculationOptionUpdated += HandleCalculationOptionUpdated;
        _context.SystemVariableUpdated += HandleSystemVariableUpdated;
        _context.WaitForCalculationIdle += HandleWaitForCalculationIdle;
        _context.CalculatableAdded += HandleCalculatableAdded;
        _context.CalculatableRemoved += HandleCalculatableRemoved;
        _context.CalculatableMoved += HandleCalculatableMoved;
        _context.GetEngineOption += HandleGetEngineOption;
    }
    private void UnhookFromContextEvents()
    {
        _context.GetEngineVersion -= HandleGetEngineVersion;
        _context.StartEvaluating -= HandleStartEvaluating;
        _context.StopEvaluating -= HandleStopEvaluating;
        _context.ContextUnloaded -= HandleContextUnloaded;
        _context.RecalculateAllRequested -= HandleRecalculateAll;
        _context.CalculateNowRequested -= HandleCalculateNow;
        _context.StartAutoCalc -= HandleStartAutoCalc;
        _context.StopAutoCalc -= HandleStopAutoCalc;
        _context.CommitAndNotifyDependentCalculatables -= HandleCommitAndNotifyDependentCalculatables;
        _context.UnitSystemUpdated -= HandleUnitSystemUpdated;
        _context.SimplifyUnitsFlagUpdated -= HandleSimplifyUnitSystemUpdated;
        _context.CalculationOptionUpdated -= HandleCalculationOptionUpdated;
        _context.SystemVariableUpdated -= HandleSystemVariableUpdated;
        _context.WaitForCalculationIdle -= HandleWaitForCalculationIdle;
        _context.CalculatableAdded -= HandleCalculatableAdded;
        _context.CalculatableRemoved -= HandleCalculatableRemoved;
        _context.CalculatableMoved -= HandleCalculatableMoved;
        _context.GetEngineOption -= HandleGetEngineOption;
    }
    private void HandleEngineUpToDate(bool isUpToDate)
    {
        try
        {
            if (isUpToDate)
            {
                _waitForCalc.Set();
                _interruptWaitForCalc.Set();
            }
            else
            {
                _waitForCalc.Reset();
                _interruptWaitForCalc.Reset();
            }
        }
        catch (Exception ex)
        {
            string message = string.Format("Exception: {0}\n{1}", GetType().Name, ex.Message);
            if (DebugTools.ReplaceAssertionWithException)
            {
                Console.WriteLine(message);
            }
            else
            {
                DebugTools.FailAssertion(message, Array.Empty<object>());
            }
        }
    }
    private void HandleEngineBusy(bool isBusy) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private string HandleGetEngineVersion() => _engine.GetVersion();
    private void HandleStartEvaluating()
    {
        _evaluatingEnabled = true;
        _context.EvaluatingEnabled = _evaluatingEnabled;
        _engine.StartWorkers();
    }
    private void HandleStopEvaluating()
    {
        _evaluatingEnabled = false;
        _context.EvaluatingEnabled = _evaluatingEnabled;
        _engine.StopWorkers();
        _engine.SetSynchronousCallbacks(true);
        _engine.SetSynchronousCallbacks(false);
    }
    private string DumpGetHandleToCalculatableMap(global::System.Collections.Generic.Dictionary<int, global::Quire.Calculation.Core.ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleContextUnloaded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleRecalculateAll()
    {
        if (!_evaluatingEnabled)
        {
            return;
        }
        HandleStopEvaluating();
        _engineOptions.Statuses[EngineStatuses.IsCalculateAll] = true;
        _engine.RecalculateAll();
        HandleStartEvaluating();
    }
    private void HandleCalculateNow(global::Quire.Calculation.Core.ICalculatable lastVisibleCalc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleStartAutoCalc()
    {
        _engine.SetAutoCalc(true);
        _engineOptions.Statuses[EngineStatuses.IsAutoCalc] = _engine.GetAutoCalc();
    }
    private void HandleStopAutoCalc()
    {
        _engine.SetAutoCalc(false);
        _engineOptions.Statuses[EngineStatuses.IsAutoCalc] = _engine.GetAutoCalc();
    }
    [global::System.Diagnostics.Conditional("DEBUG")]
    private static void DebugInfo(string info)
    {
        if (!_enableDebugWrites)
        {
            return;
        }
        string.IsNullOrEmpty(info);
    }
    private void HandleSimplifyUnitSystemUpdated(bool simplifyUnitsInResults)
    {
        _simplifyUnitSystem = simplifyUnitsInResults;
        _engineOptions.ResultsTransform.SimplifyUnits = _simplifyUnitSystem;
        HandleRecalculateAll();
    }
    private void HandleUnitSystemUpdated(global::Quire.Calculation.Core.UnitSystems unitSystem, global::System.Collections.Generic.List<global::Quire.Calculation.Core.UnitDescriptor> baseList, global::System.Collections.Generic.List<global::Quire.Calculation.Core.UnitDescriptor> derivedList) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleCalculationOptionUpdated(string name, object value)
    {
        if (name == "RunScriptingOnRecalculate")
        {
            _engineOptions.Statuses[EngineStatuses.IsRunScriptingOnRecalculate] = (bool)value;
            return;
        }
        _engine.StopWorkers();
        switch (name)
        {
            case "number_of_threads":
                _engine.SetNumThreads((int)value);
                _engineOptions.Statuses[EngineStatuses.IsMultiThreads] = (int)value > 1;
                break;
            case "units_and_constants_in_symbolics":
                _engine.SetOption("units_in_symbolics", value);
                _engine.SetOption("constants_in_symbolics", value);
                break;
            default:
                _engine.SetOption(name, value);
                break;
        }
        if (_evaluatingEnabled)
        {
            _engine.StartWorkers();
        }
    }
    private void HandleSystemVariableUpdated(string name, object value)
    {
        _engine.StopWorkers();
        _builtin.SetVariable(name, value);
        _engineOptions.SystemVariable[name] = value;
        if (_evaluatingEnabled)
        {
            _engine.StartWorkers();
        }
    }
    private void HandleWaitForCalculationIdle(int seconds, global::System.Windows.Threading.DispatcherPriority eventsProcessingPriority) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleCalculatableAdded(global::Quire.Calculation.Core.ICalculatableContainer container, global::Quire.Calculation.Core.ICalculatable arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void AddCalculatableChild(global::Quire.Calculation.Core.ICalculatable parent, global::Quire.Calculation.Core.ICalculatable child) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleCalculatableMoved(global::Quire.Calculation.Core.ICalculatable arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleCommitAndNotifyDependentCalculatables(IEnumerable<ICalculatable> toBeCommitted)
    {
        bool restartWorkers = false;
        if (_evaluatingEnabled)
        {
            _engine.StopWorkers();
            restartWorkers = true;
        }
        bool synchronousCallbacks = _engine.GetSynchronousCallbacks();
        _engine.SetSynchronousCallbacks(true);
        toBeCommitted.Each((ICalculatable calculatable) => calculatable.Commit());
        _engine.SetSynchronousCallbacks(synchronousCallbacks);
        WpfUtils.DoEvents(_contextThreadDispatcher, (System.Windows.Threading.DispatcherPriority)10);
        if (restartWorkers)
            _engine.StartWorkers();
    }
}
