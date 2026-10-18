#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class excelComponentRegionType
{
    [MirrorVerified("PtcPersistentDataexcelComponentRegionTypeMechanicalDifferentialTests")]
    public excelComponentRegionType() { }

    [MirrorVerified("PtcPersistentDataexcelComponentRegionTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDataexcelComponentRegionTypeMechanicalDifferentialTests")]
    public override object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDataexcelComponentRegionTypeMechanicalDifferentialTests")]
    private string[] mc14conversionlossannotationsField;

    [MirrorVerified("PtcPersistentDataexcelComponentRegionTypeMechanicalDifferentialTests")]
    public override string[] mc14conversionlossannotations { get => mc14conversionlossannotationsField; set => mc14conversionlossannotationsField = value; }
}
