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

public partial interface IWorksheetItemManager
{
    Point GetItemWorksheetLocation(IWhiteboardItem item);

    void SetItemWorksheetLocation(IWhiteboardItem item, Point location);

    Point GetItemWorksheetLocation(IWorksheetPageBreak pagebreak);

    void SetItemWorksheetLocation(IWorksheetPageBreak pagebreak, Point location);

    void SetContainingPage(IWhiteboardItem item, IWorksheetPage containingPage);

    IWorksheetPage GetContainingPage(IWhiteboardItem item);

    void SetItemClipGeometry(IWhiteboardItem item, Geometry clip);

    Geometry GetItemClipGeometry(IWhiteboardItem item);

    long GetItemAddMoveRemoveTimestamp(IWhiteboardItem item);

    void SetItemAddMoveRemoveTimestamp(IWhiteboardItem item, long stamp);

    bool ItemHasPendingSizeChangedEvents(IWhiteboardItem item);

    bool GetItemSplitCheckIsRequired(IWhiteboardItem item);

    void SetItemSplitCheckIsRequired(IWhiteboardItem item, bool required);

    IWhiteboardItem GetItemPrintView(IWhiteboardItem item);

    bool GetShouldAssertOnLocation(IWhiteboardItem item);

    void SetShouldAssertOnLocation(IWhiteboardItem item, bool value);

    void ClearShouldAssertOnLocation(IWhiteboardItem item);

    IEnumerable<IWhiteboardItem> SmartDescendantsSatisfyingCondition(IWhiteboardItem startItem, Predicate<IWhiteboardItem> condition);

    void HookupAreaBorderChanged(IWhiteboardItem item);
}
