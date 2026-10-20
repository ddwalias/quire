#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatGeneralType
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private radixType radixField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public radixType radix { get => radixField; set => radixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private bool showtrailingzerosField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public bool showtrailingzeros { get => showtrailingzerosField; set => showtrailingzerosField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private bool useengineeringformatField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public bool useengineeringformat { get => useengineeringformatField; set => useengineeringformatField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private string exponentialthresholdField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public string exponentialthreshold { get => exponentialthresholdField; set => exponentialthresholdField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private resultFormatGeneralTypeImaginaryvalue imaginaryvalueField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public resultFormatGeneralTypeImaginaryvalue imaginaryvalue { get => imaginaryvalueField; set => imaginaryvalueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private string precisionField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public string precision { get => precisionField; set => precisionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private string complexthresholdField = "10";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public string complexthreshold { get => complexthresholdField; set => complexthresholdField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    private string zerothresholdField = "15";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatGeneralTypeMechanicalDifferentialTests")]
    public string zerothreshold { get => zerothresholdField; set => zerothresholdField = value; }

    [MirrorVerified("Release IL")]
    public resultFormatGeneralType()
    {
        radixField = (radixType)0;
        showtrailingzerosField = false;
        useengineeringformatField = false;
        exponentialthresholdField = "3";
    }
}
