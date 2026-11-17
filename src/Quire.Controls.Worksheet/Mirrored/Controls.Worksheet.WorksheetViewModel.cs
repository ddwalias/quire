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
using Quire.Controls.Worksheet.Printing;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Wpf;
using Quire.Wpf.Printing;

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

    public bool IsContentActive
    {
        get
        {
            return _isContentActive;
        }
        set
        {
            if (_isContentActive == value)
            {
                return;
            }
            _isContentActive = value;
            OnPropertyChanged("IsContentActive");
        }
    }

    public bool IsHeaderActive
    {
        get
        {
            if (ActiveSection != Header)
            {
                return ActiveSection == FirstPageHeader;
            }
            return true;
        }
    }

    public bool IsFooterActive
    {
        get
        {
            if (ActiveSection != Footer)
            {
                return ActiveSection == FirstPageFooter;
            }
            return true;
        }
    }

    public Point InsertionPoint
    {
        get
        {
            return ActiveSection.InsertionPoint;
        }
        set
        {
            ActiveSection.InsertionPoint = value;
        }
    }

    public IWhiteboardItem ActiveItem
    {
        get
        {
            if (ActiveSection == null)
            {
                return null;
            }
            return ActiveSection.ActiveItem;
        }
    }

    public IWhiteboardItem ActiveDescendant
    {
        get
        {
            if (ActiveSection == null)
            {
                return null;
            }
            return ActiveSection.ActiveDescendant;
        }
    }

    public IList<IWhiteboardItem> SelectedDescendants
    {
        get
        {
            if (ActiveSection == null)
            {
                return null;
            }
            return ActiveSection.SelectedDescendants;
        }
    }

    public int PageCount
    {
        get
        {
            return _pageCount;
        }
        private set
        {
            _pageCount = value;
            OnPropertyChanged("PageCount");
        }
    }

    public int StartingPageNumber
    {
        get
        {
            return _startingPageNumber;
        }
        set
        {
            _startingPageNumber = value;
            OnPropertyChanged("StartingPageNumber");
            FireStartingPageNumberChanged();
            FireChanged();
        }
    }

    public int TotalPagesOffset
    {
        get
        {
            return _offset;
        }
        set
        {
            _offset = value;
            OnPropertyChanged("TotalPagesOffset");
        }
    }

    public double WorksheetHeight
    {
        get
        {
            return _worksheetHeight;
        }
        private set
        {
            if (_worksheetHeight == value)
            {
                return;
            }
            _worksheetHeight = value;
            OnPropertyChanged("WorksheetHeight");
        }
    }

    public double HeaderTop
    {
        get
        {
            return _headerTop;
        }
        private set
        {
            if (_headerTop == value)
            {
                return;
            }
            _headerTop = value;
            OnPropertyChanged("HeaderTop");
        }
    }

    public double FooterTop
    {
        get
        {
            return _footerTop;
        }
        private set
        {
            if (_footerTop == value)
            {
                return;
            }
            _footerTop = value;
            OnPropertyChanged("FooterTop");
        }
    }

    public double WorksheetPageLayoutDisplayHeight
    {
        get
        {
            if (WorksheetPageLayoutMode != PageLayoutMode.PageMode)
            {
                return PageLayout.PageBodyHeight;
            }
            return PageLayout.DisplayHeight;
        }
    }

    public double NonprintableSpaceButtonHeight
    {
        get
        {
            return _nonprintableSpaceButtonHeight;
        }
        private set
        {
            if (_nonprintableSpaceButtonHeight == value)
            {
                return;
            }
            _nonprintableSpaceButtonHeight = value;
            OnPropertyChanged("NonprintableSpaceButtonHeight");
        }
    }

    public TextFormat HeaderTextFormat
    {
        get
        {
            return Header.TextFormat;
        }
        set
        {
            Header.TextFormat = value;
        }
    }

    public TextFormat FooterTextFormat
    {
        get
        {
            return Footer.TextFormat;
        }
        set
        {
            Footer.TextFormat = value;
        }
    }

    public TextFormat FirstPageHeaderTextFormat
    {
        get
        {
            return FirstPageHeader.TextFormat;
        }
        set
        {
            FirstPageHeader.TextFormat = value;
        }
    }

    public TextFormat FirstPageFooterTextFormat
    {
        get
        {
            return FirstPageFooter.TextFormat;
        }
        set
        {
            FirstPageFooter.TextFormat = value;
        }
    }

    public double WidthWithNonprintableSpace
    {
        get
        {
            return _widthWithNonprintableSpace;
        }
        private set
        {
            if (_widthWithNonprintableSpace == value)
            {
                return;
            }
            _widthWithNonprintableSpace = value;
            OnPropertyChanged("WidthWithNonprintableSpace");
        }
    }

    public bool HasSelection
    {
        get
        {
            return _hasSelection;
        }
        set
        {
            if (_hasSelection == value)
            {
                return;
            }
            _hasSelection = value;
            OnPropertyChanged("HasSelection");
        }
    }

    public bool IsInLimbo
    {
        get
        {
            return _isInLimbo;
        }
        set
        {
            if (_isInLimbo == value)
            {
                return;
            }
            _isInLimbo = value;
            OnPropertyChanged("IsInLimbo");
        }
    }

    public bool HideFromCalculationButtonEnabled
    {
        get
        {
            return _hideFromCalculationButtonEnabled;
        }
        set
        {
            _hideFromCalculationButtonEnabled = value;
            OnPropertyChanged("HideFromCalculationButtonEnabled");
        }
    }

    public bool HideFromCalculationButtonChecked
    {
        get
        {
            return _hideFromCalculationButtonChecked;
        }
        set
        {
            _hideFromCalculationButtonChecked = value;
            OnPropertyChanged("HideFromCalculationButtonChecked");
        }
    }

    public WorksheetDragger WorksheetDragHandler
    {
        get
        {
            return _worksheetDragHandler;
        }
        set
        {
            _worksheetDragHandler = value;
            OnPropertyChanged("WorksheetDragHandler");
        }
    }

    public bool PreRecalculateAll
    {
        get
        {
            return _preRecalculateAll;
        }
        set
        {
            _preRecalculateAll = value;
            OnPropertyChanged("PreRecalculateAll");
        }
    }

    public StaticMathFormat ContentMathFormat
    {
        get
        {
            return WorksheetContent.MathFormat;
        }
        set
        {
            WorksheetContent.MathFormat = value;
        }
    }

    public bool EnableEditCurrentPageNumber
    {
        get
        {
            return _enableEditCurrentPageNumber;
        }
        set
        {
            _enableEditCurrentPageNumber = value;
            OnPropertyChanged("EnableEditCurrentPageNumber");
        }
    }

    public bool HeaderFooterZombiesReady
    {
        get
        {
            return _headerFooterZombiesReady;
        }
        set
        {
            _headerFooterZombiesReady = value;
            OnPropertyChanged("HeaderFooterZombiesReady");
        }
    }

    public bool IsRegionBorderShown
    {
        get
        {
            return _isRegionBorderShown;
        }
        set
        {
            if (_isRegionBorderShown == value)
            {
                return;
            }
            _isRegionBorderShown = value;
            OnPropertyChanged("IsRegionBorderShown");
        }
    }

    private void FireStartingPageNumberChanged()
    {
        StartingPageNumberChanged?.Invoke();
    }

    public void UpdateTotalPagesOffsetOnPageChange(int updatedPageNumberDifference)
    {
        if (TotalPagesOffset != 0)
        {
            TotalPagesOffset = Math.Max(0, TotalPagesOffset - updatedPageNumberDifference);
        }
    }

    private void UpdateWorksheetHeight()
    {
        WorksheetHeight = WorksheetPageLayoutDisplayHeight * (double)Pages.Count;
    }

    public void SetCopyWhiteboardItemsDelegate(Func<IEnumerable<IWhiteboardItem>, bool> copyWhiteboardItems)
    {
        WorksheetContent.SetCopyWhiteboardItemsDelegate(copyWhiteboardItems);
    }

    public IPageSnapshot GetPageSnapshot(int pageIndex)
    {
        return VisualHelper.CreatePageSnapshot(this, pageIndex);
    }

    public PageIndexRange GetPagesContaining(IWhiteboardItem item)
    {
        return _pageManager.GetPagesContaining(item);
    }

    public IList<IWhiteboardItem> GetWorksheetItemParts(IWhiteboardItem item)
    {
        return _pageManager.GetWorksheetItemParts(item);
    }

    public bool IsCalculableHiddenFromCalculationContext(ICalculatable calc)
    {
        if (calc is ICalculationHideableChild calculationHideableChild)
        {
            return calculationHideableChild.HideFromCalculationContext;
        }
        return false;
    }

    public Point GetWorksheetSectionItemLocation(IWhiteboardItem item)
    {
        return ActiveSection.GetItemLocation(item);
    }

    public void SetWorksheetSectionItemLocation(IWhiteboardItem item, Point worksheetLocation)
    {
        ActiveSection.SetItemLocation(item, worksheetLocation);
    }

    public bool GetItemIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item)
    {
        return ItemManager.GetIgnoreForHorizontalMovementAndWidth(item);
    }

    public bool CanInsertSpace()
    {
        if (!IsContentActive)
        {
            return false;
        }
        return ActiveOrSelectedItems.Count() == 0;
    }

    public int GetItemWorksheetPageIndex(IWhiteboardItem item)
    {
        return _pageManager.GetPageIndex(item);
    }

    public void ToggleInputRegions()
    {
        _model.InputOutputRegionManager.ToggleInputRegions(GetInputRegions());
    }

    public void ToggleOutputRegions()
    {
        _model.InputOutputRegionManager.ToggleOutputRegions(GetOutputRegions());
    }

    private bool CanDeleteSelectedItems()
    {
        return !SelectedItems.IsNullOrEmpty();
    }

    private bool CanDeleteActiveOrSelectedItems()
    {
        return !ActiveOrSelectedItems.IsNullOrEmpty();
    }

    private bool CanDeleteActiveItem()
    {
        if (IsContentActive)
        {
            return ActiveItem != null;
        }
        return false;
    }

    private bool CanToggleHideFromCalculationContext()
    {
        return HideFromCalculationButtonEnabled;
    }

    private bool CanBookmarkTag()
    {
        if (!IsHeaderActive && !IsFooterActive)
        {
            return ActiveOrSelectedItems.Count() == 1;
        }
        return false;
    }

    private bool ErrorCanTraceBackward()
    {
        if (CurrentError != null && CurrentError.OriginalError != null)
        {
            return true;
        }
        return false;
    }

    private bool ErrorCanTraceForward()
    {
        if (CurrentError != null && CurrentErrorChain != null && CurrentErrorChain.FindNextError(CurrentError) != null)
        {
            return true;
        }
        return false;
    }

    private void UpdateHasSelectionProperty()
    {
        HasSelection = SelectedDescendants.Any();
    }

    private void UpdateIsInLimboProperty()
    {
        IsInLimbo = ActiveDescendant == null || ActiveDescendant is IWhiteboardControl;
    }

    private bool CanAddToDictionary()
    {
        if (ActiveDescendant is ISpellCheckRegion spellCheckRegion)
        {
            return spellCheckRegion.CanAddToDictionary();
        }
        return false;
    }

    internal bool AddWordToDictionary(string word)
    {
        if (!DictionaryHelper.AddWordToDictionary(word))
        {
            return false;
        }
        RefreshDictionary();
        FireDictionaryUpdated();
        return true;
    }

    internal void RemoveFromDictionary(string word)
    {
        if (!DictionaryHelper.RemoveFromDictionary(word))
        {
            return;
        }
        RefreshDictionary();
        FireDictionaryUpdated();
    }

    private void FireDictionaryUpdated()
    {
        DictionaryUpdated?.Invoke();
    }

    private void HandleSelectedDescendantsChanged()
    {
        if (IsContentActive)
        {
            UpdateIsRegionBorderShownState();
            OnPropertyChanged("SelectedDescendants");
        }
    }

    protected void FireChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
