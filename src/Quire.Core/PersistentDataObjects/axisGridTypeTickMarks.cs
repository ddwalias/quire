#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisGridTypeTickMarks
{
    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeTickMarksMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeTickMarksMechanicalDifferentialTests")]
    public axisGridTypeTickMarks()
    {
        displayField = false;
    }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeTickMarksMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }
}
