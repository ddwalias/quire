#nullable disable
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class VisualDragger
{
    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    protected override void OnContextMenuOpening(ContextMenuEventArgs e) => e.Handled = true;

    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    public void ResetDrag(Point point) => DragStartPoint = point;

    [MirrorVerified("PtcControlsCoreVisualDraggerMechanicalDifferentialTests")]
    public Point DragStartPoint { get; private set; }
}
