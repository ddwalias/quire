#nullable disable
using System;
using System.Windows;
using Quire;

namespace Quire.Controls.Core;

public abstract partial class ItemResizerBase
{
    protected readonly FrameworkElement ElementToResize;

    protected readonly double OriginalWidth;

    protected readonly double OriginalHeight;

    protected readonly bool WidthIsAuto;

    protected readonly bool HeightIsAuto;

    private readonly bool _preserveAspectRatio;

    protected ItemResizerBase(FrameworkElement elementToResize, bool preserveAspectRatio)
    {
        ElementToResize = elementToResize;
        _preserveAspectRatio = preserveAspectRatio;
        WidthIsAuto = double.IsNaN(elementToResize.Width);
        HeightIsAuto = double.IsNaN(elementToResize.Height);
        OriginalWidth = (WidthIsAuto ? elementToResize.ActualWidth : elementToResize.Width);
        OriginalHeight = (HeightIsAuto ? elementToResize.ActualHeight : elementToResize.Height);
    }

    protected void ResizeElementBasedOnOffset(FrameworkElement element, Vector offset, bool changeWidth, bool changeHeight, bool resizeInProgress = false)
    {
        if (_preserveAspectRatio)
        {
            double num = OriginalWidth / OriginalHeight;
            DebugTools.DevAssert(!changeWidth || !changeHeight, null);
            double width;
            double height;
            if (changeWidth && offset.X != 0.0)
            {
                width = Math.Max(element.MinWidth, Math.Min(element.MaxWidth, OriginalWidth + offset.X));
                height = width / num;
                if (height.IsLessThan(element.MinHeight))
                {
                    height = element.MinHeight;
                    width = height * num;
                }
                else if (height.IsGreaterThan(element.MaxHeight))
                {
                    height = element.MaxHeight;
                    width = height * num;
                }
                element.Width = width;
                element.Height = height;
            }
            else if (changeHeight && offset.Y != 0.0)
            {
                height = Math.Max(element.MinHeight, Math.Min(element.MaxHeight, OriginalHeight + offset.Y));
                width = height * num;
                if (width.IsLessThan(element.MinWidth))
                {
                    width = element.MinWidth;
                    height = width / num;
                }
                else if (width.IsGreaterThan(element.MaxWidth))
                {
                    width = element.MaxWidth;
                    height = width / num;
                }
                element.Width = width;
                element.Height = height;
            }
        }
        else if (element is IEquationControl equationControl && equationControl.IsResizableInputControl)
        {
            equationControl.HandleSizeChanged(offset, changeWidth, changeHeight, resizeInProgress);
        }
        else
        {
            if (changeWidth && offset.X != 0.0)
            {
                element.Width = Math.Max(element.MinWidth, Math.Min(element.MaxWidth, OriginalWidth + offset.X));
            }
            if (changeHeight && offset.Y != 0.0)
            {
                element.Height = Math.Max(element.MinHeight, Math.Min(element.MaxHeight, OriginalHeight + offset.Y));
            }
        }
    }
}
