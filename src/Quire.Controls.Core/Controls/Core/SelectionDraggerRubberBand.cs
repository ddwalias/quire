#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SelectionDraggerRubberBand
{
    [MirrorVerified("PtcControlsCoreSelectionDraggerRubberBandMechanicalDifferentialTests")]
    private Rect _bounds;

    [MirrorVerified("PtcControlsCoreSelectionDraggerRubberBandMechanicalDifferentialTests")]
    private readonly Pen _pen;

    [MirrorVerified("PtcControlsCoreSelectionDraggerRubberBandMechanicalDifferentialTests")]
    protected override void OnRender(DrawingContext dc) => dc.DrawRectangle(null, _pen, _bounds);
}
