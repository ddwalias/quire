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

public partial interface IWorksheetVisualHelper
{
    bool ShouldToggleSelect { get; }

    bool ShouldContiguousSelect { get; }

    IWhiteboardItem FindWhiteboardItemAncestor(object child);

    IWhiteboardItem FindWhiteboardItemAncestorIncludingSelf(object child);

    IWhiteboardItemContainer FindContainerAncestor(IWhiteboardItem item);

    IWorksheetPage FindWorksheetPageAncestor(object child);

    UIElement FindUIElementAncestorOrSelf(object child);

    IWorksheetPageSection FindPageSectionContainingPoint(Point location, IWorksheetControl worksheet);

    IWorksheetControl FindWorksheetControlAncestor(object child);

    object FindOneOfPageRelatedAncestorsIncludingSelf(object child);

    Point TranslatePoint(Point point, object translateFrom, object translateTo);

    Rect TransformBounds(IWhiteboardItem fromItem, object transformTo, Rect bounds);

    bool IsSignificantDistance(Vector vector);

    void BringIntoView(IWhiteboardItem item);

    void BringIntoView(IWhiteboardItem item, Rect targetRect);

    void BringIntoView(IWorksheetPageSection section, Rect target);

    void SuppressBringIntoViewAndExecute(IWorksheetPageSection pageSection, Action action);

    void ScrollIntoView(IWhiteboardItem item, Rect target);

    IDictionary<IdLabels, ExtendedFont> GetDefaultLabeledIdFormat();

    Point GetVisualLocation(IWhiteboardItem item);

    void BringItemsBoundingBoxIntoView(IEnumerable<IWhiteboardItem> items, PageLayoutMode mode);

    IPageSnapshot CreatePageSnapshot(IWorksheetViewModel wsViewModel, int pageIndex);

    bool ExecuteCommandOnItem(IEquationControl control, ICommand command, object parameter);

    void DisplayErrorMessage(string message);

    void DisplayWarningMessage(string message);

    int DisplayMessage(string prompt, MessageBoxImage icon, CheckBoxData[] checkBoxesData, string[] buttonsKey, string defaultButtonKey, string cancelButtonKey);

    IScrollInfo GetScrollViewerRect(object child, out Rect mainScrollViewerBounds);

    bool IsItemInView(IWhiteboardItem item, IWorksheetControl parentWorksheet);

    double TranslateScrollViewportLocationToWorksheet(IScrollInfo scrollViewer, IWorksheetControl wc, double yLocationInScrollViewport, Direction direction);

    double TranslateWorksheetLocationToScrollViewport(IScrollInfo scrollViewer, IWorksheetControl wc, double yLocationInWorksheet);

    Point TranslateWorksheetItemsCanvasPointToWorksheet(Point point, IWorksheetViewModel worksheetViewModel, Nullable<Direction> direction = default);

    void ScrollToPage(IWorksheetPage firstVisiblePage, IWorksheetPage targetPage, bool isViewPortCorrection = false);

    void LoadActivePage(IWorksheetPage firstVisiblePage, IWorksheetPage activePage);

    IWhiteboardItem FindTopWhiteboardItemAncestorIncludingSelf(object source);
}
