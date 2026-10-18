#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class worksheetRegionTypeIndexes
{
    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeIndexesMechanicalDifferentialTests")]
    public worksheetRegionTypeIndexes() { }

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeIndexesMechanicalDifferentialTests")]
    private string[] phraseField;

    [MirrorVerified("PtcPersistentDataObjectsworksheetRegionTypeIndexesMechanicalDifferentialTests")]
    public string[] phrase { get => phraseField; set => phraseField = value; }
}
