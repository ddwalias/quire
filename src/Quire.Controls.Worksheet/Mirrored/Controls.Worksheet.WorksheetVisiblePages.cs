using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetVisiblePages
{
    private partial class WorksheetVisiblePagesRange
    {
        private const int InitialFirstIndex = 0;

        private const int InitialLastIndex = -1;

        internal int First { get; private set; }

        internal int Last { get; private set; }

        public WorksheetVisiblePagesRange()
        {
            Reset();
        }

        public void Update(int firstVisiblePageIndex, int lastVisiblePageIndex)
        {
            First = firstVisiblePageIndex;
            Last = lastVisiblePageIndex;
        }

        public bool IsIndexInVisibleRange(int index)
        {
            if (index >= First)
            {
                return index <= Last;
            }
            return false;
        }

        public bool IsAnyIndexOfRangeInVisibleRange(int firstIndex, int lastIndex)
        {
            if (firstIndex <= Last)
            {
                return lastIndex >= First;
            }
            return false;
        }

        public void Reset()
        {
            First = 0;
            Last = -1;
        }
    }

    private readonly ObservableRangeCollection<IWorksheetPage> _pages;

    private readonly WorksheetVisiblePagesRange _visiblePagesRange = new WorksheetVisiblePagesRange();

    private readonly ObservableRangeCollection<IWorksheetPage> _visiblePages = new ObservableRangeCollection<IWorksheetPage>();

    public ObservableRangeCollection<IWorksheetPage> VisiblePages => _visiblePages;

    public int FirstVisiblePageIndex => _visiblePagesRange.First;

    public int LastVisiblePageIndex => _visiblePagesRange.Last;

    public bool IsVisiblePagesLoaded { get; private set; }

    public WorksheetVisiblePages(ObservableRangeCollection<IWorksheetPage> pages)
    {
        _pages = pages;
    }

    public bool IsPageIndexVisible(int pageIndex)
    {
        return _visiblePagesRange.IsIndexInVisibleRange(pageIndex);
    }

    public bool IsAnyPageIndexVisible(int firstPageIndex, int lastPageIndex)
    {
        return _visiblePagesRange.IsAnyIndexOfRangeInVisibleRange(firstPageIndex, lastPageIndex);
    }

    public double SetVisiblePages(int newFirstIndex, int newLastIndex, double pageHeight)
    {
        return UpdateVisiblePages(newFirstIndex, newLastIndex, pageHeight);
    }

    public double UpdateVisiblePagesOnScrollChanged(double verticalOffset, double viewportHeight, double pageHeight)
    {
        if (viewportHeight == 0.0 || pageHeight == 0.0)
        {
            return 0.0;
        }
        IsVisiblePagesLoaded = true;
        int newFirstIndex = Math.Max((int)(verticalOffset / pageHeight), 0);
        int newLastIndex = Math.Min((int)((verticalOffset + viewportHeight) / pageHeight), _pages.Count - 1);
        return UpdateVisiblePages(newFirstIndex, newLastIndex, pageHeight);
    }

    public void UpdateVisiblePagesOnPageAddedIfNeeded(int indexOfFirstAddedPage, double verticalOffset, double viewportHeight, double pageHeight)
    {
        if (IsVisiblePagesLoaded && !IsPageIndexVisible(indexOfFirstAddedPage))
        {
            int lastVisiblePageIndex = LastVisiblePageIndex;
            int num = Math.Max(indexOfFirstAddedPage, lastVisiblePageIndex + 1);
            if (Math.Min((int)((verticalOffset + viewportHeight) / pageHeight), _pages.Count - 1) >= num)
            {
                _visiblePagesRange.Update(FirstVisiblePageIndex, num);
                for (int pageIndex = lastVisiblePageIndex + 1; pageIndex <= num; pageIndex++)
                {
                    AddToEnd(pageIndex);
                }
            }
        }
    }

    public void UpdateVisiblePagesOnPageRemovedIfNeeded(int indexOfFirstRemovedPage)
    {
        if (IsVisiblePagesLoaded && IsPageIndexVisible(indexOfFirstRemovedPage))
        {
            int lastVisiblePageIndex = LastVisiblePageIndex;
            int num = Math.Max(indexOfFirstRemovedPage - 1, 0);
            _visiblePagesRange.Update(FirstVisiblePageIndex, num);
            for (int pageIndex = lastVisiblePageIndex; pageIndex > num; pageIndex--)
            {
                RemoveFromEnd(pageIndex);
            }
        }
    }

    public void RemoveAllPages()
    {
        _visiblePagesRange.Reset();
        _visiblePages.Clear();
        IsVisiblePagesLoaded = false;
    }

    public void Dispose()
    {
        RemoveAllPages();
    }

    private double UpdateVisiblePages(int newFirstIndex, int newLastIndex, double pageHeight)
    {
        if (newFirstIndex != FirstVisiblePageIndex || newLastIndex != LastVisiblePageIndex)
        {
            int firstVisiblePageIndex = FirstVisiblePageIndex;
            int lastVisiblePageIndex = LastVisiblePageIndex;
            _visiblePagesRange.Update(newFirstIndex, newLastIndex);
            if (newLastIndex > lastVisiblePageIndex)
            {
                for (int num = Math.Max(lastVisiblePageIndex + 1, newFirstIndex); num <= newLastIndex; num++)
                {
                    AddToEnd(num);
                }
            }
            if (newFirstIndex < firstVisiblePageIndex)
            {
                for (int pageIndex = Math.Min(firstVisiblePageIndex - 1, newLastIndex); pageIndex >= newFirstIndex; pageIndex--)
                {
                    AddToBeginning(pageIndex);
                }
            }
            if (newLastIndex < lastVisiblePageIndex)
            {
                int firstPageToRemove = Math.Max(firstVisiblePageIndex, newLastIndex + 1);
                for (int pageIndex = lastVisiblePageIndex; pageIndex >= firstPageToRemove; pageIndex--)
                {
                    RemoveFromEnd(pageIndex);
                }
            }
            if (newFirstIndex > firstVisiblePageIndex)
            {
                int lastPageToRemove = Math.Min(lastVisiblePageIndex, newFirstIndex - 1);
                for (int pageIndex = firstVisiblePageIndex; pageIndex <= lastPageToRemove; pageIndex++)
                {
                    RemoveFromBeginning(pageIndex);
                }
            }
        }
        DebugTools.DevAssert(_visiblePages.Count == LastVisiblePageIndex - FirstVisiblePageIndex + 1, "Problem with sinchronization of Visible Pages and Pages collections.\nCount = " + _visiblePages.Count + "\nLast - First + 1 = " + (LastVisiblePageIndex - FirstVisiblePageIndex + 1));
        return pageHeight * (double)FirstVisiblePageIndex;
    }

    private void AddToEnd(int pageIndex)
    {
        IWorksheetPage worksheetPage = _pages[pageIndex];
        DebugTools.DevAssert(worksheetPage != null && !_visiblePages.Contains(worksheetPage), "Problem with sinchronization of Visible Pages and Pages collections: The page exists in the visible pages collection.");
        _visiblePages.Add(worksheetPage);
        UpdatePageItemsVisibility(worksheetPage, isPageVisible: true);
    }

    private void AddToBeginning(int pageIndex)
    {
        IWorksheetPage worksheetPage = _pages[pageIndex];
        DebugTools.DevAssert(worksheetPage != null && !_visiblePages.Contains(worksheetPage), "Problem with sinchronization of Visible Pages and Pages collections: The page exists in the visible pages collection.");
        _visiblePages.Insert(0, worksheetPage);
        UpdatePageItemsVisibility(worksheetPage, isPageVisible: true);
    }

    private void RemoveFromBeginning(int pageIndex)
    {
        DebugTools.DevAssert(_visiblePages.Count > 0, "The visible pages collection is empty.");
        IWorksheetPage worksheetPage = _pages[pageIndex];
        DebugTools.DevAssert(worksheetPage != null && _visiblePages[0] == worksheetPage, "Problem with sinchronization of Visible Pages and Pages collections: The page is not in the visible pages collection.");
        PassFocusToAncestorIfNeeded(worksheetPage);
        _visiblePages.RemoveAt(0);
        UpdatePageItemsVisibility(worksheetPage, isPageVisible: false);
    }

    private void RemoveFromEnd(int pageIndex)
    {
        DebugTools.DevAssert(_visiblePages.Count > 0, "The visible pages collection is empty.");
        bool flag = _pages.Count > pageIndex;
        if (flag)
        {
            DebugTools.DevAssert(_pages[pageIndex] != null && _visiblePages[_visiblePages.Count - 1] == _pages[pageIndex], "Problem with sinchronization of Visible Pages and Pages collections: The page is not in the visible pages collection.");
        }
        PassFocusToAncestorIfNeeded(_visiblePages.Last());
        _visiblePages.RemoveAt(_visiblePages.Count - 1);
        if (flag)
        {
            UpdatePageItemsVisibility(_pages[pageIndex], isPageVisible: false);
        }
    }

    private static void PassFocusToAncestorIfNeeded(IWorksheetPage worksheetPage)
    {
        WorksheetPageControl worksheetPageControl = (WorksheetPageControl)worksheetPage;
        if (worksheetPageControl != null && worksheetPageControl.IsKeyboardFocusWithin)
        {
            PassFocusToPagesAncestor(worksheetPageControl.TryFindAncestorIncludingSelf<IWorksheetControl>());
        }
        else
        {
            DebugTools.AssertNotNull(worksheetPageControl, "WorksheetPageControl should not be null.");
        }
    }

    internal static void PassFocusToPagesAncestor(IWorksheetControl worksheetControl)
    {
        WorksheetControl visibleWorksheetControl = (WorksheetControl)worksheetControl;
        if (visibleWorksheetControl != null && !visibleWorksheetControl.VisiblePagesControl.IsFocused)
        {
            ScrollViewer scrollViewer = null;
            double offset = 0.0;
            if (visibleWorksheetControl.WorksheetPageLayoutMode == PageLayoutMode.DraftMode)
            {
                scrollViewer = visibleWorksheetControl.TryFindAncestorIncludingSelf<ScrollViewer>();
                if (scrollViewer != null)
                {
                    offset = scrollViewer.HorizontalOffset;
                }
            }
            bool focused = visibleWorksheetControl.VisiblePagesControl.Focus();
            if ((scrollViewer != null) & focused)
            {
                scrollViewer.ScrollToHorizontalOffset(offset);
            }
            DebugTools.DevAssert(focused, "The focus was not passed to VisiblePagesControl.");
        }
        else
        {
            DebugTools.AssertNotNull(visibleWorksheetControl, "WorksheetControl should not be null.");
        }
    }
}
