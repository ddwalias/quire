#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatPercentType
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    private radixType radixField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    public radixType radix { get => radixField; set => radixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    private bool radixFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    public bool radixSpecified { get => radixFieldSpecified; set => radixFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    private bool showtrailingzerosField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    public bool showtrailingzeros { get => showtrailingzerosField; set => showtrailingzerosField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    private resultFormatGeneralTypeImaginaryvalue imaginaryvalueField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    public resultFormatGeneralTypeImaginaryvalue imaginaryvalue { get => imaginaryvalueField; set => imaginaryvalueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    private string precisionField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    public string precision { get => precisionField; set => precisionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    private string complexthresholdField = "10";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    public string complexthreshold { get => complexthresholdField; set => complexthresholdField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    private string zerothresholdField = "15";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatPercentTypeMechanicalDifferentialTests")]
    public string zerothreshold { get => zerothresholdField; set => zerothresholdField = value; }

    [MirrorVerified("Release IL")]
    public resultFormatPercentType()
    {
        radixField = (radixType)0;
        showtrailingzerosField = false;
    }
}
