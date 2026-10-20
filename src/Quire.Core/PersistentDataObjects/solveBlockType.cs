#nullable disable
using Quire.Mirror;
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

    [MirrorVerified("Release IL")]
    public override IEnumerable<IRegionType> Regions { get => regions; set => regions = Enumerable.ToArray<solveBlockRegionType>(Enumerable.Cast<solveBlockRegionType>(value)); }
}
