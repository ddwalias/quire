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
    internal static void EnableDebugWrites(bool enable) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static global::Quire.Calculation.CalculationController BindTo(global::Quire.Calculation.Core.ICalculatableContext context) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static void Dispose() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static void UNITTEST_InitBuilder(global::Quire.Calculation.EngineBuilder engineBuilder) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HookupAndInitializeEngine() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HookupContextEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UnhookFromContextEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleEngineUpToDate(bool isUpToDate) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleEngineBusy(bool isBusy) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private string HandleGetEngineVersion() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleStartEvaluating() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleStopEvaluating() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private string DumpGetHandleToCalculatableMap(global::System.Collections.Generic.Dictionary<int, global::Quire.Calculation.Core.ICalculatable> handleToICalculatableMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleContextUnloaded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleRecalculateAll() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleCalculateNow(global::Quire.Calculation.Core.ICalculatable lastVisibleCalc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleStartAutoCalc() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleStopAutoCalc() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void DebugInfo(string info) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleSimplifyUnitSystemUpdated(bool simplifyUnitsInResults) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleUnitSystemUpdated(global::Quire.Calculation.Core.UnitSystems unitSystem, global::System.Collections.Generic.List<global::Quire.Calculation.Core.UnitDescriptor> baseList, global::System.Collections.Generic.List<global::Quire.Calculation.Core.UnitDescriptor> derivedList) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleCalculationOptionUpdated(string name, object value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleSystemVariableUpdated(string name, object value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
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
