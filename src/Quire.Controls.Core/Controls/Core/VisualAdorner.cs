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
}
