#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class regionInTextBase
{
    [MirrorVerified("PtcPersistentDataObjectsregionInTextBaseMechanicalDifferentialTests")]
    protected regionInTextBase() { }

    [MirrorVerified("PtcPersistentDataObjectsregionInTextBaseMechanicalDifferentialTests")]
    private string textcaretpositionField;

    [MirrorVerified("PtcPersistentDataObjectsregionInTextBaseMechanicalDifferentialTests")]
    public string textcaretposition { get => textcaretpositionField; set => textcaretpositionField = value; }
}
