#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quire.Controls.Plots;

public partial class PlotItemPlaceholder
{
    private static readonly DoubleComparer _doubleComparer = DoubleComparer.DefaultDoubleComparer;
    private const int Angle90 = 90;
    private const int Angle270 = 270;
    private const int Angle360 = 360;
    private Size _expectedRenderSize = new Size(0.0, 0.0);
    private const double CollapsedHeight = 0.0;
    private const double CollapsedWidth = 0.0;

    public bool SnapToGrid { get; private set; }

    public Size GridCellSize { get; private set; }

    public void Update(Size gridCellSize, bool snapToGrid)
    {
        if (snapToGrid == SnapToGrid && gridCellSize == GridCellSize)
        {
            return;
        }
        SnapToGrid = snapToGrid;
        GridCellSize = gridCellSize;
        if (IsMeasureValid)
        {
            AdjustPadding();
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (VisualChildrenCount > 0 && GetVisualChild(0) is UIElement uIElement)
        {
            uIElement.InvalidateMeasure();
        }
        Size constraint2 = new Size(double.PositiveInfinity, double.PositiveInfinity);
        Size expectedRenderSize = base.MeasureOverride(constraint2);
        Thickness padding = Padding;
        AdjustPadding(expectedRenderSize);
        double num = Padding.Left + Padding.Right - (padding.Left + padding.Right);
        double num2 = Padding.Top + Padding.Bottom - (padding.Top + padding.Bottom);
        _expectedRenderSize = new Size(expectedRenderSize.Width + num, expectedRenderSize.Height + num2);
        if (num != 0.0 || num2 != 0.0)
        {
            expectedRenderSize = base.MeasureOverride(constraint2);
            DebugTools.DevAssert(_doubleComparer.Compare(expectedRenderSize.Width, _expectedRenderSize.Width) == 0 && _doubleComparer.Compare(expectedRenderSize.Height, _expectedRenderSize.Height) == 0, null);
        }
        return _expectedRenderSize;
    }

    internal void AdjustPadding()
    {
        if (Visibility == Visibility.Collapsed)
        {
            return;
        }
        DebugTools.DevAssert(IsMeasureValid, null);
        if (IsArrangeValid)
        {
            DebugTools.DevAssert(_doubleComparer.Compare(_expectedRenderSize.Width, RenderSize.Width) == 0 && _doubleComparer.Compare(_expectedRenderSize.Height, RenderSize.Height) == 0, null);
        }
        AdjustPadding(_expectedRenderSize);
    }

    private static bool IsPlotTitleOrientationSwapped(FrameworkElement element)
    {
        if (!(element is PlotTitle { IsRotationAllowed: not false }))
        {
            return false;
        }
        Transform layoutTransform = element.LayoutTransform;
        if (layoutTransform == null || layoutTransform == Transform.Identity)
        {
            return false;
        }
        RotateTransform rotateTransform = layoutTransform as RotateTransform;
        if (rotateTransform == null && layoutTransform is TransformGroup transformGroup)
        {
            foreach (Transform child in transformGroup.Children)
            {
                rotateTransform = child as RotateTransform;
                if (rotateTransform != null)
                {
                    break;
                }
            }
        }
        if (rotateTransform == null)
        {
            return false;
        }
        int num = (int)Math.Round(rotateTransform.Angle);
        num = (num % 360 + 360) % 360;
        if (num != 90)
        {
            return num == 270;
        }
        return true;
    }

    public double GetMinLegendPlaceholderWidth()
    {
        if (Visibility == Visibility.Collapsed)
        {
            return 0.0;
        }
        double num2;
        FrameworkElement frameworkElement = Content as FrameworkElement;
        if (HasContent && DebugTools.AssertNotNull(frameworkElement, null).Passed)
        {
            double num = (IsPlotTitleOrientationSwapped(frameworkElement) ? (frameworkElement.MinHeight + frameworkElement.Margin.Top + frameworkElement.Margin.Bottom) : (frameworkElement.MinWidth + frameworkElement.Margin.Left + frameworkElement.Margin.Right));
            num2 = num + 2.0 * GetHalfPaddingValue(num, GridCellSize.Width);
        }
        else
        {
            num2 = _expectedRenderSize.Width;
            if (IsArrangeValid)
            {
                DebugTools.DevAssert(_doubleComparer.Compare(num2, ActualWidth) == 0, null);
            }
        }
        return num2 + Margin.Left + Margin.Right;
    }

    private static double GetHalfPaddingValue(double contentDimension, double gridCellDimension)
    {
        double num = gridCellDimension - contentDimension % gridCellDimension;
        if (_doubleComparer.Compare(num, 0.0) == 0 || _doubleComparer.Compare(num, gridCellDimension) == 0)
        {
            num = 0.0;
        }
        return num / 2.0;
    }
}
