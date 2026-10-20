#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatDecimalType
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    private radixType radixField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    public radixType radix { get => radixField; set => radixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    private bool showtrailingzerosField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    public bool showtrailingzeros { get => showtrailingzerosField; set => showtrailingzerosField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    private resultFormatGeneralTypeImaginaryvalue imaginaryvalueField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    public resultFormatGeneralTypeImaginaryvalue imaginaryvalue { get => imaginaryvalueField; set => imaginaryvalueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    private string precisionField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    public string precision { get => precisionField; set => precisionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    private string complexthresholdField = "10";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    public string complexthreshold { get => complexthresholdField; set => complexthresholdField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    private string zerothresholdField = "15";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatDecimalTypeMechanicalDifferentialTests")]
    public string zerothreshold { get => zerothresholdField; set => zerothresholdField = value; }

    [MirrorVerified("Release IL")]
    public resultFormatDecimalType()
    {
        radixField = (radixType)0;
        showtrailingzerosField = false;
    }
}
