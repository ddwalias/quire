#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class xAxis
{
    [MirrorVerified("PtcPersistentDataObjectsxAxisMechanicalDifferentialTests")]
    private string legendpositionField;

    [MirrorVerified("PtcPersistentDataObjectsxAxisMechanicalDifferentialTests")]
    public xAxis()
    {
        legendpositionField = "PlotBoundaryBottom";
    }

    [MirrorVerified("PtcPersistentDataObjectsxAxisMechanicalDifferentialTests")]
    private xyDomainType xyDomainField;

    [MirrorVerified("PtcPersistentDataObjectsxAxisMechanicalDifferentialTests")]
    public xyDomainType xyDomain { get => xyDomainField; set => xyDomainField = value; }

    [MirrorVerified("PtcPersistentDataObjectsxAxisMechanicalDifferentialTests")]
    public string legendposition { get => legendpositionField; set => legendpositionField = value; }
}
