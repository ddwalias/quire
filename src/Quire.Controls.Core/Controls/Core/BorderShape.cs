#nullable disable
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.Controls.Core;

internal partial class BorderShape
{
    [MirrorVerified("PtcControlsCoreBorderShapeMechanicalDifferentialTests")]
    public BorderShape() { }

    [MirrorVerified("PtcControlsCoreBorderShapeMechanicalDifferentialTests")]
    protected override Geometry DefiningGeometry => GetBorderGeometry();
}
