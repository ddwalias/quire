#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class yAxis
{
    [MirrorVerified("PtcPersistentDataObjectsyAxisMechanicalDifferentialTests")]
    private string legendpositionField;

    [MirrorVerified("PtcPersistentDataObjectsyAxisMechanicalDifferentialTests")]
    public yAxis()
    {
        legendpositionField = "PlotBoundaryLeft";
    }

    [MirrorVerified("PtcPersistentDataObjectsyAxisMechanicalDifferentialTests")]
    private xyDomainType xyDomainField;

    [MirrorVerified("PtcPersistentDataObjectsyAxisMechanicalDifferentialTests")]
    public xyDomainType xyDomain { get => xyDomainField; set => xyDomainField = value; }

    [MirrorVerified("PtcPersistentDataObjectsyAxisMechanicalDifferentialTests")]
    public string legendposition { get => legendpositionField; set => legendpositionField = value; }
}
