#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisGridTypeGridLabels
{
    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridLabelsMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridLabelsMechanicalDifferentialTests")]
    public axisGridTypeGridLabels()
    {
        displayField = false;
    }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridLabelsMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }
}
