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

public partial interface IWorksheetPageManager
{
    ObservableRangeCollection<IWorksheetPage> Pages { get; }

    ObservableRangeCollection<IWorksheetPage> VisiblePages { get; }

    WorksheetPageLayout PageLayout { get; }

    ObservableRangeCollection<IWorksheetPageBreak> Pagebreaks { set; }

    IWorksheetVisualHelper VisualHelper { get; }

    bool IsVisiblePagesLoaded { get; }

    event Action ContentActivated ;

    event Action<IWhiteboardItem> ItemSplit ;

    IWorksheetPage SetInsertionPointOnAppropriatePage(Point insertionPoint);

    void AddItemToAppropriatePage(IWhiteboardItem item);

    void RemoveItemFromAppropriatePage(IWhiteboardItem item);

    void SplitItemIfNeeded(IWhiteboardItem item, IWorksheetPage prevItemContainingPage = default, bool forceSplitEvenIfNoVisiblePages = false);

    void RemoveItemFromAppropriatePage(IWhiteboardItem item, bool removeEmptyPagesAtTheEnd);

    IWorksheetPage AppendNewPage();

    void RemoveEmptyPagesAfterLocation(Point point);

    int GetTrailingEmptyPageCount();

    IWorksheetPage GetItemPage(IWhiteboardItem item);

    IList<IWhiteboardItem> GetWorksheetItemParts(IWhiteboardItem item);

    void HandleItemSizeChanged(IWhiteboardItem item, Size oldSize, Size newSize);

    void RemoveAllPages();

    void HandleItemMoved(IWhiteboardItem item, Point oldLocation);

    void ActivateItemPart(IWhiteboardItem item, int partIndex, bool replaceOnly);

    void ActivateDescendantParentIfNeeded(IWhiteboardItem item, IWhiteboardItem descendantItem);

    Size GetCustomSplitItemRenderSize(ICustomSplitItem customSplitItem);

    int GetPageIndex(double itemLocation);

    int GetPageIndex(double itemLocation, bool roundUpIfNeeded);

    int GetPageIndex(IWhiteboardItem whiteboardItem);

    void HideZombies(IWhiteboardItem item, bool clearItemClip);

    void ShowZombies(IWhiteboardItem item);

    bool ExecuteOrScheduleSplitAttempt(IWhiteboardItem item, IWorksheetPage prevItemContainingPage = default, bool mustExecuteImmediately = false);

    double UpdateVisiblePagesOnScrollChanged(double verticalOffset, double viewportHeight, double pageHeight);

    void UpdateVisiblePagesOnPageAddedIfNeeded(int indexOfFirstAddedPage, double verticalOffset, double viewportHeight, double pageHeight);

    void UpdateVisiblePagesOnPageRemovedIfNeeded(int indexOfFirstRemovedPage);

    void ScrollToPageIfNeeded(IWorksheetPage targetPage);

    void GoToPage(IWorksheetPage targetPage);

    string VisiblePagesRangeToString(int startingPageNumber = 1);

    double PrePrintUpdateVisiblePages(PageIndexRange pageRange, double pageHeight);

    double PostPrintUpdateVisiblePages(double pageHeight);

    Nullable<Rect> GetClippedItemPartRect(IWhiteboardItem realItem, int itemPartIndex, IWorksheetItemManager worksheetItemManager);

    PageIndexRange GetPagesContaining(IWhiteboardItem item);

    void ActivateWorksheetItemPartForcingSplitIfNeeded(IWhiteboardItem topItem, IWhiteboardItem intoViewItem, Rect intoViewItemRect, bool replaceOnly);

    bool IsItemSplit(IWhiteboardItem item);
}
