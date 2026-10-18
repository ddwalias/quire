#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class polarPlotTypeAxesAngularAxis
{
    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesAngularAxisMechanicalDifferentialTests")]
    private string legendpositionField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesAngularAxisMechanicalDifferentialTests")]
    public polarPlotTypeAxesAngularAxis()
    {
        legendpositionField = "PlotBoundaryBottom";
    }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesAngularAxisMechanicalDifferentialTests")]
    private xyDomainType angularDomainField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesAngularAxisMechanicalDifferentialTests")]
    public xyDomainType angularDomain { get => angularDomainField; set => angularDomainField = value; }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesAngularAxisMechanicalDifferentialTests")]
    public string legendposition { get => legendpositionField; set => legendpositionField = value; }
}
