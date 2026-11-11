#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class UIElementFacade
{
    [MirrorVerified("PtcControlsCoreUIElementFacadeMechanicalDifferentialTests")]
    public UIElementFacade() { }

    [MirrorVerified("PtcControlsCoreUIElementFacadeMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }

    public bool SetFocus(object obj)
    {
        return GetUIElement(obj).Focus();
    }

    public void Measure(object obj, Size availableSize)
    {
        GetUIElement(obj).Measure(availableSize);
    }

    public void Arrange(object obj, Rect finalRect)
    {
        GetUIElement(obj).Arrange(finalRect);
    }

    public Size GetRenderSize(object obj)
    {
        Size renderSize;
        UIElement regionBorder = obj is IRegionWithOuterParts ? (obj as IRegionWithOuterParts).RegionBorder : null;
        if (regionBorder != null)
        {
            renderSize = regionBorder.RenderSize;
        }
        else
        {
            UIElement element = GetUIElement(obj);
            renderSize = element.RenderSize;
            if (renderSize.Height == 0 && renderSize.Width == 0 && !element.IsVisible && element.IsMeasureValid)
                renderSize = GetDesiredSize(obj);
        }
        return renderSize;
    }

    public Size GetDesiredSize(object obj)
    {
        return GetUIElement(obj).DesiredSize;
    }

    public virtual Rect GetDescendantBounds(object obj)
    {
        Rect bounds = VisualTreeHelper.GetDescendantBounds(GetUIElement(obj));
        if (IsRectEmpty(bounds))
            bounds = new Rect();
        return bounds;
    }

    public bool IsItemMeasureValid(object obj)
    {
        return GetUIElement(obj).IsMeasureValid;
    }

    public bool IsItemArrangeValid(object obj)
    {
        return GetUIElement(obj).IsArrangeValid;
    }

    private static bool IsRectEmpty(Rect rect)
    {
        return double.IsInfinity(rect.X) && double.IsInfinity(rect.Y) && double.IsInfinity(rect.Width) && double.IsInfinity(rect.Height);
    }

    public void SetRenderTransform(object obj, Transform transform)
    {
        GetUIElement(obj).RenderTransform = transform;
    }

    public Point TranslatePoint(object source, object target, Point sourcePoint)
    {
        return GetUIElement(source).TranslatePoint(sourcePoint, GetUIElement(target));
    }

    public Rect TransformBoundsToAncestor(object descendant, object ancestor, Rect bounds)
    {
        try
        {
            return GetUIElement(descendant).TransformToAncestor(GetUIElement(ancestor)).TransformBounds(bounds);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
        }
        return bounds;
    }

    public void BlockUntilLoaded()
    {
        WpfUtils.DoEvents(Dispatcher.CurrentDispatcher, DispatcherPriority.Loaded);
    }

    public void BlockUntilInput()
    {
        WpfUtils.DoEvents(Dispatcher.CurrentDispatcher, DispatcherPriority.Input);
    }

    protected static UIElement GetUIElement(object obj)
    {
        return obj as UIElement;
    }

    public void ExecuteOnLayoutUpdated(object control, Action action)
    {
        WpfUtils.ExecuteOnLayoutUpdated(GetUIElement(control), action);
    }
}
