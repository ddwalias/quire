using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.ExcelComponent;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Text;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetViewModel
{
    public ObservableRangeCollection<IWorksheetPage> VisiblePages => _pageManager.VisiblePages;

    public ObservableRangeCollection<IWorksheetPage> Pages => _pageManager.Pages;

    public int TrailingEmptyPageCount => _pageManager.GetTrailingEmptyPageCount();

    public bool IsAutoCalcChecked => IsAutoCalcOn;

    public bool IsCalculating => _model.IsCalculating;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UNITTEST_Evaluate()
    {
        Evaluate();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UNITTEST_DirectionalMoveHandler(Direction direction)
    {
        DirectionalMoveHandler(direction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool UNITTEST_CanMoveInDirection(Direction direction)
    {
        return CanMoveInDirection(direction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool UNITTEST_CanPaste()
    {
        return CanPaste();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UNITTEST_Paste()
    {
        Paste();
    }

    public void HandleSelectAll()
    {
        ActiveSection.SelectAllItems();
    }

    public void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem)
    {
        ActiveSection.ToggleItemSelection(item, includeActiveItem);
    }

    public void SelectContiguous(IWhiteboardItem item)
    {
        ActiveSection.SelectContiguous(item);
    }

    public void SelectItem(IWhiteboardItem item)
    {
        ActiveSection.SelectItem(item);
    }

    public void SelectItems(IEnumerable<IWhiteboardItem> items)
    {
        ActiveSection.SelectItems(items);
    }

    public void ClearSelection()
    {
        ActiveSection.ClearSelection();
    }

    public void PageDown()
    {
        MoveByPage(Direction.Down);
    }

    public void PageUp()
    {
        MoveByPage(Direction.Up);
    }

    public bool HasScriptableControls { get; set; }

    private EngineError CurrentErrorChain { get; set; }

    public IHeaderFooterControl Header { get; private set; }

    public IHeaderFooterControl Footer { get; private set; }

    public IHeaderFooterControl FirstPageHeader { get; private set; }

    public IHeaderFooterControl FirstPageFooter { get; private set; }

    public bool IsSavedWorksheet { private get; set; }

    public IWorksheetContent WorksheetContent { get; private set; }

    public NumberFormat DisplayNumberFormat { get; set; }

    public MatrixFormat ResultMatrixFormat { get; set; }

    public DragMode CurrentDragMode { get; set; }

    private void CollapseIfPresent(IHeaderFooterZombie zombie)
    {
        zombie?.Collapse();
    }

    private void ShowIfPresent(IHeaderFooterZombie zombie)
    {
        zombie?.Show();
    }

    private void ResetVisualBrushIfPresent(IHeaderFooterZombie zombie)
    {
        zombie?.ResetVisualBrush();
    }

    private void OnStyleCollectionChanged()
    {
        FireChanged();
    }

    public bool IsValidInsertionPoint(Point point)
    {
        return WorksheetContent.IsValidInsertionPoint(point);
    }

    private bool CanCreateOleObjectControl()
    {
        return CanCreateControlOfType<IOleObjectControl>();
    }

    private bool CanCreateChartComponent()
    {
        return CanCreateControlOfType<IChartComponentControl>();
    }

    public IWorksheetPageBreak CreatePageBreakControl()
    {
        return ControlCreator.CreatePageBreakControl();
    }

    public void HandleToggleAutoCalculation(bool newState)
    {
        IsAutoCalcOn = newState;
        FireChanged();
    }

    public bool CanReCalculateAll()
    {
        return IsCalculationsEnabled;
    }

    public bool CanRemoveSpace(Direction direction)
    {
        return CanDeleteVerticalSpace(direction);
    }

    private bool CanCopySelection()
    {
        return SelectedItems.Count() > 0;
    }

    private bool CanCutSelection()
    {
        return SelectedItems.Count() > 0;
    }

    private Rect GetItemRect(IWhiteboardItem item)
    {
        return ItemManager.GetItemRect(item);
    }

    private bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items)
    {
        return ItemManager.ItemsOverlapped(items);
    }

    private static bool IsPageWideSizablePredicate(IWhiteboardItem item)
    {
        return item is IPageWideSizable;
    }

    public void ImportData(string fileName)
    {
        InsertFileName(fileName);
    }

    public IDictionary<IdLabels, ExtendedFont> GetDefaultLabeledIdFormat()
    {
        return VisualHelper.GetDefaultLabeledIdFormat();
    }

    private IEnumerable<IIORegion> GetInputRegions()
    {
        return GetIORegions<IInputRegion>();
    }

    private IEnumerable<IIORegion> GetOutputRegions()
    {
        return GetIORegions<IOutputRegion>();
    }

    public void HandleSetCurrentProofingLanguage(CultureInfo culture)
    {
        CurrentProofingLanguage = XmlLanguageHelper.ConvertCultureInfoToXmlLanguage(culture);
    }

    public void OnRequestRestoreInsertionPoint(IWorksheetControl worksheetControl, RoutedEventArgs eventArgs)
    {
        Activate(worksheetControl);
        eventArgs.Handled = true;
    }
}
