#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class polarPlotTypeAxes
{
    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesMechanicalDifferentialTests")]
    public polarPlotTypeAxes() { }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesMechanicalDifferentialTests")]
    private polarPlotTypeAxesAngularAxis angularAxisField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesMechanicalDifferentialTests")]
    public polarPlotTypeAxesAngularAxis angularAxis { get => angularAxisField; set => angularAxisField = value; }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesMechanicalDifferentialTests")]
    private polarPlotTypeAxesRadialAxis radialAxisField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesMechanicalDifferentialTests")]
    public polarPlotTypeAxesRadialAxis radialAxis { get => radialAxisField; set => radialAxisField = value; }
}
