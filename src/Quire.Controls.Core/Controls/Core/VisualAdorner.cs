#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class VisualAdorner
{
    [MirrorVerified("PtcControlsCoreVisualAdornerMechanicalDifferentialTests")]
    protected virtual void OnLostFocus(object sender, RoutedEventArgs eventArgs) { }

    [MirrorVerified("PtcControlsCoreVisualAdornerMechanicalDifferentialTests")]
    private readonly UIElementCollection _visualChildren;

    [MirrorVerified("PtcControlsCoreVisualAdornerMechanicalDifferentialTests")]
    protected override Visual GetVisualChild(int index) => _visualChildren[index];

    [MirrorVerified("PtcControlsCoreVisualAdornerMechanicalDifferentialTests")]
    public UIElementCollection VisualChildren => _visualChildren;

    [MirrorVerified("PtcControlsCoreVisualAdornerMechanicalDifferentialTests")]
    protected override int VisualChildrenCount => _visualChildren.Count;

    protected VisualAdorner(UIElement adornedElement, bool trackVisibility = false)
        : base(adornedElement, trackVisibility)
    {
        _visualChildren = new UIElementCollection(this, this);
        Hookup();
    }

    protected void Hookup()
    {
        Add();
        Focusable = false;
        IsHitTestVisible = false;
        AdornedElement.LostFocus += OnLostFocus;
        if (AdornedFrameworkElement != null)
        {
            AdornedFrameworkElement.FocusVisualStyle = null;
        }
    }

    protected new void AddVisualChild(Visual child)
    {
        _visualChildren.Add((UIElement)child);
    }

    public override void Remove()
    {
        base.Remove();
        AdornedElement.LostFocus -= OnLostFocus;
    }
}
