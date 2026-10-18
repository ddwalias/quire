#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class complex
{
    [MirrorVerified("PtcPersistentDataObjectscomplexMechanicalDifferentialTests")]
    public complex() { }

    [MirrorVerified("PtcPersistentDataObjectscomplexMechanicalDifferentialTests")]
    private real realField;

    [MirrorVerified("PtcPersistentDataObjectscomplexMechanicalDifferentialTests")]
    public real real { get => realField; set => realField = value; }

    [MirrorVerified("PtcPersistentDataObjectscomplexMechanicalDifferentialTests")]
    private imag imagField;

    [MirrorVerified("PtcPersistentDataObjectscomplexMechanicalDifferentialTests")]
    public imag imag { get => imagField; set => imagField = value; }
}
