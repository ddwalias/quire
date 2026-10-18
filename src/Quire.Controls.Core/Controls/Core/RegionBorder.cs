#nullable disable
using System.Windows;
using System.Windows.Shapes;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class RegionBorder
{
    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    private Shape _busyProgressBar;

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    private void HideBusyStateElements() => _busyProgressBar.Visibility = (Visibility)2;

    [MirrorVerified("PtcControlsCoreRegionBorderMechanicalDifferentialTests")]
    public RegionBorder() { }
}
