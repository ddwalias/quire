#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class textEmbeddedRegionType
{
    [MirrorVerified("PtcPersistentDataObjectstextEmbeddedRegionTypeMechanicalDifferentialTests")]
    public textEmbeddedRegionType() { }

    [MirrorVerified("PtcPersistentDataObjectstextEmbeddedRegionTypeMechanicalDifferentialTests")]
    private mathType mathField;

    [MirrorVerified("PtcPersistentDataObjectstextEmbeddedRegionTypeMechanicalDifferentialTests")]
    public mathType math { get => mathField; set => mathField = value; }

    [MirrorVerified("PtcPersistentDataObjectstextEmbeddedRegionTypeMechanicalDifferentialTests")]
    private string[] mc14conversionlossannotationsField;

    [MirrorVerified("PtcPersistentDataObjectstextEmbeddedRegionTypeMechanicalDifferentialTests")]
    public override string[] mc14conversionlossannotations { get => mc14conversionlossannotationsField; set => mc14conversionlossannotationsField = value; }
}
