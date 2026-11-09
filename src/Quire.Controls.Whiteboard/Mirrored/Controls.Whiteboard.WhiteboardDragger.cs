#nullable disable
#pragma warning disable
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Wpf;
using DragEventArgs = System.Windows.DragEventArgs;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardDragger
{
    private static WhiteboardDragger _default;

    public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(WhiteboardDragger), new UIPropertyMetadata(false, IsDragSourceChanged));

    public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(WhiteboardDragger), new UIPropertyMetadata(false, IsDropTargetChanged));

    private FrameworkElement _dragDropManager;
    private FrameworkElement _whiteboardControl;
    protected Point _startLocation;
    private readonly DataFormat _dataFormat = DataFormats.GetDataFormat("WhiteboardDraggerData");
    private object _dragData;
    private readonly WhiteboardItemManager _whiteboardItemManager = new WhiteboardItemManager();

    public static WhiteboardDragger Default
    {
        get
        {
            if (_default == null)
            {
                _default = new WhiteboardDragger();
            }
            return _default;
        }
    }

    private IWhiteboardViewModel ViewModel
    {
        get => _dragData as IWhiteboardViewModel;
        set => _dragData = value;
    }

    public event Action OnNoDrag;

    public static void MouseDown(IWhiteboardControl control)
    {
        Default.DragSourceMouseLeftButtonDown(control as FrameworkElement);
    }

    public static bool GetIsDragSource(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsDragSourceProperty);
    }

    public static void SetIsDragSource(DependencyObject obj, bool value)
    {
        obj.SetValue(IsDragSourceProperty, value);
    }

    private static void IsDragSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        UIElement dragSource = dependencyObject as UIElement;
        if (dragSource == null)
        {
            return;
        }
        if (object.Equals(e.NewValue, true))
        {
            Default.HookupDragSourceEvents(dragSource);
        }
        else
        {
            Default.UnhookDragSourceEvents(dragSource);
        }
    }

    public static bool GetIsDropTarget(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsDropTargetProperty);
    }

    public static void SetIsDropTarget(DependencyObject obj, bool value)
    {
        obj.SetValue(IsDropTargetProperty, value);
    }

    private static void IsDropTargetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        UIElement dropTarget = dependencyObject as UIElement;
        if (dropTarget == null)
        {
            return;
        }
        if (object.Equals(e.NewValue, true))
        {
            HookupDropTargetEvents(dropTarget);
        }
        else
        {
            UnhookDropTargetEvents(dropTarget);
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
        ViewModel?.ActiveOrSelectedItems.Each((IWhiteboardItem item) =>
        {
            if (!_whiteboardItemManager.GetIsBringToView(item))
            {
                _whiteboardItemManager.SetIsBringToView(item, value: true);
                if (DebugTools.IsDebug)
                {
                    DebugTools.FailAssertion("IsBringToView is expected to be true for item: " + item.GetType().Name, Array.Empty<object>());
                }
            }
        });
        OnNoDrag = null;
        ViewModel = null;
    }

    private void DragSourceMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        DragSourceMouseLeftButtonDown(sender as FrameworkElement);
    }

    private void DragSourceMouseLeftButtonDown(FrameworkElement sender)
    {
        _whiteboardControl = sender;
        if (_whiteboardControl == null)
        {
            return;
        }
        _dragDropManager = ((FrameworkElement)_whiteboardControl.GetValue(WhiteboardControl.DragDropManagerProperty)) ?? Application.Current.MainWindow;
        _startLocation = Mouse.GetPosition(_whiteboardControl);
        _dragData = _whiteboardControl.DataContext;
    }

    private void DragSourceMouseLeftButtonUp(object sender, MouseButtonEventArgs e1)
    {
        if (ViewModel != null && ViewModel.CurrentDragMode == DragMode.Move)
        {
            FireOnNoDrag();
        }
        EndDrag();
    }

    private void DragSourceMouseMove(object sender, MouseEventArgs e)
    {
        if (_whiteboardControl == null || ViewModel == null || ViewModel.CurrentDragMode != DragMode.Move)
        {
            return;
        }
        if (!WpfUtils.IsSignificantMouseMove(_startLocation, e.GetPosition(_whiteboardControl)))
        {
            return;
        }
        _dragDropManager.DragEnter += TopWindowDragEnter;
        _dragDropManager.DragOver += TopWindowDragOver;
        _dragDropManager.DragLeave += TopWindowDragLeave;
        DataObject data = new DataObject(_dataFormat.Name, _dragData);
        DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
        VisualCleanup();
        _dragDropManager.DragEnter -= TopWindowDragEnter;
        _dragDropManager.DragOver -= TopWindowDragOver;
        _dragDropManager.DragLeave -= TopWindowDragLeave;
        EndDrag();
    }

    private void TopWindowQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        if (e.EscapePressed)
        {
            e.Action = DragAction.Cancel;
            ViewModel.ActiveOrSelectedItems.Each((IWhiteboardItem item) =>
            {
                Point itemLocation = ViewModel.GetItemLocation(item);
                _whiteboardItemManager.SetItemLocation(item, itemLocation);
                _whiteboardItemManager.SetRenderTransform(item, null);
            });
            ViewModel.CurrentDragMode = DragMode.None;
        }
    }

    private void TopWindowDragEnter(object sender, DragEventArgs e)
    {
        DoDrag(e);
    }

    private void TopWindowDragOver(object sender, DragEventArgs e)
    {
        DoDrag(e);
    }

    private void TopWindowDragLeave(object sender, DragEventArgs e)
    {
        e.Handled = true;
    }

    private void VisualCleanup()
    {
        if (ViewModel != null && ViewModel.ActiveOrSelectedItems.Count() > 0)
        {
            ViewModel.ActiveOrSelectedItems.Each((IWhiteboardItem item) =>
            {
                _whiteboardItemManager.SetRenderTransform(item, Transform.Identity);
            });
        }
    }

    private void DoDrag(DragEventArgs e)
    {
        IWhiteboardViewModel whiteboardViewModel = e.Data.GetData(_dataFormat.Name) as IWhiteboardViewModel;
        if (_whiteboardControl == null || whiteboardViewModel == null || whiteboardViewModel.ActiveOrSelectedItems.Count() == 0)
        {
            return;
        }
        Point position = e.GetPosition(_whiteboardControl);
        Vector offset = new Vector(position.X - _startLocation.X, position.Y - _startLocation.Y);
        whiteboardViewModel.ActiveOrSelectedItems.Each((IWhiteboardItem item) =>
        {
            _whiteboardItemManager.SetRenderTransform(item, _whiteboardItemManager.GetIgnoreForHorizontalMovementAndWidth(item) ? new TranslateTransform(0.0, offset.Y) : new TranslateTransform(offset.X, offset.Y));
        });
        e.Handled = true;
    }

    // Blocked on Roslyn devirtualization of newobj to call (Mathcad IL requires callvirt on WhiteboardCommand.Execute)
    [global::Quire.Mirror.MirrorStub]
    private void DropTargetDrop(object sender, DragEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DropTargetDragEnter(object sender, DragEventArgs e)
    {
        DoDrag(e);
    }

    private void DropTargetDragOver(object sender, DragEventArgs e)
    {
        DoDrag(e);
    }

    private void DropTargetDragLeave(object sender, DragEventArgs e)
    {
        e.Handled = true;
    }

    private void HookupDragSourceEvents(UIElement dragSource)
    {
        dragSource.PreviewQueryContinueDrag += TopWindowQueryContinueDrag;
        dragSource.PreviewMouseLeftButtonDown += DragSourceMouseLeftButtonDown;
        dragSource.PreviewMouseLeftButtonUp += DragSourceMouseLeftButtonUp;
        dragSource.PreviewMouseMove += DragSourceMouseMove;
    }

    private void UnhookDragSourceEvents(UIElement dragSource)
    {
        dragSource.PreviewQueryContinueDrag -= TopWindowQueryContinueDrag;
        dragSource.PreviewMouseLeftButtonDown -= DragSourceMouseLeftButtonDown;
        dragSource.PreviewMouseLeftButtonUp -= DragSourceMouseLeftButtonUp;
        dragSource.PreviewMouseMove -= DragSourceMouseMove;
    }

    private static void HookupDropTargetEvents(UIElement dropTarget)
    {
        dropTarget.AllowDrop = true;
        dropTarget.PreviewDrop += Default.DropTargetDrop;
        dropTarget.PreviewDragEnter += Default.DropTargetDragEnter;
        dropTarget.PreviewDragOver += Default.DropTargetDragOver;
        dropTarget.PreviewDragLeave += Default.DropTargetDragLeave;
    }

    private static void UnhookDropTargetEvents(UIElement dropTarget)
    {
        dropTarget.AllowDrop = false;
        dropTarget.PreviewDrop -= Default.DropTargetDrop;
        dropTarget.PreviewDragEnter -= Default.DropTargetDragEnter;
        dropTarget.PreviewDragOver -= Default.DropTargetDragOver;
        dropTarget.PreviewDragLeave -= Default.DropTargetDragLeave;
    }

    public WhiteboardDragger()
    {
    }
}
