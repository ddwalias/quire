#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatEngineeringType
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private radixType radixField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public radixType radix { get => radixField; set => radixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private bool radixFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public bool radixSpecified { get => radixFieldSpecified; set => radixFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private bool showtrailingzerosField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public bool showtrailingzeros { get => showtrailingzerosField; set => showtrailingzerosField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private bool useenotationField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public bool useenotation { get => useenotationField; set => useenotationField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private resultFormatGeneralTypeImaginaryvalue imaginaryvalueField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public resultFormatGeneralTypeImaginaryvalue imaginaryvalue { get => imaginaryvalueField; set => imaginaryvalueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private string precisionField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public string precision { get => precisionField; set => precisionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private string complexthresholdField = "10";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public string complexthreshold { get => complexthresholdField; set => complexthresholdField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    private string zerothresholdField = "15";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatEngineeringTypeMechanicalDifferentialTests")]
    public string zerothreshold { get => zerothresholdField; set => zerothresholdField = value; }

    [MirrorVerified("Release IL")]
    public resultFormatEngineeringType()
    {
        radixField = (radixType)0;
        showtrailingzerosField = false;
        useenotationField = false;
    }
}
