using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetDragger
{
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
        if (e.NewValue is WorksheetDragger worksheetDragger2)
        {
            worksheetDragger2.HookupDragSourceEvents(dragSource);
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
        if (e.NewValue is WorksheetDragger worksheetDragger2)
        {
            worksheetDragger2.HookupDropTargetEvents(dropTarget);
        }
    }

    private void FireOnNoDrag()
    {
        if (OnNoDrag != null)
        {
            OnNoDrag();
        }
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

    private void PrepareItemForDragging(UIElement item, AdornerLayer adornerLayer)
    {
        PrepareItemZombiesForDragging(item, adornerLayer);
        DragAdorner dragAdorner = new DragAdorner(item, item);
        adornerLayer.Add(dragAdorner);
        _dragAdorners[item] = dragAdorner;
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

    private void DoDrag(System.Windows.DragEventArgs e)
    {
        if (e.Data.GetData(_dataFormat.Name) is not IWorksheetViewModel || _draggedItems.Count == 0)
        {
            return;
        }
        UpdatePositionsDuringDrag();
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
        double num = (double)ViewModel.Pages.IndexOf(page) * pageLayout.PageBodyHeight;
        double num2 = ((ViewModel.WorksheetPageLayoutMode == PageLayoutMode.DraftMode) ? 0.0 : pageLayout.PageMargin.Top);
        double num3 = num - num2;
        if (pagePoint.Y < num2)
        {
            num3 = num - pagePoint.Y;
        }
        if (pagePoint.Y > num2 + pageLayout.PageBodyHeight)
        {
            num3 = num - (pagePoint.Y - pageLayout.PageBodyHeight);
        }
        double left = pageLayout.PageMargin.Left;
        return new Point(pagePoint.X + left, pagePoint.Y + num3);
    }
}
