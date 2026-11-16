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

public partial interface IWorksheetContent
{
    double ContentAreaWidth { set; }

    ObservableRangeCollection<IWorksheetPageBreak> PageBreaks { get; }

    bool BoundDirectionalMoveByContentWidth { set; }

    StaticMathFormat MathFormat { get; set; }

    IDictionary<IdLabels, ExtendedFont> LabeledIdFormat { get; set; }

    event Action<IWhiteboardItem, IWhiteboardItem> ActiveItemChanged ;

    event Action ActiveDescendantChanged ;

    event Action SelectedDescendantsChanged ;

    event Action InsertionPointSet ;

    event Action<double> NonprintableSpaceWidthIncrease ;

    void SetWidthWithNonprintableSpace(double newWidth);

    void AddPageBreakAtLocation(IWorksheetPageBreak pagebreak, Point location, bool forceAlignment = false);

    Point GetItemLocation(IWorksheetPageBreak pageBreak);

    void SetItemLocation(IWorksheetPageBreak pageBreak, Point location);

    void CommitItem(IWhiteboardItem item);

    void CommitActiveItem();

    IEnumerable<IWhiteboardItem> SortItemsByLocation(IEnumerable<IWhiteboardItem> items, bool descending);

    bool CheckPotentialMove(Dictionary<IWhiteboardItem, Point> itemsAtNewLocations, bool preventRestrictedItemLocations);

    Point CalculateItemWorksheetLocation(IWhiteboardItem item, Point worksheetLocation, GridRestriction restriction);

    void SetCopyWhiteboardItemsDelegate(Func<IEnumerable<IWhiteboardItem>, bool> copyWhiteboardItems);

    IWhiteboardItem GetItemAtLocation(Point location);

    bool IsValidInsertionPoint(Point point);

    void HandleItemGotFocus(IWhiteboardItem item);
}
