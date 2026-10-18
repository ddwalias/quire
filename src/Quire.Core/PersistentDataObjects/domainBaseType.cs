#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class domainBaseType
{
    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    private bool allownegativevaluesField;

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public domainBaseType()
    {
        allownegativevaluesField = true;
    }

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    private valueType startValueField;

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public valueType startValue { get => startValueField; set => startValueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    private valueType secondTickValueField;

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public valueType secondTickValue { get => secondTickValueField; set => secondTickValueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    private valueType endValueField;

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public valueType endValue { get => endValueField; set => endValueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    private scaleType scaletypeField;

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public scaleType scaletype { get => scaletypeField; set => scaletypeField = value; }

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public bool allownegativevalues { get => allownegativevaluesField; set => allownegativevaluesField = value; }

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    private bool allownegativevaluesFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public bool allownegativevaluesSpecified { get => allownegativevaluesFieldSpecified; set => allownegativevaluesFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    private bool autoscaleField;

    [MirrorVerified("PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests")]
    public bool autoscale { get => autoscaleField; set => autoscaleField = value; }
}
