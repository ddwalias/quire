#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class worksheetRegionTypeRendering
{
    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingMechanicalDifferentialTests")]
    public worksheetRegionTypeRendering() { }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingMechanicalDifferentialTests")]
    private worksheetRegionTypeRenderingElementimagemap elementimagemapField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingMechanicalDifferentialTests")]
    public worksheetRegionTypeRenderingElementimagemap elementimagemap { get => elementimagemapField; set => elementimagemapField = value; }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingMechanicalDifferentialTests")]
    private string itemidrefField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeRenderingMechanicalDifferentialTests")]
    public string itemidref { get => itemidrefField; set => itemidrefField = value; }
}
