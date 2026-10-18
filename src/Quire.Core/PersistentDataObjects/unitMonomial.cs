#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class unitMonomial
{
    [MirrorVerified("PtcPersistentDataObjectsunitMonomialMechanicalDifferentialTests")]
    public unitMonomial() { }

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialMechanicalDifferentialTests")]
    private unitMonomialUnitReference[] unitReferenceField;

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialMechanicalDifferentialTests")]
    public unitMonomialUnitReference[] unitReference { get => unitReferenceField; set => unitReferenceField = value; }

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialMechanicalDifferentialTests")]
    private string systemField;

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialMechanicalDifferentialTests")]
    public string system { get => systemField; set => systemField = value; }
}
