#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatFractionType
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    private radixType radixField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    public radixType radix { get => radixField; set => radixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    private bool radixFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    public bool radixSpecified { get => radixFieldSpecified; set => radixFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    private bool usemixednumbersField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    public bool usemixednumbers { get => usemixednumbersField; set => usemixednumbersField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    private resultFormatGeneralTypeImaginaryvalue imaginaryvalueField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    public resultFormatGeneralTypeImaginaryvalue imaginaryvalue { get => imaginaryvalueField; set => imaginaryvalueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    private string precisionField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    public string precision { get => precisionField; set => precisionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    private string complexthresholdField = "10";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    public string complexthreshold { get => complexthresholdField; set => complexthresholdField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    private string zerothresholdField = "15";

    [MirrorVerified("PtcPersistentDataObjectsresultFormatFractionTypeMechanicalDifferentialTests")]
    public string zerothreshold { get => zerothresholdField; set => zerothresholdField = value; }

    [MirrorVerified("Release IL")]
    public resultFormatFractionType()
    {
        radixField = (radixType)0;
        usemixednumbersField = false;
    }
}
