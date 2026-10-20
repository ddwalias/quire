#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentDataObjects;

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

    [MirrorVerified("Release IL")]
    public override IEnumerable<IRegionType> Regions { get => regions; set => regions = Enumerable.ToArray<chartComponentRegionType>(Enumerable.Cast<chartComponentRegionType>(value)); }
}
