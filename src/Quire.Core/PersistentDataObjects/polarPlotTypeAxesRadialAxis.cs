#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class polarPlotTypeAxesRadialAxis
{
    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesRadialAxisMechanicalDifferentialTests")]
    private string legendpositionField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesRadialAxisMechanicalDifferentialTests")]
    public polarPlotTypeAxesRadialAxis()
    {
        legendpositionField = "PlotBoundaryLeft";
    }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesRadialAxisMechanicalDifferentialTests")]
    private xyDomainType radialDomainField;

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesRadialAxisMechanicalDifferentialTests")]
    public xyDomainType radialDomain { get => radialDomainField; set => radialDomainField = value; }

    [MirrorVerified("PtcPersistentDataObjectspolarPlotTypeAxesRadialAxisMechanicalDifferentialTests")]
    public string legendposition { get => legendpositionField; set => legendpositionField = value; }
}
