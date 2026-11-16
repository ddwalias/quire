using System;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetVisiblePages : IDisposable
{
    ObservableRangeCollection<IWorksheetPage> VisiblePages { get; }

    int FirstVisiblePageIndex { get; }

    int LastVisiblePageIndex { get; }

    bool IsVisiblePagesLoaded { get; }

    bool IsPageIndexVisible(int pageIndex);

    bool IsAnyPageIndexVisible(int firstPageIndex, int lastPageIndex);

    double UpdateVisiblePagesOnScrollChanged(double verticalOffset, double viewportHeight, double pageHeight);

    void UpdateVisiblePagesOnPageAddedIfNeeded(int indexOfFirstAddedPage, double verticalOffset, double viewportHeight, double pageHeight);

    void UpdateVisiblePagesOnPageRemovedIfNeeded(int indexOfFirstRemovedPage);

    void RemoveAllPages();

    double SetVisiblePages(int newFirstIndex, int newLastIndex, double pageHeight = 0.0);
}
