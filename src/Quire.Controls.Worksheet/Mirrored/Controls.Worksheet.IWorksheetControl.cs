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

public partial interface IWorksheetControl
{
    NumberFormat DisplayNumberFormat { get; set; }

    MatrixFormat ResultMatrixFormat { get; set; }

    TextFormat ContentTextFormat { get; set; }

    TextFormat HeaderTextFormat { get; set; }

    TextFormat FooterTextFormat { get; set; }

    ITextStylesManager TextStylesManager { get; set; }

    IDictionary<IdLabels, ExtendedFont> LabeledIdFormat { get; set; }

    MathFormat MathFormat { get; }

    bool IsSavedWorksheet { set; }

    int CurrentPageNumber { get; }

    bool IsScriptingRunWorksheetOpen { get; }

    bool HasScriptingControls { get; }

    bool EnableEditCurrentPageNumber { get; set; }

    int PageCount { get; }

    int StartingPageNumber { get; set; }

    int TotalPagesOffset { get; set; }

    PageLayoutMode WorksheetPageLayoutMode { get; set; }

    double WorksheetPageLayoutDisplayHeight { get; }

    ObservableRangeCollection<IWhiteboardItem> ActiveSectionItems { get; }

    bool IsHeaderActive { get; }

    bool IsFooterActive { get; }

    bool IsFirstPageHeaderFooterDifferent { get; set; }

    IHeaderFooterControl FirstPageHeader { get; }

    IHeaderFooterControl FirstPageFooter { get; }

    Point InsertionPoint { get; }

    ObservableRangeCollection<IWhiteboardItem> WorksheetItems { get; }

    INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems { get; }

    bool IsContentActive { get; }

    bool IsWorksheetContentScoped { get; }

    IWhiteboardItem ActiveItem { get; }

    IWhiteboardItem ActiveDescendant { get; }

    IList<IWhiteboardItem> SelectedDescendants { get; }

    bool DisplayGrid { get; set; }

    bool IsSolveBlockLabelVisible { get; set; }

    bool DisplayHFGrid { get; set; }

    bool UseMajorGridLines { get; set; }

    bool UseHFMajorGridLines { get; set; }

    bool DisplayFrame { get; set; }

    bool DisplayHeaderFrame { get; set; }

    bool DisplayFooterFrame { get; set; }

    string FilePath { get; set; }

    bool CacheWorksheet { get; set; }

    CalculationOptions DocCalculationOptions { get; }

    CalculationVariables DocCalculationVariables { get; }

    bool IsCalculationsEnabled { get; set; }

    bool IsAutoCalcChecked { get; set; }

    bool IsAutoCalcOn { get; set; }

    IIOItemCollection Inputs { get; }

    IIOItemCollection Outputs { get; }

    IWorksheetPageManager PageManager { get; }

    bool ContainsSymbolicEvaluation { get; }

    IBookmarksManager BookmarksManager { get; set; }

    event Action DictionaryUpdated ;

    event Action StartingPageNumberChanged ;

    IWorksheetPersistentData GetWorksheetData();

    void ScrollToActiveArea();

    void ActivateHeader();

    void ActivateFooter();

    void ActivateContent();

    void Activate();

    IPageSnapshot GetPageSnapshot(int pageIndex);

    PageIndexRange GetPagesContaining(IWhiteboardItem whiteboardItem);

    void ApplyWorksheetData(IWorksheetPersistentData data);

    bool FindOnce(string text, bool caret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions);

    void WaitForCalculation();

    void WaitForCalculation(int seconds, DispatcherPriority priority);

    void PreSave(IDocumentSaveOptions saveOptions);

    string GetEngineVersion();

    void UpdateWorksheetLayoutOnScrollChanged(double verticalOffset, double viewportHeight);

    void PrePrintUpdateWorksheetLayout(Win32PrintRange printRange, PageIndexRange pageRange);

    void PrePrintUpdateWorksheetLayout(PageIndexRange pageRange);

    void PostPrintUpdateWorksheetLayout();

    object FindResource(object key);

    IWhiteboardItem TraverseWhiteboardItemsForBookmarkTag(IEnumerable<IWhiteboardItem> itemCollection, string valueTag);

    void TraverseWhiteboardItems(IEnumerable<IWhiteboardItem> wbiCollection, Func<IWhiteboardItem, bool, bool> function, bool isWorksheetContentItems);

    void TraverseWhiteboardItems(Func<IWhiteboardItem, bool, bool> function);

    void RefreshDictionary();

    void UpdateIsRegionBorderShownState();
}
