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
        Size unboundedConstraint = new Size(double.PositiveInfinity, double.PositiveInfinity);
        Size expectedRenderSize = base.MeasureOverride(unboundedConstraint);
        Thickness padding = Padding;
        AdjustPadding(expectedRenderSize);
        double horizontalPaddingChange = Padding.Left + Padding.Right - (padding.Left + padding.Right);
        double verticalPaddingChange = Padding.Top + Padding.Bottom - (padding.Top + padding.Bottom);
        _expectedRenderSize = new Size(expectedRenderSize.Width + horizontalPaddingChange, expectedRenderSize.Height + verticalPaddingChange);
        if (horizontalPaddingChange != 0.0 || verticalPaddingChange != 0.0)
        {
            expectedRenderSize = base.MeasureOverride(unboundedConstraint);
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

    private void AdjustPadding(Size expectedRenderSize)
    {
        if (Content == null && (string)Tag != "0" && (string)Tag != "1")
        {
            return;
        }
        DebugTools.DevAssert(Visibility != Visibility.Collapsed, null);
        if (SnapToGrid)
        {
            double contentWidth;
            double contentHeight;
            FrameworkElement frameworkElement = Content as FrameworkElement;
            if (HasContent && DebugTools.AssertNotNull(frameworkElement, null).Passed)
            {
                bool flag = IsPlotTitleOrientationSwapped(frameworkElement);
                contentWidth = (flag ? frameworkElement.DesiredSize.Height : frameworkElement.DesiredSize.Width);
                contentHeight = (flag ? frameworkElement.DesiredSize.Width : frameworkElement.DesiredSize.Height);
                if (frameworkElement.IsArrangeValid && frameworkElement.IsMeasureValid)
                {
                    double x = (flag ? (contentWidth - (frameworkElement.Margin.Top + frameworkElement.Margin.Bottom)) : (contentWidth - (frameworkElement.Margin.Left + frameworkElement.Margin.Right)));
                    double adjustedLength = (flag ? (contentHeight - (frameworkElement.Margin.Left + frameworkElement.Margin.Right)) : (contentHeight - (frameworkElement.Margin.Top + frameworkElement.Margin.Bottom)));
                    DebugTools.DevAssert(_doubleComparer.Compare(x, frameworkElement.ActualWidth) == 0, null);
                    DebugTools.DevAssert(_doubleComparer.Compare(adjustedLength, frameworkElement.ActualHeight) == 0, null);
                }
            }
            else
            {
                contentWidth = expectedRenderSize.Width - (Padding.Left + Padding.Right);
                contentHeight = expectedRenderSize.Height - (Padding.Top + Padding.Bottom);
            }
            double contentDimension = contentWidth + Margin.Left + Margin.Right;
            double halfPaddingValue = GetHalfPaddingValue(contentDimension, GridCellSize.Width);
            double contentHeightWithMargin = contentHeight + Margin.Top + Margin.Bottom;
            double verticalHalfPadding = GetHalfPaddingValue(contentHeightWithMargin, GridCellSize.Height);
            Thickness padding = Padding;
            if (_doubleComparer.Compare(padding.Left, halfPaddingValue) != 0 || _doubleComparer.Compare(padding.Top, verticalHalfPadding) != 0 || _doubleComparer.Compare(padding.Right, halfPaddingValue) != 0 || _doubleComparer.Compare(padding.Bottom, verticalHalfPadding) != 0)
            {
                Padding = new Thickness(halfPaddingValue, verticalHalfPadding, halfPaddingValue, verticalHalfPadding);
            }
        }
        else
        {
            Padding = new Thickness(0.0);
        }
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
        double minimumWidth;
        FrameworkElement frameworkElement = Content as FrameworkElement;
        if (HasContent && DebugTools.AssertNotNull(frameworkElement, null).Passed)
        {
            double num = (IsPlotTitleOrientationSwapped(frameworkElement) ? (frameworkElement.MinHeight + frameworkElement.Margin.Top + frameworkElement.Margin.Bottom) : (frameworkElement.MinWidth + frameworkElement.Margin.Left + frameworkElement.Margin.Right));
            minimumWidth = num + 2.0 * GetHalfPaddingValue(num, GridCellSize.Width);
        }
        else
        {
            minimumWidth = _expectedRenderSize.Width;
            if (IsArrangeValid)
            {
                DebugTools.DevAssert(_doubleComparer.Compare(minimumWidth, ActualWidth) == 0, null);
            }
        }
        return minimumWidth + Margin.Left + Margin.Right;
    }

    public double GetMinLegendPlaceholderHeight()
    {
        if (Visibility == Visibility.Collapsed)
        {
            return 0.0;
        }
        FrameworkElement frameworkElement = Content as FrameworkElement;
        double height;
        bool flag = HasContent && DebugTools.AssertNotNull(frameworkElement, null).Passed;
        if (flag)
        {
            bool isPlotTitleOrLegend;
            if (frameworkElement is PlotTitle || frameworkElement is PlotLegend)
            {
                isPlotTitleOrLegend = true;
            }
            else
            {
                isPlotTitleOrLegend = false;
            }
            flag = isPlotTitleOrLegend;
        }
        if (flag)
        {
            double num = (IsPlotTitleOrientationSwapped(frameworkElement) ? (frameworkElement.MinWidth + frameworkElement.Margin.Left + frameworkElement.Margin.Right) : (frameworkElement.MinHeight + frameworkElement.Margin.Top + frameworkElement.Margin.Bottom));
            return num + 2.0 * GetHalfPaddingValue(num, GridCellSize.Height) + Margin.Top + Margin.Bottom;
        }
        height = _expectedRenderSize.Height;
        if (IsArrangeValid)
        {
            DebugTools.DevAssert(_doubleComparer.Compare(height, ActualHeight) == 0, null);
        }
        return height + Margin.Top + Margin.Bottom;
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
