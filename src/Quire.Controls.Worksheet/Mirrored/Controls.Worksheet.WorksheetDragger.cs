using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetDragger
{
    private readonly IList<IWhiteboardItem> _draggedItems = new List<IWhiteboardItem>();
    private readonly IDictionary<UIElement, DragAdorner> _dragAdorners = new Dictionary<UIElement, DragAdorner>();
    private readonly IList<Adorner> _dragZombieAdorners = new List<Adorner>();
    private IWhiteboardItem _firstDraggedItem;
    private FrameworkElement _dragDropManager;
    private Point _visualStartLocation;
    private Point _worksheetStartLocation;
    private readonly DataFormat _dataFormat = DataFormats.GetDataFormat("WorksheetDraggerData");
    private object _dragData;
    private FrameworkElement _dragDropConsumer;
    public Action<Vector, IEnumerable<IWhiteboardItem>> OnDragDropEnd = delegate
    {
    };
    public Action<Point> OnDragDropMove = delegate
    {
    };
    public static readonly DependencyProperty DragSourceHandlerProperty = DependencyProperty.RegisterAttached(
        "DragSourceHandler",
        typeof(WorksheetDragger),
        typeof(WorksheetDragger),
        new UIPropertyMetadata(DragSourceHandlerChanged));
    public static readonly DependencyProperty DropTargetHandlerProperty = DependencyProperty.RegisterAttached(
        "DropTargetHandler",
        typeof(WorksheetDragger),
        typeof(WorksheetDragger),
        new UIPropertyMetadata(DropTargetHandlerChanged));

    private IWorksheetViewModel ViewModel
    {
        get
        {
            return _dragData as IWorksheetViewModel;
        }
        set
        {
            _dragData = value;
        }
    }

    public void MouseDown(IWorksheetControl control, DependencyObject originalSource)
    {
        DragSourceMouseLeftButtonDownAuxiliary(control as DependencyObject, originalSource);
    }

    public event Action OnNoDrag;

    public static WorksheetDragger GetDragSourceHandler(DependencyObject obj)
    {
        return (WorksheetDragger)obj.GetValue(DragSourceHandlerProperty);
    }

    public static void SetDragSourceHandler(DependencyObject obj, WorksheetDragger value)
    {
        obj.SetValue(DragSourceHandlerProperty, value);
    }

    private static void DragSourceHandlerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not UIElement dragSource)
        {
            return;
        }
        if (e.OldValue is WorksheetDragger worksheetDragger)
        {
            worksheetDragger.UnhookDragSourceEvents(dragSource);
        }
        if (e.NewValue is WorksheetDragger dragger)
        {
            dragger.HookupDragSourceEvents(dragSource);
        }
    }

    public static WorksheetDragger GetDropTargetHandler(DependencyObject obj)
    {
        return (WorksheetDragger)obj.GetValue(DropTargetHandlerProperty);
    }

    public static void SetDropTargetHandler(DependencyObject obj, WorksheetDragger value)
    {
        obj.SetValue(DropTargetHandlerProperty, value);
    }

    private static void DropTargetHandlerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not UIElement dropTarget)
        {
            return;
        }
        if (e.OldValue is WorksheetDragger worksheetDragger)
        {
            worksheetDragger.UnhookDropTargetEvents(dropTarget);
        }
        if (e.NewValue is WorksheetDragger dragger)
        {
            dragger.HookupDropTargetEvents(dropTarget);
        }
    }

    private void FireOnNoDrag()
    {
        if (OnNoDrag != null)
        {
            OnNoDrag();
        }
    }

    private void EndDrag()
    {
        IWorksheetItemManager itemManager = WorksheetViewModel.ItemManager;
        _draggedItems.Each(delegate (IWhiteboardItem item)
        {
            if (!itemManager.GetIsBringToView(item))
            {
                itemManager.SetIsBringToView(item, true);
            }
        });
        OnNoDrag = null;
        ViewModel = null;
    }

    private void DragSourceMouseLeftButtonDownAuxiliary(object sender, DependencyObject originalSource)
    {
        _dragDropConsumer = sender as FrameworkElement;
        if (_dragDropConsumer == null)
        {
            return;
        }
        _dragDropManager = (FrameworkElement)_dragDropConsumer.GetValue(WhiteboardControl.DragDropManagerProperty) ?? Application.Current.MainWindow;
        _dragData = _dragDropConsumer.DataContext;
        try
        {
            _visualStartLocation = MouseUtilities.GetMousePosition(_dragDropConsumer);
        }
        catch (InvalidOperationException ex)
        {
            DebugTools.FailAssertion(ex.Message);
            return;
        }
        _firstDraggedItem = ViewModel.VisualHelper.FindTopWhiteboardItemAncestorIncludingSelf(originalSource);
        if (_firstDraggedItem == null)
        {
            return;
        }
        ViewModel.ActiveOrSelectedItems.Each(delegate (IWhiteboardItem item)
        {
            if (!_draggedItems.Contains(item))
            {
                _draggedItems.Add(item);
            }
        });
    }

    private void DragSourceMouseLeftButtonUp(object sender, MouseButtonEventArgs e1)
    {
        if (ViewModel != null)
        {
            if (ViewModel.CurrentDragMode == DragMode.Move)
            {
                FireOnNoDrag();
            }
            ViewModel.CurrentDragMode = DragMode.None;
        }
        EndDrag();
        IWorksheetControl worksheetControl = ((DependencyObject)e1.OriginalSource).TryFindAncestorIncludingSelf<IWorksheetControl>(includeLogical: true);
        if (DebugTools.AssertNotNull(worksheetControl, null).Passed)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual)worksheetControl);
            CleanupDrag(adornerLayer);
        }
    }

    private void DragSourceMouseMove(object sender, MouseEventArgs e)
    {
        if (ViewModel == null || ViewModel.CurrentDragMode != DragMode.Move)
        {
            return;
        }
        DependencyObject originalSource = e.OriginalSource as DependencyObject;
        IWorksheetControl worksheetControl = originalSource.TryFindAncestorIncludingSelf<IWorksheetControl>(false);
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual)worksheetControl);
        IWorksheetPage page = ViewModel.VisualHelper.FindWorksheetPageAncestor(originalSource);
        if (page == null)
        {
            IWhiteboardItem item = ViewModel.VisualHelper.FindTopWhiteboardItemAncestorIncludingSelf(originalSource);
            if (item != null)
            {
                page = ViewModel.PageManager.GetItemPage(item);
            }
        }
        _worksheetStartLocation = ConvertPageCoordinatesToWorksheetCoordinates(e.GetPosition((IInputElement)page), page);
        _draggedItems.Cast<UIElement>().Each(delegate (UIElement item)
        {
            PrepareItemForDragging(item, adornerLayer);
        });
        _dragDropManager.DragEnter += TopWindowDragEnter;
        _dragDropManager.DragOver += TopWindowDragOver;
        _dragDropManager.DragLeave += TopWindowDragLeave;
        DataObject data = new DataObject(_dataFormat.Name, _dragData);
        DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
        CleanupDrag(adornerLayer);
        _dragDropManager.DragEnter -= TopWindowDragEnter;
        _dragDropManager.DragOver -= TopWindowDragOver;
        _dragDropManager.DragLeave -= TopWindowDragLeave;
        EndDrag();
    }

    private void PrepareItemForDragging(UIElement item, AdornerLayer adornerLayer)
    {
        PrepareItemZombiesForDragging(item, adornerLayer);
        DragAdorner dragAdorner = new DragAdorner(item, item);
        adornerLayer.Add(dragAdorner);
        _dragAdorners[item] = dragAdorner;
    }

    private void PrepareItemZombiesForDragging(UIElement item, AdornerLayer adornerLayer)
    {
        if (!(item is IWhiteboardControl))
        {
            ViewModel.PageManager.HideZombies(item as IWhiteboardItem, false);
            WpfUtils.GetAdornersOfDescendants(item).Each(delegate (Adorner itemAdorner)
            {
                Visual ancestor = itemAdorner.TryFindVisualAncestor<Visual>(false);
                Rect descendantBounds = WorksheetViewModel.ItemManager.GetDescendantBounds(itemAdorner);
                Rect boundingBox = itemAdorner.TransformToVisual(ancestor).TransformBounds(descendantBounds);
                DragZombieAdorner dragZombieAdorner = new DragZombieAdorner(itemAdorner, itemAdorner, boundingBox);
                _dragZombieAdorners.Add(dragZombieAdorner);
                adornerLayer.Add(dragZombieAdorner);
            });
        }
    }

    private void TopWindowQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        if (e.EscapePressed)
        {
            e.Action = DragAction.Cancel;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual)((DependencyObject)e.OriginalSource).TryFindAncestorIncludingSelf<IWorksheetControl>(false));
            CleanupDrag(adornerLayer);
            if (DebugTools.DevAssert(ViewModel.CurrentDragMode == DragMode.None, null).Failed)
            {
                ViewModel.CurrentDragMode = DragMode.None;
            }
        }
        if (ViewModel == null || ViewModel.CurrentDragMode != DragMode.Move)
        {
            return;
        }
        GrowWorksheetIfAppropriate();
        UpdatePositionsDuringDrag();
    }

    private void GrowWorksheetIfAppropriate()
    {
        Point mousePosition = MouseUtilities.GetMousePosition(_dragDropConsumer);
        OnDragDropMove(mousePosition);
        _dragDropConsumer.BringIntoView(new Rect(mousePosition, default(Size)));
    }

    private void TopWindowDragEnter(object sender, System.Windows.DragEventArgs e)
    {
        DoDrag(e);
    }

    private void TopWindowDragOver(object sender, System.Windows.DragEventArgs e)
    {
        DoDrag(e);
    }

    private void TopWindowDragLeave(object sender, System.Windows.DragEventArgs e)
    {
        e.Handled = true;
    }

    private void CleanupDrag(AdornerLayer adornerLayer)
    {
        _draggedItems.Cast<UIElement>().Each(delegate (UIElement item)
        {
            item.RenderTransform = Transform.Identity;
            item.IsHitTestVisible = true;
            item.Visibility = Visibility.Visible;
            if (ViewModel != null && item is not IWhiteboardControl)
            {
                ViewModel.PageManager.ShowZombies(item as IWhiteboardItem);
            }
            _dragAdorners.TryGetValue(item, out DragAdorner adorner);
            if (adorner != null)
            {
                adornerLayer.Remove(adorner);
            }
        });
        _draggedItems.Clear();
        _dragAdorners.Clear();
        _dragZombieAdorners.Each(adornerLayer.Remove);
        _dragZombieAdorners.Clear();
        if (ViewModel != null)
        {
            ViewModel.CurrentDragMode = DragMode.None;
        }
    }

    private void DoDrag(System.Windows.DragEventArgs e)
    {
        if (e.Data.GetData(_dataFormat.Name) is not IWorksheetViewModel || _draggedItems.Count == 0)
        {
            return;
        }
        UpdatePositionsDuringDrag();
        e.Handled = true;
    }

    private void UpdatePositionsDuringDrag()
    {
        Point mousePosition = MouseUtilities.GetMousePosition(_dragDropConsumer);
        Vector currentItemOffset = mousePosition - _visualStartLocation;
        Func<IWhiteboardItem, int> getItemPageNumber = (IWhiteboardItem item) =>
            (int)(ViewModel.GetWorksheetSectionItemLocation(item).Y / ViewModel.PageLayout.PageBodyHeight);
        int firstPageNumber = getItemPageNumber(_firstDraggedItem);
        double visualPageOffset = ViewModel.PageLayout.DisplayHeight - ViewModel.PageLayout.PageBodyHeight;
        if (_draggedItems.Cast<UIElement>().Any((UIElement item) => item is IPageWideSizable))
        {
            currentItemOffset.X = 0.0;
        }
        _draggedItems.Cast<UIElement>().Each(delegate (UIElement item)
        {
            double yOffset = visualPageOffset * (double)(firstPageNumber - getItemPageNumber((IWhiteboardItem)item));
            item.RenderTransform = ViewModel.GetItemIgnoreForHorizontalMovementAndWidth((IWhiteboardItem)item)
                ? new TranslateTransform(0.0, currentItemOffset.Y + yOffset)
                : new TranslateTransform(currentItemOffset.X, currentItemOffset.Y + yOffset);
            item.IsHitTestVisible = false;
            item.Visibility = Visibility.Hidden;
        });
    }

    private void DropTargetDrop(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetData(_dataFormat.Name) is not IWorksheetViewModel viewModel || _draggedItems.Count == 0)
        {
            return;
        }
        IInputElement hitElement = _dragDropManager.InputHitTest(e.GetPosition(_dragDropManager));
        IWorksheetPage dropPage = ViewModel.VisualHelper.FindWorksheetPageAncestor(hitElement);
        if (dropPage == null)
        {
            IWhiteboardItem item = ViewModel.VisualHelper.FindTopWhiteboardItemAncestorIncludingSelf(e.OriginalSource as DependencyObject);
            if (item is FrameworkElement fe && fe.DataContext is not HeaderFooterViewModel)
            {
                dropPage = ViewModel.PageManager.GetItemPage(item);
            }
        }
        Point worksheetDropLocation = (dropPage != null)
            ? ConvertPageCoordinatesToWorksheetCoordinates(e.GetPosition((IInputElement)dropPage), dropPage)
            : _worksheetStartLocation;
        Vector offset = new Vector(worksheetDropLocation.X - _worksheetStartLocation.X, worksheetDropLocation.Y - _worksheetStartLocation.Y);
        if (_draggedItems.Cast<UIElement>().Any((UIElement item) => item is IPageWideSizable))
        {
            offset.X = 0.0;
        }
        List<IWhiteboardItem> draggedItems = _draggedItems.ToList();
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual)((DependencyObject)e.OriginalSource).TryFindAncestorIncludingSelf<IWorksheetControl>(false));
        CleanupDrag(adornerLayer);
        if (DebugTools.DevAssert(viewModel.CurrentDragMode == DragMode.None, null).Failed)
        {
            viewModel.CurrentDragMode = DragMode.None;
        }
        e.Handled = true;
        OnDragDropEnd(offset, draggedItems);
    }

    private void DropTargetDragEnter(object sender, System.Windows.DragEventArgs e)
    {
        DoDrag(e);
    }

    private void DropTargetDragOver(object sender, System.Windows.DragEventArgs e)
    {
        DoDrag(e);
    }

    private void DropTargetDragLeave(object sender, System.Windows.DragEventArgs e)
    {
        IWorksheetItemManager itemManager = WorksheetViewModel.ItemManager;
        _draggedItems.Each(delegate (IWhiteboardItem item)
        {
            if (!itemManager.GetIsBringToView(item))
            {
                itemManager.SetIsBringToView(item, true);
                if (DebugTools.IsDebug)
                {
                    DebugTools.FailAssertion("IsBringToView is expected to be true for item: " + item.GetType().Name);
                }
            }
        });
        e.Handled = true;
    }

    private void HookupDragSourceEvents(UIElement dragSource)
    {
        dragSource.PreviewQueryContinueDrag += TopWindowQueryContinueDrag;
        dragSource.PreviewMouseLeftButtonDown += OnDragSourceOnPreviewMouseLeftButtonDown;
        dragSource.PreviewMouseLeftButtonUp += DragSourceMouseLeftButtonUp;
        dragSource.PreviewMouseMove += DragSourceMouseMove;
    }

    private void OnDragSourceOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
        DragSourceMouseLeftButtonDownAuxiliary(sender, args.OriginalSource as DependencyObject);
    }

    private void UnhookDragSourceEvents(UIElement dragSource)
    {
        dragSource.PreviewQueryContinueDrag -= TopWindowQueryContinueDrag;
        dragSource.PreviewMouseLeftButtonDown -= OnDragSourceOnPreviewMouseLeftButtonDown;
        dragSource.PreviewMouseLeftButtonUp -= DragSourceMouseLeftButtonUp;
        dragSource.PreviewMouseMove -= DragSourceMouseMove;
    }

    private void HookupDropTargetEvents(UIElement dropTarget)
    {
        dropTarget.AllowDrop = true;
        dropTarget.PreviewDrop += DropTargetDrop;
        dropTarget.PreviewDragEnter += DropTargetDragEnter;
        dropTarget.PreviewDragOver += DropTargetDragOver;
        dropTarget.PreviewDragLeave += DropTargetDragLeave;
    }

    private void UnhookDropTargetEvents(UIElement dropTarget)
    {
        dropTarget.AllowDrop = false;
        dropTarget.PreviewDrop -= DropTargetDrop;
        dropTarget.PreviewDragEnter -= DropTargetDragEnter;
        dropTarget.PreviewDragOver -= DropTargetDragOver;
        dropTarget.PreviewDragLeave -= DropTargetDragLeave;
    }

    private Point ConvertPageCoordinatesToWorksheetCoordinates(Point pagePoint, IWorksheetPage page)
    {
        WorksheetPageLayout pageLayout = ViewModel.PageLayout;
        double pageOffset = (double)ViewModel.Pages.IndexOf(page) * pageLayout.PageBodyHeight;
        double topMargin = ((ViewModel.WorksheetPageLayoutMode == PageLayoutMode.DraftMode) ? 0.0 : pageLayout.PageMargin.Top);
        double verticalOffset = pageOffset - topMargin;
        if (pagePoint.Y < topMargin)
        {
            verticalOffset = pageOffset - pagePoint.Y;
        }
        if (pagePoint.Y > topMargin + pageLayout.PageBodyHeight)
        {
            verticalOffset = pageOffset - (pagePoint.Y - pageLayout.PageBodyHeight);
        }
        double left = pageLayout.PageMargin.Left;
        return new Point(pagePoint.X + left, pagePoint.Y + verticalOffset);
    }

    public WorksheetDragger()
    {
    }
}
