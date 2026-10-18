#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class unitedValue
{
    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    public unitedValue() { }

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    private real realField;

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    public real real { get => realField; set => realField = value; }

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    private imag imagField;

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    public imag imag { get => imagField; set => imagField = value; }

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    private complex complexField;

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    public complex complex { get => complexField; set => complexField = value; }

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    private str strField;

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    public str str { get => strField; set => strField = value; }

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    private unitMonomial unitMonomialField;

    [MirrorVerified("PtcPersistentDataObjectsunitedValueMechanicalDifferentialTests")]
    public unitMonomial unitMonomial { get => unitMonomialField; set => unitMonomialField = value; }
}
