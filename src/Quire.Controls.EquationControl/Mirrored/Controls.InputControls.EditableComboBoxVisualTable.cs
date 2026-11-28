using Quire.Controls.Core;
using Quire.Controls.InputControls;
using Quire.EquationEditor.View;
using Quire.Matrices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Quire.EquationEditor.Tree;
using System;
using System.ComponentModel;

namespace Quire.Controls.InputControls;

[global::Quire.Mirror.MirrorOf("Ptc.Controls.InputControls.EditableComboBoxVisualTable, Ptc.Controls.EquationControl")]
public partial class EditableComboBoxVisualTable : global::Quire.Controls.Core.VisualTableContents, global::Quire.EquationEditor.View.IEditableComboBoxVisualTable, global::Quire.Wpf.ICanBlockUndoRedo, global::System.Windows.Markup.IComponentConnector
{
    public const double MinMouseMovementDistanceToCreateCellsSelection = 4d;

    private global::System.Nullable<global::System.Windows.Point> _mouseDownPoint;

    private global::System.Nullable<global::System.Windows.Point> _lastPositionCellSelectionWasCreated;

    private global::Quire.Controls.ControlAdorners.OvertypeAdorner _overtypeAdorner;

    private readonly global::Quire.Controls.InputControls.EditableComboBoxViewModel _viewModel;

    private int _rows;

    private bool _contentLoaded;

    public override int Rows { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public override int CellCount { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    bool global::Quire.Wpf.ICanBlockUndoRedo.BlockUndoRedo { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public IEditableComboBoxViewModel ViewModel => _viewModel;

    public bool IsActive => IsVisible;

    public ExtendedFont OperatorFormat
    {
        get;
        private set;
    }

    private EditableComboBoxTableCellsFontFormats CellsFontFormats
    {
        get;
        set;
    }

    public System.Tuple<int, int> SelectedCells => _viewModel.SelectedCells;

    public event global::System.Action ContentChanged;

    public event global::System.Action<bool> RequestCloseEditingMode;

    internal EditableComboBoxVisualTable(global::Quire.Controls.InputControls.EditableComboBoxViewModel viewModel, global::Quire.Controls.InputControls.EditableComboBoxTableCellsFontFormats cellsFontFormats, global::Quire.Controls.Core.ExtendedFont operatorFormat) : base(default(global::Quire.Controls.Core.TableLayoutManager)) { throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SelectedCells" && IsMeasureValid && IsArrangeValid)
        {
            UpdateAdornerLocationToSelectedCells();
        }
    }

    public void Refresh(global::Quire.Controls.InputControls.EditableComboBoxTableCellsFontFormats newCellsFontFormats, global::Quire.Controls.Core.ExtendedFont operatorFormat) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override global::System.Windows.Size MeasureOverride(global::System.Windows.Size availableSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override global::System.Windows.Size ArrangeOverride(global::System.Windows.Size finalSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool IsCellCountValid()
    {
        return Rows == _viewModel.Rows && Columns == _viewModel.Columns && Children.Count == Rows * Columns;
    }

    private void UpdateRowsColumnsIndex()
    {
        _rows = _viewModel.Rows;
        Columns = _viewModel.Columns;
    }

    private void FireContentChanged()
    {
        ContentChanged?.Invoke();
    }

    private void BindCommands() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void AddGivenCommandKeyGesturesToInstanceInputBinding(global::System.Windows.Input.RoutedCommand command) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanInsertRow(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleInsertRow(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanInsertColumn(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleInsertColumn(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanDeleteRows(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleDeleteRows(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanDeleteColumns(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleDeleteColumns(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanDeleteSelection(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _viewModel.AreThereSelectedCells;
    }

    private void HandleOnDeleteSelection(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanClearSelectedCells(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleOnClearSelectedCells(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleNewCellsAdded(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.InputControls.ICellData> newCells) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void MoveFocusedElementAndUpdateSelectedRowAfterRowsDeleted(int fromRowIndex, int toRowIndex, bool isFinishedEditingMode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleRowsDeleted(int fromRowIndex, int toRowIndex, bool isFinishedEditingMode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void HandleColumnsDeleted(int fromColumnIndex, int toColumnIndex, bool isFinishedEditingMode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DeleteChildFrameworkElementAtRowColumn(int row, int column, int numOfColumns) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Controls.Core.IMathFormat GetCellFormatAt(int row, int column) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void SetTextBoxCellFontFormatProperties(global::Quire.EquationEditor.View.IReadonlyCellData cell, global::System.Windows.Controls.TextBox textBox) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private ComboBoxCellDataType GetCellDataType(ICellData cell)
    {
        int row = cell.Row;
        int column = cell.Column;
        if (EditableComboBoxViewModel.IsTitleCell(row, column))
        {
            return ComboBoxCellDataType.Titles;
        }

        if (EditableComboBoxViewModel.IsUnitCell(row, column))
        {
            return ComboBoxCellDataType.Units;
        }

        return ComboBoxCellDataType.Values;
    }

    private global::System.Windows.Controls.TextBox BuildTextBoxControl(global::Quire.Controls.InputControls.ICellData cell) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FinishEditingAndFireRequestToCloseEditMode(bool shouldKeepFocus) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void EditModeLostFocus(object sender, RoutedEventArgs e)
    {
        DebugTools.DevAssert(!e.Handled, null);
        if (IsKeyboardFocusWithin)
        {
            if (this == e.Source)
            {
                _viewModel.ClearSelection();
                return;
            }
        }
        else
        {
            FinishEditingAndFireRequestToCloseEditMode(false);
        }
    }

    private void AddOrRemoveLostFocusHandler(bool add)
    {
        if (add)
        {
            AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(EditModeLostFocus), true);
        }
        else
        {
            RemoveHandler(UIElement.LostFocusEvent, new RoutedEventHandler(EditModeLostFocus));
        }
    }

    private void OnCloseEditMode(object sender, global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            OnCloseEditMode(null, e);
        }

        if (IsArrowKeyWhileCellsAreSelected(e.Key))
        {
            OnArrowKeyPressWhileThereAreSelectedCellsActivateFirstCellSelected(e);
        }
    }

    private bool IsArrowKeyWhileCellsAreSelected(global::System.Windows.Input.Key key) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnArrowKeyPressWhileThereAreSelectedCellsActivateFirstCellSelected(global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CanTabNavigate(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = IsKeyboardFocusWithin && !IsKeyboardFocused;
    }

    private void ForwardTabNavigate(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MoveFocusedElement(FocusNavigationDirection.Next, GetCellDataFromTextBox(e.Source, out _));
        e.Handled = true;
    }

    private void BackwardTabNavigate(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        MoveFocusedElement(FocusNavigationDirection.Previous, GetCellDataFromTextBox(e.Source, out _));
        e.Handled = true;
    }

    private void OnPreviewMoveUpByLine(object sender, ExecutedRoutedEventArgs e)
    {
        if (IsArrowKeyWhileCellsAreSelected(Key.Up))
        {
            OnArrowKeyPressWhileThereAreSelectedCellsActivateFirstCellSelected(e);
            return;
        }

        MoveFocusedElement(FocusNavigationDirection.Up, GetCellDataFromTextBox(e.Source, out _));
        e.Handled = true;
    }

    private void OnPreviewMoveDownByLine(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnPreviewMoveLeftByCharacter(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnPreviewMoveRightByCharacter(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void MoveFocusFromElementAfterRowColumnWereInserted(global::System.Windows.Input.FocusNavigationDirection direction, global::Quire.EquationEditor.View.IReadonlyCellData cellData) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void MoveFocusedElement(global::System.Windows.Input.FocusNavigationDirection direction, global::Quire.EquationEditor.View.IReadonlyCellData cellData) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void UpdateAdornerLocationToSelectedCells() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.ControlAdorners.OvertypeAdorner EnsureOvertypeAdornerAndFocus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void OnMouseDown(global::System.Windows.Input.MouseButtonEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void OnPreviewMouseDown(global::System.Windows.Input.MouseButtonEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseUp(e);
        if (e.ChangedButton == MouseButton.Left)
        {
            _mouseDownPoint = null;
            _lastPositionCellSelectionWasCreated = null;
        }
    }

    protected override void OnPreviewMouseMove(global::System.Windows.Input.MouseEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void GetStartEndCellIndices(Point currentCellIndexPoint, out int mouseDownCellIndex, out int currentCellIndex)
    {
        mouseDownCellIndex = _layoutManager.CellIndexFromPoint(_mouseDownPoint.GetValueOrDefault(currentCellIndexPoint));
        currentCellIndex = _layoutManager.CellIndexFromPoint(currentCellIndexPoint);
    }

    private void OnIsVisibleChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void OnDpiChanged(System.Windows.DpiScale oldDpi, System.Windows.DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        _viewModel.UpdatePlaceHolderBoxSize(newDpi);
    }

    public static global::Quire.EquationEditor.View.IReadonlyCellData GetCellDataFromTextBox(object element, out global::System.Windows.Controls.TextBox textBox) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static bool IsTextBoxDataContextIsCellData(object element, out Quire.EquationEditor.View.IReadonlyCellData cellData, out System.Windows.Controls.TextBox textBox)
    {
        cellData = GetCellDataFromTextBox(element, out textBox);
        return cellData != null;
    }

    private int GetRowFromTextBoxIndex(int idx)
    {
        return MatrixHelper.RowOf(idx, Columns);
    }

    private int GetColumnFromTextBoxIndex(int idx)
    {
        return MatrixHelper.ColumnOf(idx, Columns);
    }

    private Quire.EquationEditor.View.IReadonlyCellData GetCellDataAtRowColumnFromVisualTableChildren(int row, int column)
    {
        return GetCellDataFromTextBox(GetChildElemAtRowColumn(row, column), out _);
    }

    private System.Windows.UIElement GetChildAt(int index)
    {
        if (!DebugTools.DevAssert(0 <= index && index < Children.Count, null).Passed)
        {
            return null;
        }

        return Children[index];
    }

    private System.Windows.UIElement GetChildElemAtRowColumn(int row, int column)
    {
        return GetChildAt(MatrixHelper.IndexOf(row, column, Columns));
    }

    private bool IsVisualDataEqualsViewModelTableData() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void InitializeComponent() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
