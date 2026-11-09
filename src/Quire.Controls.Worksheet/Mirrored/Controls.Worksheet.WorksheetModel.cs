using Quire.Calculation;
using Quire.Controls.Core;
using System.Windows;

namespace Quire.Controls.Worksheet;

partial class WorksheetModel
{
    public event Action ContextUnloaded = () => { };
    private bool _simplifyUnitSystem = true;
    private bool _isAutoCalcOn = true;
    public event Action<IWhiteboardItem, Point> ItemMoved = delegate { };

    public bool EvaluatingEnabled { get; set; }
    public global::System.Collections.Generic.List<global::Quire.Calculation.Core.UnitDescriptor> CustomBaseUnitDescriptors { get; set; }
    public global::System.Collections.Generic.List<global::Quire.Calculation.Core.UnitDescriptor> CustomDerivedUnitDescriptors { get; set; }
    public global::Quire.Controls.Core.WhiteboardItemsCollection WhiteboardItems { get; private set; }
    public global::Quire.Controls.Core.ITextStylesManager TextStylesManager { get; set; }
    public global::Quire.Wpf.ObservableRangeCollection<global::Quire.Controls.Core.IWorksheetPageBreak> PageBreaks { get; private set; }
    public global::Quire.Controls.Core.BookmarkTags.IBookmarksManager BookmarksManager { get; set; }

    public WorksheetModel()
    {
        _calculationBinder = new CalculationBinder();
        Initialize();
    }

    // Original unfinished members retain the two constructor closure ordinals.
    private void HandleCollectionChangedForCalculation(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleItemMoved(object item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleItemRemoved(object item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleItemAdded(object item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void CommitItemIfNeeded(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void CommitItem(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void FireCommitAndNotifyDependentCalculatables(global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> toBeCommitted) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void FireWaitForCalculationIdle() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void FireWaitForCalculationIdle(int seconds, global::System.Windows.Threading.DispatcherPriority priority) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireBinOctHexModeOptionUpdated(string binOctHexMode, object value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireCalculationOptionUpdated(string name, object value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireSystemVariableUpdated(string name, object value) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireUnitSystemUpdated(global::Quire.Calculation.Core.UnitSystems unitSystem) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleUnitSystemUpdated() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireSimplifyUnitsFlagChanged(bool simplifyUnitSystem) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void FireRecalculateAll() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void FireCalculateNow() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void FireCalculateRegionNow(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void DisposeCalculationContext() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FirePropertyChanged(string propertyName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void Initialize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleShadowCalculatableCollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void RemoveItem(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void AddItemAtLocation(object item, global::System.Windows.Point location) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetItemWorksheetLocation(object item, global::System.Windows.Point newLocation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Windows.Point GetItemWorksheetLocation(object item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::Quire.Controls.Core.IWhiteboardItem FirstItemInDirection(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items, global::Quire.Direction direction, global::System.Windows.Point startPoint) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public bool ContainsError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void ShowError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool IsAutoCalcOn
    {
        get => _isAutoCalcOn;
        set
        {
            _isAutoCalcOn = value;
            if (_isAutoCalcOn)
                FireStartAutoCalc();
            else
                FireStopAutoCalc();
        }
    }

    public WorksheetModel(CalculationBinder binder)
    {
        _calculationBinder = binder;
        Initialize();
    }
}
