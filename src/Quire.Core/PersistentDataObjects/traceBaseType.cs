#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class traceBaseType
{
    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    public traceBaseType() { }

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    private traceStyleType traceStyleField;

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    public traceStyleType traceStyle { get => traceStyleField; set => traceStyleField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    private string numofpointsField;

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    public string numofpoints { get => numofpointsField; set => numofpointsField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    private int resultRefField;

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    public int resultRef { get => resultRefField; set => resultRefField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    private bool resultRefFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectstraceBaseTypeMechanicalDifferentialTests")]
    public bool resultRefSpecified { get => resultRefFieldSpecified; set => resultRefFieldSpecified = value; }
}
