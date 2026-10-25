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
            double num2;
            double num3;
            if (changeWidth && offset.X != 0.0)
            {
                num2 = Math.Max(element.MinWidth, Math.Min(element.MaxWidth, OriginalWidth + offset.X));
                num3 = num2 / num;
                if (num3.IsLessThan(element.MinHeight))
                {
                    num3 = element.MinHeight;
                    num2 = num3 * num;
                }
                else if (num3.IsGreaterThan(element.MaxHeight))
                {
                    num3 = element.MaxHeight;
                    num2 = num3 * num;
                }
                element.Width = num2;
                element.Height = num3;
            }
            else if (changeHeight && offset.Y != 0.0)
            {
                num3 = Math.Max(element.MinHeight, Math.Min(element.MaxHeight, OriginalHeight + offset.Y));
                num2 = num3 * num;
                if (num2.IsLessThan(element.MinWidth))
                {
                    num2 = element.MinWidth;
                    num3 = num2 / num;
                }
                else if (num2.IsGreaterThan(element.MaxWidth))
                {
                    num2 = element.MaxWidth;
                    num3 = num2 / num;
                }
                element.Width = num2;
                element.Height = num3;
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
