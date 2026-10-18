#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class solveBlockType
{
    [MirrorVerified("PtcPersistentDataObjectssolveBlockTypeMechanicalDifferentialTests")]
    public solveBlockType() { }

    [MirrorVerified("PtcPersistentDataObjectssolveBlockTypeMechanicalDifferentialTests")]
    private solveBlockRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDataObjectssolveBlockTypeMechanicalDifferentialTests")]
    public solveBlockRegionType[] regions { get => regionsField; set => regionsField = value; }
}
