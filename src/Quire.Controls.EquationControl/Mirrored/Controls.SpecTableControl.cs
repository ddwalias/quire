#nullable disable
using System.Windows.Controls;
using Quire.Calculation.Core;
using Quire.EquationEditor;
using Quire.Controls.Core;
using Quire.EquationEditor.SpecTable;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Whiteboard;
using Quire.Matrices;
using Quire.PersistentData;
using Quire.Undo;
using System.Windows;
using Quire.EquationEditor.SpecTable.Model;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.ComponentModel;
using Quire.Services;

namespace Quire.Controls;

[global::Quire.Mirror.MirrorOf("Ptc.Controls.SpecTableControl, Ptc.Controls.EquationControl")]
public partial class SpecTableControl : global::System.Windows.Controls.ContentControl, global::Quire.Controls.ISpecTableControl, global::Quire.Controls.Core.IDitaRegion, global::Quire.Serialization.IPersistentDataProvider, global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculationHideableChild, global::System.ComponentModel.INotifyPropertyChanged, global::Quire.Controls.Core.IDelegateFocus, global::Quire.Controls.Core.ISearchableRegion, global::Quire.Controls.Core.IWhiteboardItem, global::Quire.Controls.Core.IHelpContext, global::Quire.Calculation.Core.ICalculationErrorOwner, global::Quire.Controls.Core.IORegions.IInputRegion, global::Quire.Controls.Core.IORegions.IIORegion, global::Quire.Controls.Core.IORegions.IIORegionBase, global::Quire.Controls.Core.IORegions.IIOCalculatableContainerRegion, global::Quire.Controls.ICurrentIdLabelProperty, global::Quire.Controls.Core.IMathFormatOwner, global::Quire.Controls.Core.ICurrentMathFormatProperty, global::Quire.Controls.Core.IMathFormatForDeserialization, global::Quire.Controls.Core.ISupportCustomInitialization, global::Quire.Controls.Core.IHasChildArrangeInfoProvider
{
    private static readonly Type _typeOfThis;

    public static readonly DependencyProperty CalcIsDisabledProperty;

    public static readonly DependencyProperty SynchronizationStatusProperty;

    public static readonly DependencyProperty CalculatingStatusProperty;

    private const string FirstPropertyForXamlSerializationPropertyName = "FirstPropertyForXamlSerialization";

    private readonly global::Quire.Calculation.Core.Support.CalculatableChildHider _childHider;

    internal readonly global::Quire.Controls.SpecTableView View;

    internal readonly global::Quire.EquationEditor.SpecTable.ISpecTableModel Model;

    internal readonly global::Quire.EquationEditor.SpecTable.SpecTableCellMap Map;

    private global::Quire.Controls.EquationControl _currentCell;

    public static readonly RoutedEvent ChangedEvent;

    private global::Quire.Calculation.Core.EngineResult[] _engineResults;

    private global::System.Nullable<global::Quire.Controls.Core.IdLabels> _currentIdLabel;

    private global::System.Nullable<global::Quire.Controls.Core.IdLabels> _currentDisplayIdLabel;

    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public double VerticalAlignmentOffset => View.VerticalAlignmentOffset;

    public bool IsEmpty => false;

    public bool IsActive
    {
        get;
        set;
    }

    public EquationControl ActiveCell => View.ActiveCell;

    public TableCoordinate ActiveCellCoordinate
    {
        get
        {
            EquationControl activeCell = ActiveCell;
            if (activeCell == null)
            {
                throw new InvalidOperationException("There is no active cell, likely the table does not have keyboard focus");
            }
            return new TableCoordinate(Grid.GetRow(activeCell), Grid.GetColumn(activeCell));
        }
    }

    public string ActiveCellSExpression
    {
        get
        {
            EquationControl activeCell = ActiveCell;
            if (activeCell == null)
            {
                throw new InvalidOperationException("There is no active cell, likely the table does not have keyboard focus");
            }
            return activeCell.EquationSExpression;
        }
    }

    public int NumberOfColumns => Model.ColumnCount;

    public bool FirstPropertyForXamlSerialization => true;

    public SynchronizationStatus SynchronizationStatus
    {
        get => (SynchronizationStatus)GetValue(SynchronizationStatusProperty);
        set => SetValue(SynchronizationStatusProperty, value);
    }

    public CalculatingStatus CalculatingStatus
    {
        get => (CalculatingStatus)GetValue(CalculatingStatusProperty);
        set => SetValue(CalculatingStatusProperty, value);
    }

    public int ErrorCount
    {
        get
        {
            int num = 0;
            foreach (ISpecTableColumn column in Model.Columns)
            {
                foreach (IEquationModel item in column)
                {
                    ICalculatableInfo calculatableInfo = Map.GetAssociatedView(item) as EquationControl;
                    num += calculatableInfo.ErrorCount;
                }
            }
            return num;
        }
    }

    private global::System.Collections.Generic.List<global::Quire.Controls.Core.IFindRestrictor> SearchableRegions
    {
        get
        {
            var searchableRegions = new global::System.Collections.Generic.List<global::Quire.Controls.Core.IFindRestrictor>();
            foreach (global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn column in Model.Columns)
            {
                searchableRegions.AddRange(column.Select(cell => WhiteboardItemSearchRestrictor.CreateFindRestrictor(Map.GetAssociatedView(cell) as global::Quire.Controls.EquationControl, this)));
            }
            return searchableRegions;
        }
    }

    public object PersistentData { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public global::Quire.Calculation.Core.EngineResult[] EngineResults
    {
        get
        {
            _engineResults = Model.GetWrappedColumns().Select(wrapper => wrapper.EngineResult).ToArray();
            return _engineResults;
        }
        set => _engineResults = value;
    }

    public bool IsCalculating
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public int DataRows
    {
        get
        {
            if (Model.ColumnCount <= 0)
            {
                return 0;
            }
            return Model.GetColumn(0).DataCellCount;
        }
    }

    public int Columns => Model.ColumnCount;

    public string RegionSExpression => Model.SExpression;

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public EngineError CurrentError
    {
        get
        {
            ICalculationErrorOwner calculationErrorOwner = (ICalculationErrorOwner)ActiveCell;
            if (calculationErrorOwner != null)
            {
                return calculationErrorOwner.CurrentError;
            }
            return null;
        }
    }

    public bool CanHideFromCalculationContext => true;

    public bool HideFromCalculationContext
    {
        get => CalcIsDisabled;
        set
        {
            if (CalcIsDisabled == value)
            {
                return;
            }
            CompoundCommand command = new CompoundCommand();
            DelegateFunction0<bool> doMethod = () =>
            {
                CalcIsDisabled = value;
                FirePropertyChanged("HideFromCalculationContext");
                using (new CommandScope(command))
                {
                    foreach (global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn column in Model.Columns)
                    {
                        ((ICalculationHideableChild)Model.GetColumnWrapper(column)).HideFromCalculationContext = value;
                    }
                }
                return true;
            };
            Delegate0 undoMethod = () =>
            {
                command.Undo();
                CalcIsDisabled = !value;
                FirePropertyChanged("HideFromCalculationContext");
            };
            CommandBuffer.Execute(new UndoableCommand("Change HideFromCalculationContext", doMethod, undoMethod));
        }
    }

    public IdLabels? CurrentIdLabel
    {
        get
        {
            return _currentIdLabel;
        }
        set
        {
            _currentIdLabel = value;
            if (_currentIdLabel == IdLabels.None)
            {
                _currentIdLabel = null;
            }

            FirePropertyChanged("CurrentIdLabel");
        }
    }

    public IdLabels? CurrentDisplayIdLabel
    {
        get
        {
            return _currentDisplayIdLabel;
        }
        set
        {
            _currentDisplayIdLabel = value;
            if (_currentDisplayIdLabel == IdLabels.None)
            {
                _currentDisplayIdLabel = null;
            }

            FirePropertyChanged("CurrentDisplayIdLabel");
        }
    }

    public MathFormat MathFormat
    {
        get;
        set;
    }

    public global::Quire.Controls.Core.StaticMathFormat MathFormatForSerialize { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public MathFormat CurrentMathFormat
    {
        get => MathFormat;
        set => throw new NotImplementedException();
    }

    public global::Quire.Controls.Core.IMathFormat DeserializeMathFormat { set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IdLabels, global::Quire.Controls.Core.ExtendedFont> DeserializeLabeledIdFormat { set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool InCustomInitialization
    {
        get
        {
            return Model.InCustomInitialization;
        }
        set
        {
            Model.InCustomInitialization = value;
            if (!value)
            {
                DeserializeLabeledIdFormat = null;
                DeserializeMathFormat = null;
            }
        }
    }

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatableContainer, global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableMoved;

    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem> OnGotFocusEvent;

    public event global::System.Action<global::Quire.Controls.Core.IWhiteboardItem, global::Quire.Direction> RequestRemove;

    public event global::System.Windows.RoutedEventHandler Changed
    {
        add => AddHandler(ChangedEvent, value);
        remove => RemoveHandler(ChangedEvent, value);
    }

    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    public event global::System.Action<global::System.Windows.DependencyObject, global::Quire.Controls.Core.IORegions.IORegionChangeType> IORegionChanged;

    private static void HandleCalcIsDisabledChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        SpecTableControl table = d as SpecTableControl;
        if (table != null)
        {
            table.FireContentAndRegionChangedEvents(IORegionChangeType.CalcIsDisabledStatus);
        }
    }

    public void Activate(bool isActive)
    {
        if (isActive)
        {
            AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLoad(this, DelegateFocus);
        }
    }

    private void OnTraversedFocusEvent(object sender, global::Quire.Wpf.CustomRoutedEventArgs<global::System.Windows.Input.FocusNavigationDirection> args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SimulateMouseClick(System.Windows.Point location, int numberOfClicks)
    {
        Activate(true);
    }

    protected override void OnPreviewGotKeyboardFocus(global::System.Windows.Input.KeyboardFocusChangedEventArgs e) => base.OnPreviewGotKeyboardFocus(e);

    private static void HandleIOTagPropertyChanged(global::System.Windows.DependencyObject depObj, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    static SpecTableControl()
    {
        _typeOfThis = typeof(SpecTableControl);
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        WhiteboardDependencyProperties.IsActiveProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(IsActiveRegionChanged));
        SynchronizationStatusProperty = EquationControl.SynchronizationStatusProperty.AddOwner(_typeOfThis);
        CalculatingStatusProperty = EquationControl.CalculatingStatusProperty.AddOwner(_typeOfThis);
        IOTag.IOTagProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleIOTagPropertyChanged));
        CalcIsDisabledProperty = AttachedProperties.CalcIsDisabledProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(HandleCalcIsDisabledChanged));
        ChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
    }

    private static void IsActiveRegionChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public SpecTableControl() { }

    public SpecTableControl(int rows, int columns) { }

    public SpecTableControl(string[,] data) { }

    public static Quire.Controls.SpecTableControl DefinitionListToSpecTable(System.Collections.Generic.List<Quire.EquationEditor.Tree.MathNode> definionsList)
    {
        ISpecTableModel model = SpecTableModel.DefinitionsListToSpecTableModel(definionsList);
        if (model == null)
        {
            return null;
        }

        return new SpecTableControl(model);
    }

    internal SpecTableControl(global::Quire.EquationEditor.SpecTable.ISpecTableModel model) { }

    global::Quire.Controls.Core.IUIElementArrangeInfoProvider global::Quire.Controls.Core.IHasChildArrangeInfoProvider.ChildArrangeInfoProvider { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private void BindCommands() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ExecuteClearMathFormat(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ClearFormat();
    }

    private void ExecuteIncreaseFontSize(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.IncreaseFontSize();
    }

    private void ExecuteDecreaseFontSize(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.DecreaseFontSize();
    }

    private void ExecuteChangeFontColor(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeFontColor((Color)e.Parameter);
    }

    private void ExecuteChangeHighlightColor(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeHighlightColor((Color?)e.Parameter);
    }

    private void CanNavigateDown(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Down, false);
    }

    private void CanNavigateUp(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Up, false);
    }

    private void CanNavigateRight(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Right, false);
    }

    private void CanNavigateLeft(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Model.CanNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Left, false);
    }

    private void OnNavigateRightByTerm(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Right, true);
    }

    private void OnNavigateLeftBuyTerm(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Left, true);
    }

    private void OnNavigateDown(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Down, false);
    }

    private void OnNavigateUp(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Up, false);
    }

    private void OnNavigateRight(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Right, false);
    }

    private void OnNavigateLeft(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        Model.HandleNavigationBetweenCells(Map.GetAssociatedModel(ActiveCell), Direction.Left, false);
    }

    private void HandleOnPreviewCommit(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void Commit()
    {
        Model.Commit();
    }

    private void HookUpAllViewEventsToContentChanged(global::Quire.Controls.SpecTableView view) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleCellAddedEvent(IEquationControl arg)
    {
        EquationControl cell = arg as EquationControl;
        if (cell == null)
        {
            return;
        }

        cell.GotKeyboardFocus += HandleCellGotKeyboardFocus;
        cell.LostKeyboardFocus += HandleCellLostKeyboardFocus;
        cell.RequestEvaluation += HandleRequestEvaluation;
    }

    private void HandleRequestEvaluation(Quire.Calculation.Core.ICalculatable calculatable)
    {
        Commit();
    }

    public void HandleCellLostKeyboardFocus(object sender, RoutedEventArgs e)
    {
        ICurrentIdLabelProperty cell = sender as ICurrentIdLabelProperty;
        if (cell != null)
        {
            cell.PropertyChanged -= CellPropertyChangedHandler;
        }
    }

    public void HandleCellGotKeyboardFocus(object sender, RoutedEventArgs e)
    {
        ICurrentIdLabelProperty cell = sender as ICurrentIdLabelProperty;
        if (cell != null)
        {
            cell.PropertyChanged += CellPropertyChangedHandler;
        }

        CellPropertyChangedHandler(sender, new PropertyChangedEventArgs(nameof(CurrentIdLabel)));
        CellPropertyChangedHandler(sender, new PropertyChangedEventArgs(nameof(CurrentDisplayIdLabel)));
    }

    private void CellPropertyChangedHandler(object sender, global::System.ComponentModel.PropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireChangedEvent()
    {
        RaiseEvent(new RoutedEventArgs(SpecTableControl.ChangedEvent, this));
    }

    private void HandleContentChangeEvent(object arg1, global::Quire.Direction arg2) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleContentChangeEvent(object arg1) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public ISpecTableColumn GetActiveColumn()
    {
        return GetColumnFromIndex(ActiveCellCoordinate.Column);
    }

    public Quire.EquationEditor.SpecTable.Model.ISpecTableColumn GetColumnFromIndex(int index)
    {
        return Model.GetColumn(index);
    }

    public void DelegateFocus()
    {
        UIElement cellToActivate = _currentCell ?? ActiveCell;
        _currentCell = null;
        bool shouldFocus = false;
        if (cellToActivate != null && cellToActivate.Focusable)
        {
            shouldFocus = true;
            if (!SpecTableView.GetIsSpecTableCellVisible(cellToActivate))
            {
                ((DependencyObject)cellToActivate).SetValue(SpecTableView.IsSpecTableCellVisibleProperty, true);
            }
        }
        else if (View.Children.Count > 0)
        {
            foreach (UIElement child in View.Children)
            {
                if (child.Focusable)
                {
                    cellToActivate = child;
                    shouldFocus = true;
                    break;
                }
            }
        }
        if (shouldFocus)
        {
            AttachedProperties.SuppressBringIntoViewIfNeededAndExecute(cellToActivate, () => cellToActivate.Focus(), !AttachedProperties.GetIsBringToView(this));
        }
    }

    private void HandleNewColumn(global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleNewColumn(global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn arg, bool addToHider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void FireContentAndRegionChangedEvents(global::Quire.Controls.Core.IORegions.IORegionChangeType type) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void FireContentChangedEvents()
    {
        FireContentAndRegionChangedEvents(IORegionChangeType.Content);
    }

    private void HandleRemovedColumn(global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleColumnSynchronizationStatusChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != "SynchronizationStatus")
        {
            return;
        }

        {
            SynchronizationStatus = Model.GetWrappedColumns().Any(column => column.SynchronizationStatus == SynchronizationStatus.Pending)
                ? SynchronizationStatus.Pending
                : SynchronizationStatus.Synchronized;
        }
    }

    private void HandleColumnCalculatingStatusChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != "CalculatingStatus")
        {
            return;
        }

        CalculatingStatus = Model.GetWrappedColumns().Any(column => column.CalculatingStatus == CalculatingStatus.Calculating)
            ? CalculatingStatus.Calculating
            : CalculatingStatus.Idle;
    }

    private void FireCalculatableAdded(global::Quire.Calculation.Core.ICalculatable wrapper) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireCalculatableRemoved(ICalculatable wrapper)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved.Invoke(wrapper);
        }
    }

    public void FireRequestRemove()
    {
        try
        {
            _currentCell = ActiveCell;
        }
        catch
        {
        }

        RequestRemove(this, Direction.Left);
    }

    public bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }

    private global::System.Collections.Generic.IEnumerator<global::Quire.Controls.Core.IFindRestrictor> GetISearchableEnumerator(global::Quire.Controls.EquationControl from, global::System.Windows.Documents.LogicalDirection direction)
    {
        var searchableRegions = SearchableRegions;
        if (direction == global::System.Windows.Documents.LogicalDirection.Backward)
        {
            searchableRegions.Reverse();
        }
        int count = from != null ? searchableRegions.FindIndex(item => item.SearchableRegion == from) : 0;
        return searchableRegions.Skip(count).GetEnumerator();
    }

    public bool FindOncePrivate(string text, bool startFromCaret, global::System.Windows.Documents.LogicalDirection direction, global::Quire.Controls.Core.FindOptions options, global::System.Collections.Generic.List<global::Quire.Pair<int, int>> subscriptPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override void EndInit() => base.EndInit();

    public global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(global::Quire.Serialization.IDeS11NProvider d11NProvider, object data) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public object GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool ContainsError(EngineError error)
    {
        foreach (ICalculatable calculatableChild in CalculatableChildren)
        {
            if (calculatableChild is ICalculationErrorOwner calculationErrorOwner && calculationErrorOwner.ContainsError(error))
            {
                return true;
            }
        }
        return false;
    }

    public void ShowError(global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleRequestShowError(object sender, global::Quire.Calculation.Core.RequestShowErrorEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ShowError(global::Quire.EquationEditor.SpecTable.Model.SpecTableCalculatableWrapper wrapper, global::Quire.Calculation.Core.EngineError error) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override bool ShouldSerializeContent()
    {
        return false;
    }

    private void OnToggleHideFromCalculationContext(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        HideFromCalculationContext = !HideFromCalculationContext;
        e.Handled = true;
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    protected override void OnLostKeyboardFocus(global::System.Windows.Input.KeyboardFocusChangedEventArgs e) => base.OnLostKeyboardFocus(e);

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        DelegateFocus();
    }

    public global::System.Collections.Generic.IList<string> GetDefinedIdentifierSExpressions() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool SubstituteAndCommitInputValue(string sExpr, string valueSExpr, string scalingSExpr)
    {
        return Model.SubstituteAndCommitInputValue(sExpr, valueSExpr, scalingSExpr, IsActive);
    }

    public bool IsSuitableForInputTaggingPrivate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ActivateAndScopeIdentifierPrivate(string idSExpr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn GetLastColumnWithMatchingId(string idSExpr)
    {
        global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn column = Model.Columns.LastOrDefault(candidate => candidate.Header.EquationSExpression == idSExpr);
        if (!DebugTools.DevAssert(column.IsNotEmpty() && column.Header.EquationRoot.DescriptorName == "IDENTIFIER", null).Failed)
        {
            return column;
        }
        return null;
    }

    public string GetDefinitionSExpression(string idSExpr) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Undo.UndoableCommand CreateToggleInputCommand(global::System.Action<global::Quire.Controls.Core.IORegions.IInputRegion, bool> toggleInputTag, bool setTag)
    {
        global::Quire.EquationEditor.SpecTable.Model.ISpecTableMemento undoMemento = Model.CreateMemento();
        global::Quire.EquationEditor.SpecTable.Model.ISpecTableMemento redoMemento = null;
        DelegateFunction0<bool> doMethod = () =>
        {
            bool isActive = IsActive;
            if (setTag)
            {
                using (new CommandScope(new CompoundCommand()))
                {
                    Model.RemoveEmptyColumns();
                }
                Commit();
            }
            toggleInputTag(this, setTag);
            if (isActive)
            {
                Model.HandleNavigateToFirstCell();
            }
            return true;
        };
        Delegate0 undoMethod = () =>
        {
            redoMemento = Model.CreateMemento();
            ToggleInput(!setTag, undoMemento, toggleInputTag);
        };
        Delegate0 redoMethod = () =>
        {
            if (!DebugTools.AssertNotNull(redoMemento, null).Failed)
            {
                ToggleInput(setTag, redoMemento, toggleInputTag);
            }
        };
        return new UndoableCommand("SpecTablePreSetInput", doMethod, undoMethod, redoMethod);
    }

    private void ToggleInput(bool isSetTag, global::Quire.EquationEditor.SpecTable.Model.ISpecTableMemento memento, global::System.Action<global::Quire.Controls.Core.IORegions.IInputRegion, bool> toggleInputTag) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Calculation.Core.ICalculatable GetCalculatableBySExpression(string sExpr)
    {
        foreach (global::Quire.EquationEditor.SpecTable.Model.ISpecTableColumn column in Model.Columns)
        {
            if (column.Header.GetDefinedIdentifierSExpressions().Any(expression => expression.Equals(sExpr)))
            {
                return Model.GetColumnWrapper(column);
            }
        }
        return null;
    }

    private void HandleCopySelection(int cellIndexA, int cellIndexB) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleGotFocus(object sender, global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void HandleMathFormatChanged(global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleOverrideLabelColor(global::System.Windows.DependencyPropertyChangedEventArgs e, global::System.Windows.DependencyObject obj) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string GetHelpContext()
    {
        if (ActiveCell == null)
        {
            return null;
        }
        return ActiveCell.GetHelpContext();
    }

    internal Quire.Controls.EquationControl GetCellAt(int row, int column)
    {
        return View.GetCellAt(row, column);
    }
}
