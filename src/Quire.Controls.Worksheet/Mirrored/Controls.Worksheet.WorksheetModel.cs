using System.Linq;
using Quire.Core;
using Quire.Calculation;
using Quire.Controls.Core;
using System.Windows;

namespace Quire.Controls.Worksheet;

partial class WorksheetModel
{
    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren => _childHider.CalculatableChildren;
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

    private bool _isCalculating;
    private bool _isCalculationsEnabled;
    private global::Quire.Calculation.Core.ICalculatable _lastVisibleCalculatable;

    public bool IsCalculating
    {
        get => _isCalculating;
        set
        {
            if (_isCalculating != value)
            {
                _isCalculating = value;
                FirePropertyChanged(nameof(IsCalculating));
            }
        }
    }

    public string EngineVersion
    {
        get
        {
            if (GetEngineVersion != null)
            {
                return GetEngineVersion();
            }
            return "No Attached Engine";
        }
    }

    public bool SimplifyUnitSystem
    {
        get => _simplifyUnitSystem;
        set
        {
            if (_simplifyUnitSystem == value)
            {
                return;
            }
            _simplifyUnitSystem = value;
            FireSimplifyUnitsFlagChanged(SimplifyUnitSystem);
        }
    }

    public bool IsCalculationsEnabled
    {
        get => _isCalculationsEnabled;
        set
        {
            if (_isCalculationsEnabled == value)
            {
                return;
            }
            _isCalculationsEnabled = value;
            if (!_isCalculationsEnabled)
            {
                FireStopEvaluating();
            }
            else
            {
                FireStartEvaluating();
            }
        }
    }

    public global::Quire.Calculation.Core.ICalculatable LastVisibleCalculatable
    {
        get => _lastVisibleCalculatable;
        set => _lastVisibleCalculatable = value;
    }

    public global::Quire.Calculation.Core.UnitSystems CustomBaseUnitSystem
    {
        get => _customBaseUnitSystem;
        set => _customBaseUnitSystem = value;
    }

    public global::Quire.Calculation.Core.EngineError CurrentError
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    public void HandleScriptingRunWorksheetRecalculate(bool flag) => FireCalculationOptionUpdated("RunScriptingOnRecalculate", flag);

    private void FireCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (CalculatableAdded == null)
        {
            return;
        }
        CalculatableAdded(this, calc);
    }

    private void FireCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calc);
        }
    }

    private void FireCalculatableMoved(global::Quire.Calculation.Core.ICalculatable calc)
    {
        if (CalculatableMoved != null)
        {
            CalculatableMoved(calc);
        }
    }

    private void FireStartEvaluating()
    {
        if (StartEvaluating != null)
        {
            StartEvaluating();
        }
    }

    private void FireStopEvaluating()
    {
        if (StopEvaluating != null)
        {
            StopEvaluating();
        }
    }

    private void FireStartAutoCalc()
    {
        if (StartAutoCalc != null)
        {
            StartAutoCalc();
        }
    }

    private void FireStopAutoCalc()
    {
        if (StopAutoCalc != null)
        {
            StopAutoCalc();
        }
    }

    public WorksheetModel()
    {
        _calculationBinder = new CalculationBinder();
        Initialize();
    }

    // Original unfinished members retain the two constructor closure ordinals.
    private void HandleCollectionChangedForCalculation(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case global::System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                e.NewItems.Cast<object>().Each(HandleItemAdded);
                break;
            case global::System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                e.OldItems.Cast<object>().Each(HandleItemRemoved);
                break;
            case global::System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                e.NewItems.Cast<object>().Each(HandleItemMoved);
                break;
            default:
                global::Quire.DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
    }
    private void HandleItemMoved(object item)
    {
        CommitItemIfNeeded(item as global::Quire.Controls.Core.IWhiteboardItem);
        _childHider.ChildMoved(item);
        if (item is global::Quire.Controls.Core.IORegions.IIORegion region)
        {
            InputOutputRegionManager.HandleElementRemovedFromParentContainer(region);
            InputOutputRegionManager.HandleElementAddedToParentContainer(region);
        }
    }
    private void HandleItemRemoved(object item)
    {
        _childHider.ChildRemoved(item);
        if (item is global::Quire.Controls.Core.IORegions.IIORegion region)
        {
            InputOutputRegionManager.HandleElementRemovedFromParentContainer(region);
        }
    }
    private void HandleItemAdded(object item)
    {
        _childHider.ChildAdded(item);
        if (item is global::Quire.Controls.Core.IORegions.IIORegion region)
        {
            InputOutputRegionManager.HandleElementAddedToParentContainer(region);
        }
    }
    public void CommitItemIfNeeded(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        global::Quire.Calculation.Core.ICalculatable calculatable = item as global::Quire.Calculation.Core.ICalculatable;
        if (calculatable != null)
        {
            global::Quire.Calculation.Core.ICalculatableInfo info = calculatable as global::Quire.Calculation.Core.ICalculatableInfo;
            if (info == null || !info.IsCommitted)
            {
                calculatable.Commit();
            }
        }
        global::Quire.Calculation.Core.ICalculatableContainer container = item as global::Quire.Calculation.Core.ICalculatableContainer;
        if (container != null && !(container is global::Quire.Calculation.Core.ICalculatableAggregate))
        {
            foreach (global::Quire.Calculation.Core.ICalculatable calculatableChild in container.CalculatableChildren)
            {
                global::Quire.Calculation.Core.ICalculatableInfo childInfo = calculatableChild as global::Quire.Calculation.Core.ICalculatableInfo;
                if (childInfo == null || !childInfo.IsCommitted)
                {
                    calculatableChild.Commit();
                }
            }
        }
    }
    public void CommitItem(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void FireCommitAndNotifyDependentCalculatables(global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> toBeCommitted)
    {
        if (CommitAndNotifyDependentCalculatables != null)
        {
            CommitAndNotifyDependentCalculatables(toBeCommitted);
        }
    }
    public void FireWaitForCalculationIdle()
    {
        FireWaitForCalculationIdle(60, global::Quire.Calculation.Core.CalcResultsProcessingPriorities.DefaultResultDispatchPriority_Background);
    }
    public void FireWaitForCalculationIdle(int seconds, global::System.Windows.Threading.DispatcherPriority priority) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void FireBinOctHexModeOptionUpdated(string binOctHexMode, object value)
    {
        global::Quire.EquationEditor.Tree.DefaultDescriptorTable.Default["NUMERIC"].BinOctHexModeOn = (bool)value;
    }
    private void FireCalculationOptionUpdated(string name, object value)
    {
        CalculationOptionUpdated?.Invoke(name, value);
    }
    private void FireSystemVariableUpdated(string name, object value)
    {
        if (SystemVariableUpdated != null)
        {
            SystemVariableUpdated(name, value);
        }
    }
    private void FireUnitSystemUpdated(global::Quire.Calculation.Core.UnitSystems unitSystem)
    {
        if (UnitSystemUpdated != null)
        {
            UnitSystemUpdated(unitSystem, null, null);
        }
    }
    private void HandleUnitSystemUpdated()
    {
        if (UnitSystemUpdated != null)
        {
            if (_defaultUnitSystem == global::Quire.Calculation.Core.UnitSystems.Custom)
            {
                UnitSystemUpdated(_customBaseUnitSystem, CustomBaseUnitDescriptors, CustomDerivedUnitDescriptors);
            }
            else
            {
                UnitSystemUpdated(_defaultUnitSystem, null, null);
            }
        }
    }
    private void FireSimplifyUnitsFlagChanged(bool simplifyUnitSystem)
    {
        if (SimplifyUnitsFlagUpdated != null)
        {
            SimplifyUnitsFlagUpdated(simplifyUnitSystem);
        }
    }
    public void FireRecalculateAll()
    {
        if (RecalculateAllRequested != null)
        {
            RecalculateAllRequested();
        }
    }
    public void FireCalculateNow()
    {
        if (CalculateNowRequested != null)
        {
            CalculateNowRequested(LastVisibleCalculatable);
        }
    }
    public void FireCalculateRegionNow(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        if (item is global::Quire.Calculation.Core.ICalculatable || (item is global::Quire.Calculation.Core.ICalculatableContainer && !(item is global::Quire.Calculation.Core.ICalculatableAggregate)))
        {
            if (item is global::Quire.Controls.Core.IHasPreCalculationAction hasPreCalculationAction)
            {
                hasPreCalculationAction.PreCalculationAction();
            }
            FireStopEvaluating();
            CommitItem(item);
            FireStartEvaluating();
        }
    }
    private void DisposeCalculationContext()
    {
        ContextUnloaded();
        WhiteboardItems.RangeChanged -= HandleCollectionChangedForCalculation;
        if (_docCalculationOptions != null)
        {
            _docCalculationOptions.CalculationOptionChanged -= FireCalculationOptionUpdated;
        }
        if (_docCalculationVariables != null)
        {
            _docCalculationVariables.CalculationVariableChanged -= FireSystemVariableUpdated;
        }
    }
    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
    private void Initialize()
    {
        WhiteboardItems = new global::Quire.Controls.Core.WhiteboardItemsCollection();
        PageBreaks = new global::Quire.Wpf.ObservableRangeCollection<global::Quire.Controls.Core.IWorksheetPageBreak>();
        _itemPositionComparer = new global::Quire.Controls.Worksheet.WorksheetItemPositionComparer();
        InitializeCalculation();
    }
    private void HandleShadowCalculatableCollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void RemoveItem(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        WhiteboardItems.Remove(item);
    }
    public void AddItemAtLocation(object item, global::System.Windows.Point location)
    {
        if (item is global::Quire.Controls.Core.IWorksheetPageBreak worksheetPageBreak)
        {
            ItemManager.SetItemWorksheetLocation(worksheetPageBreak, location);
            PageBreaks.Add(worksheetPageBreak);
            return;
        }
        if (!(item is global::Quire.Controls.Core.IWhiteboardItem whiteboardItem))
        {
            throw new global::System.InvalidOperationException("Bad programmer! Item should be an IWhiteboardItem.");
        }
        ItemManager.SetItemWorksheetLocation(whiteboardItem, location);
        int index = WhiteboardItems.ToList().BinarySearch(whiteboardItem, _itemPositionComparer);
        global::Quire.DebugTools.DevAssert(index <= 0, "The item is already in the list, cannot be added twice");
        WhiteboardItems.Insert(~index, whiteboardItem);
    }
    public void SetItemWorksheetLocation(object item, global::System.Windows.Point newLocation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::System.Windows.Point GetItemWorksheetLocation(object item)
    {
        if (item is global::Quire.Controls.Core.IWhiteboardItem whiteboardItem)
        {
            return ItemManager.GetItemWorksheetLocation(whiteboardItem);
        }
        return ItemManager.GetItemWorksheetLocation(item as global::Quire.Controls.Core.IWorksheetPageBreak);
    }
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
