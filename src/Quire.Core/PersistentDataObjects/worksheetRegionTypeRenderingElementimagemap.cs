#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class worksheetRegionTypeRenderingElementimagemap
{
    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingElementimagemapMechanicalDifferentialTests")]
    public worksheetRegionTypeRenderingElementimagemap() { }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingElementimagemapMechanicalDifferentialTests")]
    private worksheetRegionTypeRenderingElementimagemapBox[] boxField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingElementimagemapMechanicalDifferentialTests")]
    public worksheetRegionTypeRenderingElementimagemapBox[] box { get => boxField; set => boxField = value; }
}
