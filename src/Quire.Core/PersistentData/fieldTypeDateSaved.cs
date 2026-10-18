#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class fieldTypeDateSaved
{
    [MirrorVerified("PtcPersistentDatafieldTypeDateSavedMechanicalDifferentialTests")]
    public fieldTypeDateSaved() { }

    [MirrorVerified("PtcPersistentDatafieldTypeDateSavedMechanicalDifferentialTests")]
    private dateSavedTemplate templateField;

    [MirrorVerified("PtcPersistentDatafieldTypeDateSavedMechanicalDifferentialTests")]
    public dateSavedTemplate template { get => templateField; set => templateField = value; }

    [MirrorVerified("PtcPersistentDatafieldTypeDateSavedMechanicalDifferentialTests")]
    private bool templateFieldSpecified;

    [MirrorVerified("PtcPersistentDatafieldTypeDateSavedMechanicalDifferentialTests")]
    public bool templateSpecified { get => templateFieldSpecified; set => templateFieldSpecified = value; }
}
