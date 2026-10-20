#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatScientificType
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private radixType radixField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public radixType radix { get => radixField; set => radixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private bool radixFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public bool radixSpecified { get => radixFieldSpecified; set => radixFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private bool showtrailingzerosField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public bool showtrailingzeros { get => showtrailingzerosField; set => showtrailingzerosField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private bool useenotationField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public bool useenotation { get => useenotationField; set => useenotationField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private resultFormatGeneralTypeImaginaryvalue imaginaryvalueField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public resultFormatGeneralTypeImaginaryvalue imaginaryvalue { get => imaginaryvalueField; set => imaginaryvalueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private string precisionField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public string precision { get => precisionField; set => precisionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private string complexthresholdField = "10";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public string complexthreshold { get => complexthresholdField; set => complexthresholdField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    private string zerothresholdField = "15";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatScientificTypeMechanicalDifferentialTests")]
    public string zerothreshold { get => zerothresholdField; set => zerothresholdField = value; }

    [MirrorVerified("Release IL")]
    public resultFormatScientificType()
    {
        radixField = (radixType)0;
        showtrailingzerosField = false;
        useenotationField = false;
    }
}
