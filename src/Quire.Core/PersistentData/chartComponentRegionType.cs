#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class chartComponentRegionType
{
    [MirrorVerified("PtcPersistentDatachartComponentRegionTypeMechanicalDifferentialTests")]
    public chartComponentRegionType() { }

    [MirrorVerified("PtcPersistentDatachartComponentRegionTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDatachartComponentRegionTypeMechanicalDifferentialTests")]
    public override object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDatachartComponentRegionTypeMechanicalDifferentialTests")]
    public override string[] mc14conversionlossannotations { get; set; }
}
