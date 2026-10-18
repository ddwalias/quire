#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class resultFormatType
{
    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    private bool numericonlyField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    public resultFormatType()
    {
        numericonlyField = true;
    }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    private object itemField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    public object Item { get => itemField; set => itemField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    private matrixFormatType matrixField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    public matrixFormatType matrix { get => matrixField; set => matrixField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    private resultFormatTypeUnit unitField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    public resultFormatTypeUnit unit { get => unitField; set => unitField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    private resultFormatTypeTable tableField;

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    public resultFormatTypeTable table { get => tableField; set => tableField = value; }

    [MirrorVerified("PtcPersistentDataObjectsresultFormatTypeMechanicalDifferentialTests")]
    public bool numericonly { get => numericonlyField; set => numericonlyField = value; }
}
