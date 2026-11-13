#nullable disable
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class CommonDragger
{
    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected Point _startPoint;

    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected Vector OffsetTo(Point p) => p - _startPoint;

    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected virtual void MouseUp_ResizeRightMode(Vector offset, UIElement element) => MouseMove_ResizeRightMode(offset, element);

    [MirrorVerified("PtcControlsCoreCommonDraggerMechanicalDifferentialTests")]
    protected virtual void MouseUp_ResizeBottomMode(Vector offset, UIElement element) => MouseMove_ResizeBottomMode(offset, element);

    private void NotifyDragStart()
    {
        if (onDragStart != null)
        {
            onDragStart();
        }
    }

    private void NotifyDragEnd(Point pt)
    {
        if (onDragEnd != null)
        {
            onDragEnd(pt);
        }
    }

    protected void CacheOriginalWidths()
    {
        foreach (UIElement element in _elementsToDrag)
        {
            if (element is FrameworkElement frameworkElement)
            {
                _originalWidthCache.Add(frameworkElement, double.IsNaN(frameworkElement.Width) ? frameworkElement.ActualWidth : frameworkElement.Width);
            }
        }
    }

    protected void CacheOriginalHeights()
    {
        foreach (UIElement element in _elementsToDrag)
        {
            if (element is FrameworkElement frameworkElement)
            {
                _originalHeightCache.Add(frameworkElement, double.IsNaN(frameworkElement.Height) ? frameworkElement.ActualHeight : frameworkElement.Height);
            }
        }
    }

    protected void MouseMove_ResizeRightMode(Vector offset, UIElement element)
    {
        SetMouseCursor(_mode);
        var frameworkElement = element as FrameworkElement;
        if (frameworkElement == null)
        {
            return;
        }
        double originalWidth = _originalWidthCache[frameworkElement];
        double width = originalWidth + offset.X;
        frameworkElement.Width = Math.Max(frameworkElement.MinWidth, Math.Min(width, frameworkElement.MaxWidth));
        if (_preserveDimensions)
        {
            double height = _originalHeightCache[frameworkElement] * width / originalWidth;
            frameworkElement.Height = Math.Max(0.0, height);
        }
    }

    protected void MouseMove_ResizeBottomMode(Vector offset, UIElement element)
    {
        SetMouseCursor(_mode);
        var frameworkElement = element as FrameworkElement;
        if (frameworkElement == null)
        {
            return;
        }
        double originalHeight = _originalHeightCache[frameworkElement];
        double height = originalHeight + offset.Y;
        frameworkElement.Height = Math.Max(frameworkElement.MinHeight, Math.Min(height, frameworkElement.MaxHeight));
        if (_preserveDimensions)
        {
            double width = _originalWidthCache[frameworkElement] * height / originalHeight;
            frameworkElement.Width = Math.Max(0.0, width);
        }
    }

    protected double ResizeLeft(Vector offset, FrameworkElement fe)
    {
        double originalWidth = _originalWidthCache[fe];
        double width = Math.Max(originalWidth - offset.X, fe.MinWidth);
        double result = originalWidth - width;
        fe.Width = width;
        return result;
    }

    protected void MouseMove_ResizeLeftMode(Vector offset, UIElement element)
    {
        SetMouseCursor(_mode);
        var frameworkElement = element as FrameworkElement;
        if (frameworkElement == null)
        {
            return;
        }
        double offsetX = ResizeLeft(offset, frameworkElement);
        frameworkElement.RenderTransform = new TranslateTransform(offsetX, 0.0);
    }

    protected static void SetMouseCursor(DragMode mode)
    {
        Cursor cursor = Cursors.Hand;
        switch (mode)
        {
            case DragMode.Move:
                cursor = Cursors.Hand;
                break;
            case DragMode.ResizeLeft:
            case DragMode.ResizeRight:
                cursor = Cursors.SizeWE;
                break;
            case DragMode.ResizeBottom:
                cursor = Cursors.SizeNS;
                break;
            case DragMode.ResizeBottomLeft:
                cursor = Cursors.SizeNESW;
                break;
            case DragMode.ResizeBottomRight:
                cursor = Cursors.SizeNWSE;
                break;
        }
        Mouse.SetCursor(cursor);
    }
}
