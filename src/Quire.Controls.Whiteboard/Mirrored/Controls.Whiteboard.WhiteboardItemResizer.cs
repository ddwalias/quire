#nullable disable
#pragma warning disable
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Whiteboard;

internal partial class WhiteboardItemResizer : ItemResizerBase, IResizer
{
    private readonly DragMode _mode;
    private readonly FrameworkElement _container;
    private Point _startPoint;
    private Point _currentPoint;
    private bool _started;
    private DispatcherTimer _bringIntoViewDispatcherTimer;
    private Point _pt;

    public event Action<Size, Size, bool, bool, DragMode> DragDone;

    public WhiteboardItemResizer(FrameworkElement container, Point startPosition, DragMode mode, FrameworkElement elementToResize, bool preserveAspectRatio, bool automation)
        : base(elementToResize, preserveAspectRatio)
    {
        _container = container;
        _mode = mode;
        _startPoint = startPosition;
        _currentPoint = startPosition;
        _started = false;
        ResizableElementAttachedProperties.SetIsResizing(ElementToResize, true);
        if (!automation)
        {
            StartCaptureEvents(_startPoint);
        }
    }

    public void ResizeEnd(Point location)
    {
        Delegate2<Vector, FrameworkElement> @delegate = null;
        switch (_mode)
        {
            case DragMode.ResizeRight:
                @delegate = MouseUpResizeRightMode;
                break;
            case DragMode.ResizeBottom:
                @delegate = MouseUpResizeBottomMode;
                break;
            case DragMode.ResizeBottomRight:
                @delegate = MouseUpResizeBottomRightMode;
                break;
            default:
                DebugTools.DevAssert(false, null);
                return;
            case DragMode.ResizeLeft:
                break;
        }
        Vector arg = OffsetTo(location);
        if (@delegate != null)
        {
            @delegate(arg, ElementToResize);
        }
        ResizableElementAttachedProperties.SetIsResizing(ElementToResize, false);
    }

    internal void MouseMove(Point location)
    {
        Vector offset = OffsetTo(location);
        switch (_mode)
        {
            case DragMode.ResizeRight:
                MouseMoveResizeRightMode(offset, ElementToResize);
                break;
            case DragMode.ResizeLeft:
                MouseMoveResizeLeftMode(offset, ElementToResize);
                break;
            case DragMode.ResizeBottom:
                MouseMoveResizeBottomMode(offset, ElementToResize);
                break;
            case DragMode.ResizeBottomRight:
                MouseMoveResizeBottomRightMode(offset, ElementToResize);
                break;
            case DragMode.ResizeBottomLeft:
                break;
        }
    }

    public void StartCaptureEvents(Point startPoint)
    {
        if (_started)
        {
            return;
        }
        _started = true;
        _startPoint = startPoint;
        _container.MouseMove += MouseMove;
        _container.MouseUp += MouseUp;
        Mouse.Capture(_container);
    }

    public void EndCaptureEvents()
    {
        if (!_started)
        {
            return;
        }
        _started = false;
        Mouse.Capture(null);
        _container.MouseMove -= MouseMove;
        _container.MouseUp -= MouseUp;
        if (!AttachedProperties.GetIsBringToView(ElementToResize))
        {
            AttachedProperties.SetIsBringToView(ElementToResize, true);
            if (DebugTools.IsDebug)
            {
                DebugTools.FailAssertion("IsBringToView is expected to be true for item: " + ElementToResize.GetType().Name);
            }
        }
        ResizeEnd(_currentPoint);
    }

    private void MouseUp(object sender, MouseButtonEventArgs e)
    {
        _currentPoint = e.GetPosition(_container);
        EndCaptureEvents();
    }

    private void MouseMove(object sender, MouseEventArgs e)
    {
        _currentPoint = e.GetPosition(_container);
        NotifyResizeProgress(_currentPoint);
    }

    private void NotifyResizeProgress(Point pt)
    {
        if (_bringIntoViewDispatcherTimer == null)
        {
            _bringIntoViewDispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 25), (DispatcherPriority)4, BringIntoViewCallback, _container.Dispatcher);
            _pt = pt;
            _bringIntoViewDispatcherTimer.Start();
        }
        else if (!_bringIntoViewDispatcherTimer.IsEnabled)
        {
            _pt = pt;
            _bringIntoViewDispatcherTimer.Start();
        }
        MouseMove(pt);
    }

    private void BringIntoViewCallback(object sender, EventArgs e)
    {
        _container.BringIntoView(new Rect(_pt, new Size(0.0, 0.0)));
        _bringIntoViewDispatcherTimer.Stop();
    }

    private Vector OffsetTo(Point point)
    {
        return TransformToElement(point) - StartPointInElementCoordinates;
    }

    private void MouseUpResizeRightMode(Vector offset, FrameworkElement element)
    {
        HandleMouseUp(element, offset, true, false);
    }

    private void MouseMoveResizeRightMode(Vector offset, FrameworkElement element)
    {
        Mouse.SetCursor(Cursors.SizeWE);
        HandleMouseMove(element, offset, true, false, _container is ICollapsibleAreaControl && element is IEquationControl equationControl && equationControl.IsScriptableControl);
    }

    private void MouseUpResizeBottomMode(Vector offset, FrameworkElement element)
    {
        HandleMouseUp(element, offset, false, true);
    }

    private void MouseMoveResizeBottomMode(Vector offset, FrameworkElement element)
    {
        Mouse.SetCursor(Cursors.SizeNS);
        HandleMouseMove(element, offset, false, true, _container is ICollapsibleAreaControl && element is IEquationControl equationControl && equationControl.IsScriptableControl);
    }

    private void MouseUpResizeBottomRightMode(Vector offset, FrameworkElement element)
    {
        HandleMouseUp(element, offset, true, true);
    }

    private void MouseMoveResizeBottomRightMode(Vector offset, FrameworkElement element)
    {
        Mouse.SetCursor(Cursors.SizeNWSE);
        HandleMouseMove(element, offset, true, true, _container is ICollapsibleAreaControl && element is IEquationControl equationControl && equationControl.IsScriptableControl);
    }

    private double ResizeLeft(Vector offset, FrameworkElement fe)
    {
        double num = Math.Max(OriginalWidth - offset.X, fe.MinWidth);
        double result = OriginalWidth - num;
        fe.Width = num;
        return result;
    }

    private void MouseMoveResizeLeftMode(Vector offset, FrameworkElement element)
    {
        Mouse.SetCursor(Cursors.SizeWE);
        double offsetX = ResizeLeft(offset, element);
        element.RenderTransform = new TranslateTransform(offsetX, 0.0);
    }

    public Point ResizeBy(Vector offset)
    {
        Point startPointInElementCoordinates = StartPointInElementCoordinates;
        Point val = ElementToResize.TransformToAncestor(_container).Transform(startPointInElementCoordinates + offset);
        MouseMove(val);
        return val;
    }

    private Point StartPointInElementCoordinates => TransformToElement(_startPoint);

    private Point TransformToElement(Point containerPoint)
    {
        if (_container.IsAncestorOf(ElementToResize))
        {
            return _container.TransformToDescendant(ElementToResize).Transform(containerPoint);
        }
        return default;
    }

    private void HandleMouseUp(FrameworkElement element, Vector offset, bool changeWidth, bool changeHeight)
    {
        ResizeElementBasedOnOffset(element, offset, changeWidth, changeHeight);
        Size arg = new Size(WidthIsAuto ? double.NaN : OriginalWidth, HeightIsAuto ? double.NaN : OriginalHeight);
        Size arg2 = new Size(element.Width, element.Height);
        DragDone(arg, arg2, WidthIsAuto, HeightIsAuto, _mode);
    }

    private void HandleMouseMove(FrameworkElement element, Vector offset, bool changeWidth, bool changeHeight, bool resizeInProgress = false)
    {
        ResizeElementBasedOnOffset(element, offset, changeWidth, changeHeight, resizeInProgress);
    }
}
