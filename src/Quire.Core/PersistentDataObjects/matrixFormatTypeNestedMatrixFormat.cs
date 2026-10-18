#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class matrixFormatTypeNestedMatrixFormat
{
    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeNestedMatrixFormatMechanicalDifferentialTests")]
    public matrixFormatTypeNestedMatrixFormat() { }

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeNestedMatrixFormatMechanicalDifferentialTests")]
    private string positionField;

    [MirrorVerified("PtcPersistentDataObjectsmatrixFormatTypeNestedMatrixFormatMechanicalDifferentialTests")]
    public string position { get => positionField; set => positionField = value; }
}
