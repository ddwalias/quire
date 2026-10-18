#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class chartComponentType
{
    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    public chartComponentType() { }

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    private chartComponentRegionType[] regionsField;

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    public chartComponentRegionType[] regions { get => regionsField; set => regionsField = value; }

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    private oleComponentSectionType inputSectionField;

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    public oleComponentSectionType InputSection { get => inputSectionField; set => inputSectionField = value; }

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    private int resultRefField;

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    public int resultRef { get => resultRefField; set => resultRefField = value; }

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    private bool resultRefFieldSpecified;

    [MirrorVerified("PtcPersistentDatachartComponentTypeMechanicalDifferentialTests")]
    public bool resultRefSpecified { get => resultRefFieldSpecified; set => resultRefFieldSpecified = value; }
}
