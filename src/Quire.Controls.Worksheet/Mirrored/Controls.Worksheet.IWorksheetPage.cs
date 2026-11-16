using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetPage : IDisposable
{
    ObservableRangeCollection<IWhiteboardItem> WhiteboardItems { get; }

    IList<IWhiteboardItem> NonOwnedWhiteboardItems { get; }

    Point InsertionPoint { get; set; }

    IWhiteboardItem ActiveItem { get; }

    IWorksheetPageBody PageBody { get; }

    IHeaderFooter Header { get; set; }

    IHeaderFooter Footer { get; set; }

    IHeaderFooterZombie HeaderZombie { get; set; }

    IHeaderFooterZombie FooterZombie { get; set; }

    IHeaderFooterZombie FirstPageHeaderZombie { get; set; }

    IHeaderFooterZombie FirstPageFooterZombie { get; set; }

    int PageNumber { get; set; }

    bool HasPageBreak { get; set; }

    event Action<IWorksheetPageSection, Rect> RequestScrollTo;

    event Action PageBodyActivated;

    void AddItemAtLocation(IWhiteboardItem item, Point location);

    void RemoveItem(IWhiteboardItem item);

    void RemoveItems(IEnumerable<IWhiteboardItem> items);

    Point GetNavigateOutLocation(Direction direction);

    void Activate(bool isActive);

    void UpdateNonPrintableSpaceIndicator(double maxXLocation);
}
