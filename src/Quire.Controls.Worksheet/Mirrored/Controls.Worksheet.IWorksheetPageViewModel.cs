using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetPageViewModel
{
    IWorksheetPageBody PageBody { get; }

    IHeaderFooterZombie HeaderZombie { get; set; }

    IHeaderFooterZombie FooterZombie { get; set; }

    IHeaderFooterZombie FirstPageHeaderZombie { get; set; }

    IHeaderFooterZombie FirstPageFooterZombie { get; set; }

    IHeaderFooter Header { get; set; }

    IHeaderFooter Footer { get; set; }

    int PageNumber { get; set; }

    bool HasPageBreak { get; set; }

    bool ShowNonPrintableSpaceIndicator { get; set; }

    double NonPrintableSpaceButtonPosition { get; set; }

    double NonPrintableSpaceIndicatorHeight { get; set; }

    Point GetNavigateOutLocation(Direction direction);

    void AddItemAtLocation(IWhiteboardItem item, Point location);

    void RemoveItem(IWhiteboardItem item);

    void RemoveItems(IEnumerable<IWhiteboardItem> items);

    void UpdateNonPrintableSpaceIndicator(double maxXLocation);
}
