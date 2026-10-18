#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class AxisInfoType
{
    [MirrorVerified("PtcPersistentDataObjectsAxisInfoTypeMechanicalDifferentialTests")]
    public AxisInfoType() { }

    [MirrorVerified("PtcPersistentDataObjectsAxisInfoTypeMechanicalDifferentialTests")]
    private unitMonomial unitMonomialField;

    [MirrorVerified("PtcPersistentDataObjectsAxisInfoTypeMechanicalDifferentialTests")]
    public unitMonomial unitMonomial { get => unitMonomialField; set => unitMonomialField = value; }

    [MirrorVerified("PtcPersistentDataObjectsAxisInfoTypeMechanicalDifferentialTests")]
    private double maxField;

    [MirrorVerified("PtcPersistentDataObjectsAxisInfoTypeMechanicalDifferentialTests")]
    public double Max { get => maxField; set => maxField = value; }

    [MirrorVerified("PtcPersistentDataObjectsAxisInfoTypeMechanicalDifferentialTests")]
    private double minField;

    [MirrorVerified("PtcPersistentDataObjectsAxisInfoTypeMechanicalDifferentialTests")]
    public double Min { get => minField; set => minField = value; }
}
