#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class unitMonomialUnitReference
{
    [MirrorVerified("PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests")]
    private string unitField;

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests")]
    public string unit { get => unitField; set => unitField = value; }

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests")]
    private string powernumeratorField;

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests")]
    public string powernumerator { get => powernumeratorField; set => powernumeratorField = value; }

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests")]
    private string powerdenominatorField;

    [MirrorVerified("PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests")]
    public string powerdenominator { get => powerdenominatorField; set => powerdenominatorField = value; }

    [MirrorVerified("Release IL")]
    public unitMonomialUnitReference()
    {
        powernumeratorField = "1";
        powerdenominatorField = "1";
    }
}
