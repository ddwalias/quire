#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisGridTypeGridFrequency
{
    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridFrequencyMechanicalDifferentialTests")]
    private bool autogridField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridFrequencyMechanicalDifferentialTests")]
    public axisGridTypeGridFrequency()
    {
        autogridField = true;
    }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridFrequencyMechanicalDifferentialTests")]
    public bool autogrid { get => autogridField; set => autogridField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridFrequencyMechanicalDifferentialTests")]
    private string valueField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridFrequencyMechanicalDifferentialTests")]
    public string Value { get => valueField; set => valueField = value; }
}
