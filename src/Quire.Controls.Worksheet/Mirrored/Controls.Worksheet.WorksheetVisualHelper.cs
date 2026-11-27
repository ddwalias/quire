using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Worksheet.Printing;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetVisualHelper
{
    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    IWhiteboardItem IWorksheetVisualHelper.FindWhiteboardItemAncestor(object child)
    {
        return FindWhiteboardItemAncestor(child);
    }

    internal static IWhiteboardItem FindWhiteboardItemAncestor(object child)
    {
        return (child as DependencyObject).TryFindVisualAncestorLimited<IWhiteboardItem, IWorksheetControl>();
    }

    IWhiteboardItem IWorksheetVisualHelper.FindWhiteboardItemAncestorIncludingSelf(object child)
    {
        return FindWhiteboardItemAncestorIncludingSelf(child);
    }

    internal static IWhiteboardItem FindWhiteboardItemAncestorIncludingSelf(object child)
    {
        return (child as DependencyObject).TryFindAncestorIncludingSelfLimited<IWhiteboardItem, IWorksheetControl>();
    }

    IWhiteboardItem IWorksheetVisualHelper.FindTopWhiteboardItemAncestorIncludingSelf(object source)
    {
        return FindTopWhiteboardItemAncestorIncludingSelf(source);
    }

    internal static IWhiteboardItem FindTopWhiteboardItemAncestorIncludingSelf(object source)
    {
        IWhiteboardItem result = null;
        for (IWhiteboardItem whiteboardItem = FindWhiteboardItemAncestorIncludingSelf(source); whiteboardItem != null; whiteboardItem = FindWhiteboardItemAncestor(whiteboardItem))
        {
            result = whiteboardItem;
        }
        return result;
    }

    public IWhiteboardItemContainer FindContainerAncestor(IWhiteboardItem item)
    {
        return (item as DependencyObject).TryFindVisualAncestor<IWhiteboardItemContainer>();
    }

    public IWorksheetPage FindWorksheetPageAncestor(object child)
    {
        return (child as DependencyObject).TryFindAncestorIncludingSelfLimited<IWorksheetPage, IWorksheetControl>();
    }

    public UIElement FindUIElementAncestorOrSelf(object element)
    {
        return (element as DependencyObject).TryFindAncestorIncludingSelf<UIElement>();
    }

    public IWorksheetPageSection FindPageSectionContainingPoint(Point location, IWorksheetControl worksheet)
    {
        if (!(worksheet is UIElement uielement))
        {
            DebugTools.FailAssertion(worksheet?.GetType().Name);
            return null;
        }
        Point hitPoint = uielement.TranslatePoint(location, uielement);
        return (uielement.InputHitTest(hitPoint) as DependencyObject).TryFindAncestorIncludingSelfLimited<IWorksheetPageSection, IWorksheetControl>();
    }

    public IWorksheetControl FindWorksheetControlAncestor(object child)
    {
        return (child as DependencyObject).TryFindAncestorIncludingSelf<IWorksheetControl>();
    }

    public object FindOneOfPageRelatedAncestorsIncludingSelf(object child)
    {
        return WpfUtils.TryFindFirstAncestorIncludingSelfLimited<IWorksheetControl>(child as DependencyObject, new List<Type>
        {
            typeof(SwitchToDraftModeButton),
            typeof(IWorksheetPage),
            typeof(IWorksheetPageSection)
        });
    }

    public Point TranslatePoint(Point point, object transformFrom, object transformTo)
    {
        UIElement uIElement = transformFrom as UIElement;
        UIElement uIElement2 = transformTo as UIElement;
        if (uIElement == null || uIElement2 == null)
        {
            return point;
        }
        return uIElement.TranslatePoint(point, uIElement2);
    }

    public bool ShouldToggleSelect
    {
        get
        {
            return ((int)Keyboard.Modifiers & 2) > 0;
        }
    }

    public bool ShouldContiguousSelect
    {
        get
        {
            return ((int)Keyboard.Modifiers & 4) > 0;
        }
    }

    public Rect TransformBounds(IWhiteboardItem transformFrom, object transformTo, Rect boxToTransform)
    {
        Rect result = default;
        Visual visualFrom = transformFrom as Visual;
        Visual visualTo = transformTo as Visual;
        if (visualFrom == null || visualTo == null)
        {
            throw new ArgumentException("Bad programmer! Arguments weren't Visuals!");
        }
        if (FindWorksheetControlAncestor(visualFrom) != null)
        {
            result = visualFrom.TransformToVisual(visualTo).TransformBounds(boxToTransform);
        }
        return result;
    }

    public bool IsSignificantDistance(Vector vector)
    {
        return WpfUtils.IsSignificantMouseMove(vector);
    }

    public void BringIntoView(IWhiteboardItem item)
    {
        if (item is FrameworkElement frameworkElement)
        {
            if (frameworkElement is ICollapsibleAreaControl)
            {
                frameworkElement.BringIntoView(new Rect(new Size(0.0, frameworkElement.ActualHeight)));
            }
            else
            {
                frameworkElement.BringIntoView();
            }
        }
        else
        {
            DebugTools.FailAssertion("The given item should be a FrameworkElement.");
        }
    }

    public void BringIntoView(IWhiteboardItem item, Rect targetRect)
    {
        if (item is FrameworkElement frameworkElement)
        {
            frameworkElement.BringIntoView(targetRect);
        }
        else
        {
            DebugTools.FailAssertion("The given item should be a FrameworkElement.");
        }
    }

    public void BringIntoView(IWorksheetPageSection section, Rect target)
    {
        if (target == Rect.Empty)
        {
            return;
        }
        Visual visual = (Visual)section;
        if (visual == null)
        {
            return;
        }
        if (!AttachedProperties.GetIsBringToView(visual))
        {
            return;
        }
        FrameworkElement worksheetFrameworkElement = WorksheetPropertiesManager.GetWorksheetFrameworkElement(visual);
        if (worksheetFrameworkElement == null)
        {
            return;
        }
        GeneralTransform transform = visual.TransformToVisual(worksheetFrameworkElement);
        worksheetFrameworkElement.BringIntoView(transform.TransformBounds(target));
    }

    public void SuppressBringIntoViewAndExecute(IWorksheetPageSection pageSection, Action action)
    {
        AttachedProperties.SuppressBringIntoViewIfNeededAndExecute(pageSection as DependencyObject, action, suppressBringToView: true);
    }

    public void ScrollIntoView(IWhiteboardItem item, Rect workingRect)
    {
        Rect mainScrollViewerBounds;
        IScrollInfo scrollViewerRect = GetScrollViewerRect(item, out mainScrollViewerBounds);
        if (scrollViewerRect == null)
        {
            return;
        }
        double zoom = GetEngineeringDocumentZoomValue(item);
        workingRect.Scale(zoom, zoom);
        if (!mainScrollViewerBounds.Contains(workingRect))
        {
            double originalTop = mainScrollViewerBounds.Top;
            double originalLeft = mainScrollViewerBounds.Left;
            mainScrollViewerBounds.Union(workingRect);
            double offsetY;
            if (originalTop != mainScrollViewerBounds.Top)
            {
                offsetY = mainScrollViewerBounds.Top - originalTop;
            }
            else
            {
                offsetY = mainScrollViewerBounds.Height - scrollViewerRect.ViewportHeight;
            }
            double offsetX;
            if (originalLeft != mainScrollViewerBounds.Left)
            {
                offsetX = mainScrollViewerBounds.Left - originalLeft;
            }
            else
            {
                offsetX = mainScrollViewerBounds.Width - scrollViewerRect.ViewportWidth;
            }
            offsetY += AdjustScrollToOffset(offsetY);
            offsetX += AdjustScrollToOffset(offsetX);
            scrollViewerRect.SetVerticalOffset(scrollViewerRect.VerticalOffset + offsetY);
            scrollViewerRect.SetHorizontalOffset(scrollViewerRect.HorizontalOffset + offsetX);
        }
    }

    private double AdjustScrollToOffset(double offset)
    {
        if (offset > 0.0)
        {
            return 16.0;
        }
        if (offset < 0.0)
        {
            return -16.0;
        }
        return 0.0;
    }

    public IScrollInfo GetScrollViewerRect(object child, out Rect mainScrollViewerBounds)
    {
        IScrollInfo scrollInfo = (child as DependencyObject).TryFindVisualAncestor<IScrollInfo>();
        while (scrollInfo != null && scrollInfo.ScrollOwner == null)
        {
            scrollInfo = (scrollInfo as DependencyObject).TryFindVisualAncestor<IScrollInfo>();
        }
        if (scrollInfo != null)
        {
            mainScrollViewerBounds = new Rect(scrollInfo.HorizontalOffset, scrollInfo.VerticalOffset, scrollInfo.ViewportWidth, scrollInfo.ViewportHeight);
            return scrollInfo;
        }
        mainScrollViewerBounds = default;
        return null;
    }

    private double GetEngineeringDocumentZoomValue(IWhiteboardItem item)
    {
        return (item as DependencyObject).TryFindVisualAncestor<IEngineeringDocument>().Zoom;
    }

    public IDictionary<IdLabels, ExtendedFont> GetDefaultLabeledIdFormat()
    {
        IDictionary<IdLabels, ExtendedFont> labeledIdFormat = new Dictionary<IdLabels, ExtendedFont>();
        labeledIdFormat[IdLabels.Variable] = FontConstants.GetDefaultIdLabelFormat(IdLabels.Variable).Clone();
        labeledIdFormat[IdLabels.Unit] = FontConstants.GetDefaultIdLabelFormat(IdLabels.Unit).Clone();
        labeledIdFormat[IdLabels.Constant] = FontConstants.GetDefaultIdLabelFormat(IdLabels.Constant).Clone();
        labeledIdFormat[IdLabels.Function] = FontConstants.GetDefaultIdLabelFormat(IdLabels.Function).Clone();
        labeledIdFormat[IdLabels.System] = FontConstants.GetDefaultIdLabelFormat(IdLabels.System).Clone();
        labeledIdFormat[IdLabels.Keyword] = FontConstants.GetDefaultIdLabelFormat(IdLabels.Keyword).Clone();
        return labeledIdFormat;
    }

    public Point GetVisualLocation(IWhiteboardItem item)
    {
        WpfUtils.DoEvents(Dispatcher.CurrentDispatcher, (DispatcherPriority)6);
        FrameworkElement worksheetFrameworkElement = WorksheetPropertiesManager.GetWorksheetFrameworkElement((Visual)item);
        return TranslatePoint(default, item, worksheetFrameworkElement);
    }

    public void BringItemsBoundingBoxIntoView(IEnumerable<IWhiteboardItem> items, PageLayoutMode mode)
    {
        if (items.Count() == 0)
        {
            return;
        }
        Func<IWhiteboardItem, Rect> getVisualRect = (IWhiteboardItem item) => new Rect(GetVisualLocation(item), ((UIElement)item).RenderSize);
        WpfUtils.QueueBackgroundTask(() =>
        {
            Rect target = getVisualRect(items.First());
            items.Each((IWhiteboardItem item) =>
            {
                target.Union(getVisualRect(item));
            });
            if (items.Any((IWhiteboardItem item) => item is IPageWideSizable) && mode == PageLayoutMode.DraftMode)
            {
                target.X -= WorksheetPageLayout.DraftModeGutterWidthInDips;
            }
            FrameworkElement worksheetFrameworkElement = WorksheetPropertiesManager.GetWorksheetFrameworkElement((Visual)items.First());
            if (worksheetFrameworkElement != null)
            {
                worksheetFrameworkElement.BringIntoView(target);
            }
        });
    }

    public IPageSnapshot CreatePageSnapshot(IWorksheetViewModel wsViewModel, int pageIndex)
    {
        PageSnapshot pageSnapshot = null;
        using (new CommandScope(new CompoundCommand()))
        {
            IWorksheetPage worksheetPage = wsViewModel.Pages[pageIndex];
            pageSnapshot = new PageSnapshot(worksheetPage.PageBody.GridCellSize)
            {
                PageNumber = worksheetPage.PageNumber,
                Header = CreateHeaderFooterSnapshot((wsViewModel.IsFirstPageHeaderFooterDifferent && worksheetPage.PageNumber == 1) ? wsViewModel.FirstPageHeader : wsViewModel.Header),
                Footer = CreateHeaderFooterSnapshot((wsViewModel.IsFirstPageHeaderFooterDifferent && worksheetPage.PageNumber == 1) ? wsViewModel.FirstPageFooter : wsViewModel.Footer)
            };
            pageSnapshot.PageBody.WaterMark = worksheetPage.PageBody.WaterMark;
            Rect pageBounds = new Rect(0.0, 0.0, wsViewModel.PageLayout.PageBodyWidth + wsViewModel.PageLayout.PageMargin.Right, wsViewModel.PageLayout.PageBodyHeight);
            pageSnapshot.PageBody.Clip = new RectangleGeometry(pageBounds);
            foreach (IWhiteboardItem whiteboardItem in worksheetPage.WhiteboardItems)
            {
                Point itemPageLocation;
                Geometry clip;
                IWhiteboardItemZombie zombie = whiteboardItem as IWhiteboardItemZombie;
                if (zombie != null)
                {
                    ZombieCreator.ConvertPageLocationAndClipBoundsFromZombieToItem(zombie, out itemPageLocation, out var itemClipBounds);
                    clip = !itemClipBounds.HasValue ? null : new RectangleGeometry(itemClipBounds.Value);
                }
                else
                {
                    itemPageLocation = ItemManager.GetItemLocation(whiteboardItem);
                    clip = ItemManager.GetItemClipGeometry(whiteboardItem);
                }
                if (itemPageLocation.X <= pageSnapshot.PageBody.Clip.Bounds.Right)
                {
                    IWhiteboardItem targetItem = (zombie != null) ? zombie.Item : whiteboardItem;
                    ISupportPrintViewport supportPrintViewport = targetItem as ISupportPrintViewport;
                    if (supportPrintViewport != null)
                    {
                        supportPrintViewport.PrintViewport = Rect.Offset(pageBounds, -itemPageLocation.X, -(itemPageLocation.Y + targetItem.VerticalAlignmentOffset));
                    }
                    IWhiteboardItem itemPrintView = ItemManager.GetItemPrintView((zombie != null) ? zombie.Item : whiteboardItem);
                    if (supportPrintViewport != null)
                    {
                        supportPrintViewport.PrintViewport = null;
                    }
                    if (itemPrintView == null)
                    {
                        throw new NullReferenceException("element printView operation failed");
                    }
                    ItemManager.SetItemLocation(itemPrintView, itemPageLocation);
                    ItemManager.SetItemClipGeometry(itemPrintView, clip);
                    pageSnapshot.PageBody.WhiteboardItems.Add(itemPrintView);
                }
            }
        }
        return pageSnapshot;
    }

    private static IHeaderFooterControl CreateHeaderFooterSnapshot(IHeaderFooterControl realHeaderFooter)
    {
        IHeaderFooterControl headerFooterControl = new HeaderFooterControl(handleUserInput: false, realHeaderFooter.Role);
        IList<IWhiteboardItem> items = new List<IWhiteboardItem>();
        foreach (IWhiteboardItem whiteboardItem in realHeaderFooter.WhiteboardItems)
        {
            IWhiteboardItem itemPrintView = ItemManager.GetItemPrintView(whiteboardItem);
            if (itemPrintView == null)
            {
                throw new NullReferenceException("element printView operation failed");
            }
            ItemManager.SetItemLocation(itemPrintView, ItemManager.GetItemLocation(whiteboardItem));
            items.Add(itemPrintView);
        }
        headerFooterControl.WhiteboardItems.AddRange(items);
        return headerFooterControl;
    }

    public bool ExecuteInThrowawayCommandScope(Action action)
    {
        using (new CommandScope(new CompoundCommand()))
        {
            int undoStackDepth = CommandBuffer.UndoStackDepth;
            action();
            if (undoStackDepth >= CommandBuffer.UndoStackDepth)
            {
                return false;
            }
        }
        return true;
    }

    public bool ExecuteCommandOnItem(IEquationControl control, ICommand command, object parameter)
    {
        RoutedCommand cmd = command as RoutedCommand;
        if (cmd == null)
        {
            return false;
        }
        return ExecuteInThrowawayCommandScope(() =>
        {
            if (cmd.CanExecute(parameter, control as IInputElement))
            {
                cmd.Execute(parameter, control as IInputElement);
            }
        });
    }

    public void DisplayErrorMessage(string message)
    {
        IMessageBoxManager service = AppServiceRegistry.GetService<IMessageBoxManager>();
        Application current = Application.Current;
        service.ShowCriticalErrorMessage(current.MainWindow, message, WpfUtils.ApplicationFullName);
    }

    public void DisplayWarningMessage(string message)
    {
        IMessageBoxManager service = AppServiceRegistry.GetService<IMessageBoxManager>();
        Application current = Application.Current;
        service.ShowCriticalWarningMessage(current.MainWindow, message, WpfUtils.ApplicationFullName);
    }

    public int DisplayMessage(string prompt, MessageBoxImage icon, CheckBoxData[] checkBoxesData, string[] buttonsKey, string defaultButtonKey, string cancelButtonKey)
    {
        IMessageBoxManager service = AppServiceRegistry.GetService<IMessageBoxManager>();
        Application current = Application.Current;
        return service.ShowQuestionWithOptions(current.MainWindow, prompt, WpfUtils.ApplicationFullName, icon, checkBoxesData, buttonsKey, defaultButtonKey, cancelButtonKey);
    }

    public double TranslateScrollViewportLocationToWorksheet(IScrollInfo scrollViewer, IWorksheetControl wc, double yLocationInScrollViewport, Direction direction)
    {
        Point canvasPoint = TranslatePoint(new Point(0.0, yLocationInScrollViewport - scrollViewer.VerticalOffset), scrollViewer, wc);
        double canvasY = canvasPoint.Y;
        WorksheetPageLayout pageLayout = wc.PageLayout;
        double displayHeight = wc.WorksheetPageLayoutDisplayHeight;
        int pageIndex = (int)(canvasY / displayHeight).Floor();
        double yOnPage = canvasY % displayHeight;
        if (wc.WorksheetPageLayoutMode == PageLayoutMode.PageMode)
        {
            yOnPage -= pageLayout.PageMargin.Top;
        }
        double maxLineY = (pageLayout.LinesPerPage - 1) * pageLayout.GridCellSize.Height;
        if (yOnPage.IsLessThan(0.0))
        {
            yOnPage = 0.0;
            if (direction == Direction.Up)
            {
                yOnPage -= pageLayout.GridCellSize.Height;
            }
        }
        else if (yOnPage.IsGreaterThan(maxLineY))
        {
            yOnPage = maxLineY;
            if (direction == Direction.Down)
            {
                yOnPage += pageLayout.GridCellSize.Height;
            }
        }
        return pageLayout.PageBodyHeight * pageIndex + yOnPage;
    }

    public double TranslateWorksheetLocationToScrollViewport(IScrollInfo scrollViewer, IWorksheetControl wc, double yLocationInWorksheet)
    {
        WorksheetPageLayout pageLayout = wc.PageLayout;
        int pageIndex = (int)((yLocationInWorksheet + pageLayout.GridCellSize.Height / 2.0) / pageLayout.PageBodyHeight).Floor();
        IWorksheetPageBody pageBody = wc.PageManager.Pages[pageIndex].PageBody;
        double yOnPage = yLocationInWorksheet % pageLayout.PageBodyHeight;
        return TranslatePoint(new Point(0.0, yOnPage), pageBody, scrollViewer).Y + scrollViewer.VerticalOffset;
    }

    private Point TranslatePageLocationToScrollViewport(IScrollInfo scrollInfo, IWorksheetPageBody pageBody, Point locationOnPage)
    {
        Point translated = TranslatePoint(locationOnPage, pageBody, scrollInfo);
        return new Point(translated.X + scrollInfo.HorizontalOffset, translated.Y + scrollInfo.VerticalOffset);
    }

    private bool IsInScrollViewport(IScrollInfo scrollInfo, Rect scrollViewPort, IWorksheetPageBody pageBody, Rect itemRectInPageBody)
    {
        Point topLeft = TranslatePageLocationToScrollViewport(scrollInfo, pageBody, itemRectInPageBody.TopLeft);
        Point bottomRight = TranslatePageLocationToScrollViewport(scrollInfo, pageBody, itemRectInPageBody.BottomRight);
        DebugTools.DevAssert(topLeft.X.IsLessThan(bottomRight.X) && topLeft.Y.IsLessThan(bottomRight.Y), null);
        Rect rect = new Rect(topLeft, bottomRight);
        return rect.IntersectsWith(scrollViewPort);
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("closure ordinal anchor; Modern Roslyn emits newobj Rect(Size) instead of in-place call Rect(Size) in ternary")]
    public bool IsItemInView(IWhiteboardItem item, IWorksheetControl parentWorksheet) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public Point TranslateWorksheetItemsCanvasPointToWorksheet(Point point, IWorksheetViewModel worksheetViewModel, Direction? direction = null)
    {
        if (worksheetViewModel.WorksheetPageLayoutMode == PageLayoutMode.DraftMode)
        {
            return point;
        }
        WorksheetPageLayout pageLayout = worksheetViewModel.PageLayout;
        double x = point.X - pageLayout.PageMargin.Left;
        double y;
        int pageIndex = (int)(point.Y / pageLayout.DisplayHeight);
        double yOnPage = point.Y - pageIndex * pageLayout.DisplayHeight - pageLayout.PageMargin.Top;
        if (yOnPage < 0.0)
        {
            y = pageIndex * pageLayout.PageBodyHeight;
            if (direction == Direction.Up)
            {
                y -= pageLayout.GridCellSize.Height;
            }
        }
        else if (yOnPage < pageLayout.PageBodyHeight)
        {
            y = yOnPage + pageIndex * pageLayout.PageBodyHeight;
        }
        else
        {
            y = (pageIndex + 1) * pageLayout.PageBodyHeight;
            if (direction == Direction.Down)
            {
                y += pageLayout.GridCellSize.Height;
            }
        }
        return new Point(x, y);
    }

    public void ScrollToPage(IWorksheetPage firstVisiblePage, IWorksheetPage targetPage, bool isGotoPage = false)
    {
        Rect scrollViewerRect;
        IScrollInfo scrollViewer = GetScrollViewerRect(firstVisiblePage, out scrollViewerRect);
        if (scrollViewer != null)
        {
            IWorksheetControl worksheetControl = FindWorksheetControlAncestor(firstVisiblePage);
            if (worksheetControl != null)
            {
                int targetPageNumber = targetPage.PageNumber;
                Direction direction = (targetPageNumber < firstVisiblePage.PageNumber) ? Direction.Up : Direction.Down;
                double viewportHeight = TranslatePoint(new Point(0.0, scrollViewer.ViewportHeight), scrollViewer.ScrollOwner.Content, worksheetControl).Y;
                double pageDisplayHeight = worksheetControl.WorksheetPageLayoutDisplayHeight;
                double yOffset;
                if (isGotoPage)
                {
                    yOffset = (targetPageNumber - 1) * pageDisplayHeight;
                }
                else
                {
                    // Equality ternary keeps Mathcad beq.s branch polarity in Release IL.
                    yOffset = (direction == Direction.Up)
                        ? (targetPageNumber * pageDisplayHeight - 1.0)
                        : ((targetPageNumber - 1) * pageDisplayHeight - viewportHeight + 1.0);
                }
                double verticalOffset = TranslatePoint(new Point(0.0, yOffset), worksheetControl, scrollViewer.ScrollOwner.Content).Y;
                scrollViewer.SetVerticalOffset(verticalOffset);
            }
            else
            {
                DebugTools.FailAssertion("WorksheetControl should not be null.");
            }
        }
        else
        {
            DebugTools.FailAssertion("ScrollViewer should not be null.");
        }
    }

    public void LoadActivePage(IWorksheetPage firstVisiblePage, IWorksheetPage activePage)
    {
        Rect scrollViewerRect;
        IScrollInfo scrollViewer = GetScrollViewerRect(firstVisiblePage, out scrollViewerRect);
        if (scrollViewer != null)
        {
            IWorksheetControl worksheetControl = FindWorksheetControlAncestor(firstVisiblePage);
            if (worksheetControl != null)
            {
                int pageNumber = activePage.PageNumber;
                double pageDisplayHeight = worksheetControl.WorksheetPageLayoutDisplayHeight;
                double viewportCenter = scrollViewer.VerticalOffset + scrollViewer.ViewportHeight / 2.0;
                // Less-than ternary keeps Mathcad blt.s branch polarity in Release IL.
                double verticalOffset = (TranslatePoint(new Point(0.0, activePage.InsertionPoint.Y + (pageNumber - 1) * pageDisplayHeight), worksheetControl, scrollViewer.ScrollOwner.Content).Y < viewportCenter)
                    ? TranslatePoint(new Point(0.0, pageNumber * pageDisplayHeight - 17.0), worksheetControl, scrollViewer.ScrollOwner.Content).Y
                    : (TranslatePoint(new Point(0.0, (pageNumber - 1) * pageDisplayHeight + 17.0), worksheetControl, scrollViewer.ScrollOwner.Content).Y - scrollViewer.ViewportHeight);
                scrollViewer.SetVerticalOffset(verticalOffset);
            }
            else
            {
                DebugTools.FailAssertion("WorksheetControl should not be null.");
            }
        }
        else
        {
            DebugTools.FailAssertion("ScrollViewer should not be null.");
        }
    }

    public WorksheetVisualHelper()
    {
    }
}

