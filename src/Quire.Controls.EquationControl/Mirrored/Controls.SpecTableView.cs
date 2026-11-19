using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Controls.ControlAdorners;
using Quire.Controls.Core;
using Quire.Controls.Internal;
using Quire.EquationEditor.SpecTable;
using Quire.Matrices;
using Quire.Wpf;

namespace Quire.Controls;

internal partial class SpecTableView
{
    private int _rows;
    private Point? _mouseDownPoint;
    private Point? _savedMouseDownPoint;
    private OvertypeAdorner _overtypeAdorner;
    private Pair<int, int>? _selectedCells;
    private List<EquationControl> _activeCells;
    public static readonly RoutedEvent ChangedEvent;
    private CommandBindingCollection _editableCommandBindings;
    public static readonly DependencyProperty IsSpecTableCellVisibleProperty;

    public EquationControl ActiveCell
    {
        get
        {
            if (IsSelected)
            {
                return null;
            }

            EquationControl focusedAncestorOfTypeElement = WpfUtils.GetFocusedAncestorOfTypeElement<EquationControl>(this);
            if (WpfUtils.IsAncestorOf(this, focusedAncestorOfTypeElement))
            {
                return focusedAncestorOfTypeElement;
            }

            return null;
        }
    }

    public List<EquationControl> ActiveCells => _activeCells;

    public Pair<int, int>? SelectedCells => _selectedCells;

    public override int CellCount => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public double VerticalAlignmentOffset { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool IsSelected => IsFocused;

    public MathFormatDobjController MathFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); private set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public IMathFormat DeserializeMathFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public event Delegate2<IEquationControl, Direction> AddColumn;

    public event Delegate2<IEquationControl, Direction> AddRow;

    public event Delegate1<IEquationControl> DeleteColumn;

    public event Delegate1<IEquationControl> DeleteRow;

    public event ShouldHideCellEventHandler ShouldHideCell;

    public event Action<IEquationControl> OnTraverseForward;

    public event Action<IEquationControl> OnTraverseBackward;

    public event Action NavigateToSpecTableLastCell;

    public event Action NavigateToSpecTableFirstCell;

    public event Action<IEquationControl> RequestRemove;

    public event Action<IEquationControl, int, int, Direction> RequestRemoveSelection;

    public event Action<int, int> RequestCopySelection;

    public event Action<IEquationControl> RequestClearActiveCell;

    public event Delegate1<IEquationControl> CellAdded;

    public event Action<bool> InputTagChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public SpecTableView(bool isRestrictedReadOnly)
    {
        Columns = 1;
        ColumnSpacing = 4.0;
        RowSpacing = 0.5;
        ClearValue(FrameworkElement.MarginProperty);
        _activeCells = new List<EquationControl>();
        BindRoutedCommandsToEvents(isRestrictedReadOnly);
        FocusVisualStyle = null;
        Background = Brushes.Transparent;
        Loaded += SpecTableViewLoaded;
        AddHandler(UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus), handledEventsToo: true);
        AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(OnLostFocus), handledEventsToo: true);
        MathFormat = new MathFormatDobjController(this);
    }

    private void SpecTableViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!IsKeyboardFocusWithin)
        {
            UpdateEmptyScalingAndExtraDataCellsVisibility(hideOnly: true);
        }
    }

    private void BindRoutedCommandsToEvents(bool isRestrictedReadOnly)
    {
        CommandBindings.Add(new CommandBinding(EquationCommands.Traverse, HandleTraverseForward));
        CommandBindings.Add(new CommandBinding(EquationCommands.BackwardsTraverse, HandleBackwardsTraverse, BackwardsTraverseCanExecute));
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, OnCanCopy));
        CreateEditableCommandsList();
        if (!isRestrictedReadOnly)
        {
            BindEditingCommands();
        }

        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.NavigateToEquationEnd, HandleNavigateToEquationEnd, null, bindingMarksEventAsHandled: true);
        WpfUtils.AddPreemptiveCommandBinding(this, EquationCommands.NavigateToEquationStart, HandleNavigateToEquationStart, null, bindingMarksEventAsHandled: true);
    }

    private void CreateEditableCommandsList()
    {
        _editableCommandBindings = new CommandBindingCollection
        {
            new CommandBinding(EquationCommands.InsertColumn, HandleInsertColumnCommand),
            new CommandBinding(EquationCommands.InsertRow, HandleInsertRowCommand),
            new CommandBinding(EquationCommands.DeleteColumn, HandleDeleteColumnCommand),
            new CommandBinding(EquationCommands.DeleteRow, HandleDeleteRowCommand),
            new CommandBinding(EquationCommands.Delete, HandleOnDelete),
            new CommandBinding(EquationCommands.Backspace, HandleOnBackspace),
            new CommandBinding(EquationCommands.ClearCells, OnClearCells, OnCanPerformClearCellsOperation),
            new CommandBinding(ApplicationCommands.Cut, OnCut, OnCanCut)
        };
    }

    private void BindEditingCommands()
    {
        CommandBindings.AddRange(_editableCommandBindings);
    }

    private void UnBindEditingCommands()
    {
        foreach (CommandBinding editableCommandBinding in _editableCommandBindings)
        {
            CommandBindings.Remove(editableCommandBinding);
        }
    }

    public void HandleRestrictedReadonlyChanged(bool isRestrictedReadonly)
    {
        if (isRestrictedReadonly)
        {
            UnBindEditingCommands();
        }
        else
        {
            BindEditingCommands();
        }
    }

    public void FireInputTagChanged(bool isInput)
    {
        if (InputTagChanged != null)
        {
            InputTagChanged(isInput);
        }
    }

    private void OnCanCut(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanCopy();
    }

    private void OnCut(object sender, ExecutedRoutedEventArgs e)
    {
        Copy();
        HandleDeleteSelection(Direction.Left);
    }

    private void OnCanCopy(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanCopy();
    }

    private void OnCopy(object sender, ExecutedRoutedEventArgs e)
    {
        Copy();
    }

    private bool CanCopy()
    {
        return SelectedCells.HasValue;
    }

    private void Copy()
    {
        if (RequestCopySelection != null && SelectedCells.HasValue)
        {
            RequestCopySelection(SelectedCells.Value.First, SelectedCells.Value.Second);
        }
    }

    private void OnCanPerformClearCellsOperation(object sender, CanExecuteRoutedEventArgs e)
    {
        bool canExecute = false;
        if (SelectedCells.HasValue && SelectedCellsAreEmpty())
        {
            canExecute = true;
        }
        else if (ActiveCell != null && !ActiveCell.IsEmpty)
        {
            canExecute = true;
        }

        e.CanExecute = canExecute;
    }

    private bool SelectedCellsAreEmpty()
    {
        bool result = false;
        if (ActiveCells != null && ActiveCells.Count > 0)
        {
            foreach (EquationControl activeCell in ActiveCells)
            {
                if (!activeCell.IsEmpty)
                {
                    result = true;
                    break;
                }
            }
        }

        return result;
    }

    public void FillActiveCells()
    {
        foreach (int item2 in MatrixHelper.SubMatrixIndices(SelectedCells.Value.First, SelectedCells.Value.Second, Columns))
        {
            if (Children[ViewIndexFromModelIndex(item2)] is EquationControl item)
            {
                _activeCells.Add(item);
            }
            else
            {
                DebugTools.DevAssert(false, "Selected equation control cannot be null !");
            }
        }
    }

    public void ClearActiveCells()
    {
        _activeCells.Clear();
    }

    private void OnClearCells(object sender, ExecutedRoutedEventArgs e)
    {
        if (SelectedCells.HasValue)
        {
            HandleDeleteSelection(Direction.Indeterminate);
        }
        else
        {
            HandleClearActiveCell();
        }

        e.Handled = true;
    }

    private void HandleClearActiveCell()
    {
        RequestClearActiveCell?.Invoke(ActiveCell);
    }

    private void HandleOnDelete(object sender, ExecutedRoutedEventArgs e)
    {
        HandleDeleteSelection(Direction.Right);
        e.Handled = true;
    }

    private void HandleOnBackspace(object sender, ExecutedRoutedEventArgs e)
    {
        HandleDeleteSelection(Direction.Left);
        e.Handled = true;
    }

    private void HandleDeleteSelection(Direction direction)
    {
        if (SelectedCells.HasValue)
        {
            int index = ViewIndexFromModelIndex(SelectedCells.Value.First);
            if (Children[index] is IEquationControl equationControl)
            {
                FireRequestRemoveSelection(equationControl, direction);
            }
            else
            {
                DebugTools.FailAssertion(((object)Children[index])?.GetType().Name);
            }
        }
    }

    private int ViewIndexFromModelIndex(int modelIndex)
    {
        RowColumnHelper rowColumnHelper = new RowColumnHelper(Columns);
        int num = rowColumnHelper.ColIndexFromCellIndex(modelIndex);
        int num2 = rowColumnHelper.RowIndexFromCellIndex(modelIndex);
        return num * Rows + num2;
    }

    private void HandleNavigateToEquationStart(object sender, ExecutedRoutedEventArgs e)
    {
        if (NavigateToSpecTableFirstCell != null)
        {
            NavigateToSpecTableFirstCell();
        }

        e.Handled = true;
    }

    private void HandleNavigateToEquationEnd(object sender, ExecutedRoutedEventArgs e)
    {
        if (NavigateToSpecTableLastCell != null)
        {
            NavigateToSpecTableLastCell();
        }

        e.Handled = true;
    }

    private void SpecTableViewLostFocus(object sender, RoutedEventArgs e)
    {
        ClearSelection();
    }

    public void Clear()
    {
        Children.Clear();
        Columns = 0;
        _rows = 0;
    }

    public void AddChild(IEquationControl cellView, int rowIndex, int columnIndex)
    {
        if (!(cellView is UIElement element))
        {
            DebugTools.FailAssertion(cellView?.GetType().Name);
            return;
        }

        _rows = Math.Max(rowIndex + 1, _rows);
        Columns = Math.Max(Columns, columnIndex + 1);
        Grid.SetRow(element, rowIndex);
        Grid.SetColumn(element, columnIndex);
        Children.Add(element);
    }

    public override IEnumerable<UIElement> GetChildrenInRowMajorOrder() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void OnRender(DrawingContext dc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleInsertColumnCommand(object sender, ExecutedRoutedEventArgs e)
    {
        HandleCellCommandWithDirectionParameter(e, FireAddColumn);
    }

    private void HandleInsertRowCommand(object sender, ExecutedRoutedEventArgs e)
    {
        HandleCellCommandWithDirectionParameter(e, FireAddRow);
    }

    private void HandleDeleteColumnCommand(object sender, ExecutedRoutedEventArgs e)
    {
        HandleDeleteCommand(e, FireDeleteColumn);
    }

    private void HandleDeleteRowCommand(object sender, ExecutedRoutedEventArgs e)
    {
        HandleDeleteCommand(e, FireDeleteRow);
    }

    private void HandleDeleteCommand(ExecutedRoutedEventArgs e, Delegate1<IEquationControl> eventFirer) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void HandleCellCommandWithDirectionParameter(ExecutedRoutedEventArgs e, Delegate2<IEquationControl, Direction> eventFirer) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleTraverseForward(object sender, ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleBackwardsTraverse(object sender, ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void BackwardsTraverseCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        EquationControl equationControl = FindCellFromDecendent(e.Source as FrameworkElement);
        e.CanExecute = Children[0] != equationControl;
    }

    internal static EquationControl FindCellFromDecendent(FrameworkElement element)
    {
        while (element != null && !(element is EquationControl))
        {
            element = element.Parent as FrameworkElement;
        }

        return element as EquationControl;
    }

    private void FireAddColumn(IEquationControl ctrl, Direction dir)
    {
        if (ctrl != null)
        {
            AddColumn?.Invoke(ctrl, dir);
        }

        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    private void FireAddRow(IEquationControl ctrl, Direction dir)
    {
        if (ctrl != null)
        {
            AddRow?.Invoke(ctrl, dir);
        }

        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    private void FireDeleteRow(IEquationControl ctrl)
    {
        if (ctrl != null)
        {
            DeleteRow?.Invoke(ctrl);
        }

        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    private void FireDeleteColumn(IEquationControl ctrl)
    {
        if (ctrl != null)
        {
            DeleteColumn?.Invoke(ctrl);
        }

        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    private void FireRequestRemoveSelection(IEquationControl equationControl, Direction direction)
    {
        if (RequestRemoveSelection != null && SelectedCells.HasValue)
        {
            RequestRemoveSelection(equationControl, SelectedCells.Value.First, SelectedCells.Value.Second, direction);
        }
    }

    private void OnGotFocus(object sender, RoutedEventArgs e)
    {
        MakeAllCellsVisible();
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        UpdateEmptyScalingAndExtraDataCellsVisibility(hideOnly: true);
    }

    public static bool GetIsSpecTableCellVisible(UIElement equation)
    {
        if (equation != null)
        {
            return (bool)equation.GetValue(IsSpecTableCellVisibleProperty);
        }

        return false;
    }

    private void UpdateEmptyScalingAndExtraDataCellsVisibility(bool hideOnly)
    {
        if (ShouldHideCell == null)
        {
            return;
        }

        foreach (UIElement child in Children)
        {
            if (child is IEquationControl cell)
            {
                ShouldHideCellEventArgs e2 = new ShouldHideCellEventArgs(cell);
                ShouldHideCell(this, e2);
                if (e2.ShouldHideCell)
                {
                    HideCellContent(child);
                }
                else if (!hideOnly)
                {
                    ShowCellContent(child);
                }
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }
    }

    private void MakeAllCellsVisible()
    {
        foreach (UIElement child in Children)
        {
            ShowCellContent(child);
        }
    }

    public void UpdateCellsVisibility()
    {
        UpdateEmptyScalingAndExtraDataCellsVisibility(hideOnly: false);
    }

    internal static void HideCellContent(UIElement cell)
    {
        cell.SetValue(IsSpecTableCellVisibleProperty, false);
    }

    internal static void ShowCellContent(UIElement cell)
    {
        cell.SetValue(IsSpecTableCellVisibleProperty, true);
    }

    private void UpdateAdornerLocationToSelectedCells() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CreateOvertypeAdorner()
    {
        ClearOvertypeAdorner();
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
        if (adornerLayer != null)
        {
            _overtypeAdorner = new OvertypeAdorner(this);
            adornerLayer.Add(_overtypeAdorner);
        }
        else
        {
            DebugTools.DevAssert(false, null);
        }

        Focusable = true;
        Focus();
        LostFocus += SpecTableViewLostFocus;
    }

    public void ClearOvertypeAdorner() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ClearSelection()
    {
        ClearOvertypeAdorner();
        ClearActiveCells();
    }

    public void FireCellAdded(IEquationControl eq)
    {
        CellAdded?.Invoke(eq);
    }

    private void CreateSelection(int cellIndexA, int cellIndexB)
    {
        if (cellIndexA == cellIndexB)
        {
            _overtypeAdorner?.UpdateAdornerLocation(default, default);
            return;
        }

        _selectedCells = new Pair<int, int>(cellIndexA, cellIndexB);
        FillActiveCells();
        UpdateAdornerLocationToSelectedCells();
    }

    protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
    {
        _mouseDownPoint = null;
        base.OnPreviewMouseUp(e);
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        ClearSelection();
        _savedMouseDownPoint = null;
        _mouseDownPoint = e.GetPosition(this);
        base.OnPreviewMouseDown(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        _savedMouseDownPoint = _mouseDownPoint;
        _mouseDownPoint = null;
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        if (SelectedCells.HasValue)
        {
            _mouseDownPoint = _savedMouseDownPoint;
        }
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        if (_mouseDownPoint.HasValue)
        {
            GetStartEndCellIndices(e.GetPosition(this), out var mouseDownCellIndex, out var currentCellIndex);
            CreateSelection(mouseDownCellIndex, currentCellIndex);
        }

        base.OnPreviewMouseMove(e);
    }

    public void SelectCells(int cellIndexA, int cellIndexB)
    {
        if (!IsMeasureValid)
        {
            Dispatcher.BeginInvoke((DispatcherPriority)6, (Delegate)new Action<int, int>(SelectCells), cellIndexA, new object[1] { cellIndexB });
        }
        else
        {
            CreateSelection(cellIndexA, cellIndexB);
        }
    }

    private void GetStartEndCellIndices(Point currentCellIndexPoint, out int mouseDownCellIndex, out int currentCellIndex)
    {
        mouseDownCellIndex = _layoutManager.CellIndexFromPoint(_mouseDownPoint.Value);
        currentCellIndex = _layoutManager.CellIndexFromPoint(currentCellIndexPoint);
    }

    public void SetDeserializedMathFormatData(IEquationControl equation) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal EquationControl GetCellAt(int row, int column)
    {
        return GetChildrenInRowMajorOrder()?.ElementAt(MatrixHelper.IndexOf(row, column, Columns)) as EquationControl;
    }
}
