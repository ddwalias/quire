#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Include;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls;

internal partial class RegionCloneForImageHelper
{
    private readonly EngineeringDocument _doc;

    private readonly FrameworkElement _fe;

    private RegionImageSettings _imageSettings;

    private Rect? _boundsConstraint;

    public RegionCloneForImageHelper(EngineeringDocument doc, FrameworkElement fe, RegionImageSettings imageSettings, Rect? boundsConstraint)
    {
        _doc = doc;
        _fe = fe;
        _imageSettings = imageSettings;
        _boundsConstraint = boundsConstraint;
    }

    public void ApplyLayoutSettings(Rect bounds, FrameworkElement feClone, Border border, CopyWidgetBase popup)
    {
        DebugTools.DevAssert(_fe != feClone, null);
        double pageWidth = _doc.Worksheet.PageLayout.PageBodyWidth;
        FrameworkElement ancestor = _fe.TryFindVisualAncestor<IWhiteboardControl>(false) as FrameworkElement;
        if (ancestor != null)
        {
            pageWidth = ancestor.ActualWidth;
        }
        double includeWidth = double.NaN;
        if (_fe is IIncludeWorksheetControl)
        {
            includeWidth = DebugTools.DevAssert(_fe.Width.IsValid(), null).Passed ? _fe.Width : pageWidth;
        }
        double margin = 25.0;
        if (_boundsConstraint.HasValue && DebugTools.DevAssert(bounds.IntersectsWith(_boundsConstraint.Value), null).Passed)
        {
            bounds.Intersect(_boundsConstraint.Value);
            margin = 0.0;
        }
        double width = bounds.Width + 2.0 * margin;
        double height = bounds.Height + 2.0 * margin;
        Transform transform = _imageSettings.ApplyZoom ? _doc.LayoutTransform : Transform.Identity;
        Thickness padding = new Thickness(margin - bounds.Left, margin - bounds.Top, 0.0, 0.0);
        feClone.VerticalAlignment = VerticalAlignment.Top;
        feClone.HorizontalAlignment = HorizontalAlignment.Left;
        DebugTools.DevAssert(border.Child == feClone, null);
        if (_imageSettings.ApplyZoom)
        {
            feClone.SetValue(DocumentAttachedProperties.ZoomProperty, _doc.Zoom);
            width *= _doc.Zoom;
            height *= _doc.Zoom;
        }
        if (_imageSettings.ShrinkToFrame)
        {
            double virtualWidth = SystemParameters.VirtualScreenWidth;
            double maxHeight = SystemParameters.VirtualScreenHeight * 0.75;
            double scale = 1.0;
            if (width > virtualWidth)
            {
                scale *= virtualWidth / width;
                height *= scale;
            }
            if (height > maxHeight)
            {
                scale *= maxHeight / height;
            }
            if (scale != 1.0)
            {
                Matrix matrix = transform.Value;
                matrix.Scale(scale, scale);
                transform = new MatrixTransform(matrix);
                pageWidth *= scale;
            }
        }
        border.Padding = padding;
        border.LayoutTransform = transform;
        popup.ParentWidth = pageWidth;
        popup.IncludeWidth = includeWidth;
        popup.ChildBorder.Width = width;
        popup.ChildBorder.Height = height;
    }
}
