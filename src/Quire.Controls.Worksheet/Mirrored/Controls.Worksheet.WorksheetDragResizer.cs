using System;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetDragResizer
{
    private Point StartPointInElementCoordinates
    {
        get
        {
            return TransformToElement(_startPoint);
        }
    }

    private void HandleResizeEnd(Point location)
    {
        Vector offset = OffsetTo(location);
        switch (_mode)
        {
            case DragMode.ResizeRight:
                HandleMouseUp(offset, changeWidth: true, changeHeight: false);
                break;
            case DragMode.ResizeBottom:
                HandleMouseUp(offset, changeWidth: false, changeHeight: true);
                break;
            case DragMode.ResizeBottomRight:
                HandleMouseUp(offset, changeWidth: true, changeHeight: true);
                break;
            case DragMode.ResizeBottomLeft:
                break;
        }
    }

    private void HandleResizeInProgress(Point location)
    {
        Vector offset = OffsetTo(location);
        switch (_mode)
        {
            case DragMode.ResizeRight:
                Mouse.SetCursor(Cursors.SizeWE);
                ResizeElementBasedOnOffset(ElementToResize, offset, changeWidth: true, changeHeight: false, resizeInProgress: true);
                break;
            case DragMode.ResizeBottomRight:
                Mouse.SetCursor(Cursors.SizeNWSE);
                ResizeElementBasedOnOffset(ElementToResize, offset, changeWidth: true, changeHeight: true, resizeInProgress: true);
                break;
            case DragMode.ResizeBottom:
                Mouse.SetCursor(Cursors.SizeNS);
                ResizeElementBasedOnOffset(ElementToResize, offset, changeWidth: false, changeHeight: true, resizeInProgress: true);
                break;
            case DragMode.ResizeBottomLeft:
                break;
        }
    }

    private void HookupMouseEvents()
    {
        _container.MouseMove += MouseMove;
        _container.MouseUp += MouseUp;
        Mouse.Capture(_container);
    }

    private void UnhookMouseEvents()
    {
        _container.MouseMove -= MouseMove;
        _container.MouseUp -= MouseUp;
        Mouse.Capture(null);
        if (!AttachedProperties.GetIsBringToView(ElementToResize))
        {
            AttachedProperties.SetIsBringToView(ElementToResize, value: true);
            if (DebugTools.IsDebug)
            {
                DebugTools.FailAssertion("IsBringToView is expected to be true for item: " + ElementToResize.GetType().Name);
            }
        }
    }

    private void MouseUp(object sender, MouseButtonEventArgs e)
    {
        ResizeEnd(e.GetPosition(_container));
        UnhookMouseEvents();
    }

    public void ResizeEnd(Point currentPosition)
    {
        HandleResizeEnd(currentPosition);
        ResizableElementAttachedProperties.SetIsResizing(ElementToResize, value: false);
    }

    public Point ResizeBy(Vector offset)
    {
        Point startPointInElementCoordinates = StartPointInElementCoordinates;
        Point val = ElementToResize.TransformToAncestor(_container).Transform(startPointInElementCoordinates + offset);
        MouseMove(val);
        return val;
    }

    private Point TransformToElement(Point point)
    {
        if (_container.IsAncestorOf(ElementToResize))
        {
            return _container.TransformToDescendant(ElementToResize).Transform(point);
        }
        return new Point(0.0, 0.0);
    }

    private void MouseMove(object sender, MouseEventArgs e)
    {
        Point position = e.GetPosition(_container);
        MouseMove(position);
    }

    private void MouseMove(Point currentPoint)
    {
        _container.BringIntoView(new Rect(currentPoint, default(Size)));
        DragProgress(currentPoint);
        HandleResizeInProgress(currentPoint);
    }

    private Vector OffsetTo(Point p)
    {
        return TransformToElement(p) - StartPointInElementCoordinates;
    }

    private void HandleMouseUp(Vector offset, bool changeWidth, bool changeHeight)
    {
        ResizeElementBasedOnOffset(ElementToResize, offset, changeWidth, changeHeight);
        Size arg = new Size(WidthIsAuto ? double.NaN : OriginalWidth, HeightIsAuto ? double.NaN : OriginalHeight);
        Size arg2 = new Size(ElementToResize.Width, ElementToResize.Height);
        DragDone(arg, arg2, _mode);
    }
}
