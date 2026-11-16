using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Quire.Calculation;
using Quire.Calculation.Core;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent;
using Quire.Controls.Include;
using Quire.Controls.Mathboard;
using Quire.Controls.Plots;
using Quire.Controls.Text;
using Quire.Controls.Worksheet.Printing;
using Quire.Core;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Services;
using Quire.Wpf;
using Quire.Wpf.Printing;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetViewModel
{
    Point InsertionPoint { get; set; }

    ObservableRangeCollection<IWorksheetPage> VisiblePages { get; }

    ObservableRangeCollection<IWorksheetPage> Pages { get; }

    int PageCount { get; }

    int TrailingEmptyPageCount { get; }

    int StartingPageNumber { get; set; }

    int TotalPagesOffset { get; set; }

    double WorksheetHeight { get; }

    double HeaderTop { get; }

    double FooterTop { get; }

    PageLayoutMode WorksheetPageLayoutMode { get; set; }

    WorksheetPageLayout PageLayout { get; }

    double WorksheetPageLayoutDisplayHeight { get; }

    IWorksheetPageManager PageManager { get; }

    IWhiteboardItem ActiveItem { get; }

    IWhiteboardItem ActiveDescendant { get; }

    IList<IWhiteboardItem> SelectedDescendants { get; }

    ObservableRangeCollection<IWhiteboardItem> WorksheetItems { get; }

    INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems { get; }

    IEnumerable<IWhiteboardItem> ActiveOrSelectedItems { get; }

    DragMode CurrentDragMode { get; set; }

    IWorksheetSection ActiveSection { get; }

    int CurrentPageNumber { get; }

    bool EnableEditCurrentPageNumber { get; set; }

    bool IsContentActive { get; }

    bool IsHeaderActive { get; }

    bool IsFooterActive { get; }

    bool IsFirstPageHeaderActive { get; }

    bool IsFirstPageFooterActive { get; }

    Thickness NonprintableSpaceButtonPosition { get; }

    double NonprintableSpaceButtonHeight { get; }

    TextFormat ContentTextFormat { get; set; }

    TextFormat HeaderTextFormat { get; set; }

    TextFormat FooterTextFormat { get; set; }

    TextFormat FirstPageHeaderTextFormat { get; set; }

    TextFormat FirstPageFooterTextFormat { get; set; }

    bool IsFirstPageHeaderFooterDifferent { get; set; }

    bool DisplayGrid { get; set; }

    bool IsSolveBlockLabelVisible { get; set; }

    bool DisplayHFGrid { get; set; }

    bool DisplayFrame { get; set; }

    bool DisplayHeaderFrame { get; set; }

    bool DisplayFooterFrame { get; set; }

    bool UseMajorGridLines { get; set; }

    bool UseHFMajorGridLines { get; set; }

    NumberFormat DisplayNumberFormat { get; set; }

    MatrixFormat ResultMatrixFormat { get; set; }

    IDictionary<IdLabels, ExtendedFont> LabeledIdFormat { get; set; }

    IWorksheetContent WorksheetContent { get; }

    IHeaderFooterControl Header { get; }

    IHeaderFooterControl Footer { get; }

    IHeaderFooterControl FirstPageHeader { get; }

    IHeaderFooterControl FirstPageFooter { get; }

    bool IsSavedWorksheet { set; }

    double WidthWithNonprintableSpace { get; }

    bool HideFromCalculationButtonChecked { get; set; }

    bool HideFromCalculationButtonEnabled { get; set; }

    bool IsCalculationsEnabled { get; set; }

    bool IsCalculating { get; }

    bool IsAutoCalcOn { get; set; }

    bool IsAutoCalcChecked { get; }

    bool PreRecalculateAll { get; }

    StaticMathFormat ContentMathFormat { get; set; }

    bool OleObjectAutoResize { get; set; }

    string DisplayFilePath { get; set; }

    IRestrictedAreaAlignmentGrid WorksheetGrid { get; }

    XmlLanguage CurrentProofingLanguage { get; }

    IWorksheetVisualHelper VisualHelper { get; }

    IWorksheetPage ActivePage { get; }

    bool HasScriptableControls { get; set; }

    bool IsRegionBorderShown { get; set; }

    event Action StartingPageNumberChanged ;

    event EventHandler Changed ;

    event Action CurrentElementChanged ;

    event Action DictionaryUpdated ;

    int DisplayMessageId(worksheetMessageIdType messageId);

    TextFormat GetCurrentTextFormatClone(TextFormat inputTextFormat);

    void AddItemAtLocation(IWhiteboardItem item, Point location);

    IEquationControl CreateNewEquation(string sExpression);

    void SetStyleValues(TextFormat textFormat, ITextStyle style);

    IImageControl CreateImageControl();

    IRichTextBase CreateTextboxControl(string text = default);

    IFileNameControl CreateFileNameControl();

    IFilePathControl CreateFilePathControl();

    IPageNumberControl CreatePageNumberControl(string template);

    ISavedDateControl CreateSavedDateControl(string template);

    IPlotControl CreateXYPlotControl();

    IPlotControl CreatePolarPlotControl();

    IPlotControl<XyzEquation> CreateContourPlotControl();

    ISpecTableControl CreateSpecTableControl(Nullable<TableCoordinate> dimensions);

    IAggregateMathboardControl CreateSolveBlockControl();

    IAggregateMathboardControl CreateExcelComponentControl();

    IAggregateMathboardControl CreateChartComponentControl();

    IParagraphControl CreateTextBlockControl(string text = default);

    IIncludeWorksheetControl CreateIncludeControl(string filePath);

    IWorksheetPageBreak CreatePageBreakControl();

    IOleObjectControl CreateOleObjectControl(OleObjectControlCreationMethod creationMethod);

    Point GetWorksheetSectionItemLocation(IWhiteboardItem item);

    void SetWorksheetSectionItemLocation(IWhiteboardItem item, Point worksheetLocation);

    bool HandleTextInput(string text);

    bool HandleKeyDown(KeyEventArgs args);

    void HandleDelete();

    void HandleBackspace();

    void HandleEnter();

    void HandleTab(FocusNavigationDirection direction);

    void HandleControlEnter();

    void HandleSelectAll();

    void HandleBlockedPreviewMouseButton(object originalSource);

    bool CanDeleteVerticalSpace(Direction direction);

    void DeleteActiveItem();

    void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem);

    void SelectContiguous(IWhiteboardItem item);

    void SelectItem(IWhiteboardItem item);

    void SelectItems(IEnumerable<IWhiteboardItem> items);

    void ClearSelection();

    void SelectItemsInBounds(IWorksheetControl worksheet, Point start, Point end);

    void OnRequestMoveAndResize(IWorksheetControl worksheetControl, ResizeEventMouseArgs args);

    void HandleCaretLeavesTextArea(IWorksheetControl worksheetControl, CommonRoutedEvents.CaretLeavesTextRegionEventArgs args);

    bool HandlePreviewMouseDown(object originalSource, Point position, bool activateSection, bool rightButton);

    bool HandleMouseDown(object originalSource, bool alreadyHandled);

    bool HandlePreviewMouseUp(object originalSource);

    bool HandleMouseUp(object originalSource, Point position, bool rightButton);

    bool GetItemIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item);

    void ActivateHeader();

    void ActivateFooter();

    void ActivateContent();

    void ActivateHeader(Point insertionPoint);

    void ActivateFooter(Point insertionPoint);

    void ActivateContent(Point insertionPoint);

    void InsertPageBreak();

    void ExpandToIncludePoint(Point pointToInclude);

    void PopulateZombies();

    void ScrollToActiveArea();

    void Activate(IWorksheetControl worksheetControl);

    void MoveItem(IWhiteboardItem item, Point point);

    void MoveItemsBy(IEnumerable<IWhiteboardItem> items, Vector offset);

    void HandleGoToPage(object pageNumber);

    IPageSnapshot GetPageSnapshot(int pageIndex);

    PageIndexRange GetPagesContaining(IWhiteboardItem item);

    int GetItemWorksheetPageIndex(IWhiteboardItem item);

    IList<IWhiteboardItem> GetWorksheetItemParts(IWhiteboardItem item);

    bool FindOnce(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions);

    void ToggleInputRegions();

    void ToggleOutputRegions();

    bool CanToggleRegionsAsInput();

    bool CanToggleRegionsAsOutput();

    void ImportData(string fileName);

    bool CanImportData();

    bool HandleRequestBringIntoView(DependencyObject intoViewObject, Rect bringIntoViewRect);

    IDictionary<IdLabels, ExtendedFont> GetDefaultLabeledIdFormat();

    void SetCopyWhiteboardItemsDelegate(Func<IEnumerable<IWhiteboardItem>, bool> copyWhiteboardItems);

    void HandleTabOutOfText(CustomRoutedEventArgs<FocusNavigationDirection> eventArgs);

    void TraverseWhiteboardItems(IEnumerable<IWhiteboardItem> wbiCollection, Func<IWhiteboardItem, bool, bool> function, bool isWorksheetContentItems);

    void TraverseWhiteboardItems(Func<IWhiteboardItem, bool, bool> function);

    bool IsValidInsertionPoint(Point point);

    void ApplyWorksheetData(IWorksheetPersistentData worksheetData);

    void RefreshDictionary();

    void HandleSetCurrentProofingLanguage(CultureInfo argsParameter);

    void OnRequestRestoreInsertionPoint(IWorksheetControl worksheetControl, RoutedEventArgs eventArgs);

    void RequestNavigateHandler(IWorksheetControl worksheetControl, RequestNavigateEventArgs eventArgs);

    void UpdateIsRegionBorderShownState();

    void ToggleIsSolveBlockLabelVisible();
}
