#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisGridTypeGridLines
{
    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridLinesMechanicalDifferentialTests")]
    private string colorField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridLinesMechanicalDifferentialTests")]
    public string color { get => colorField; set => colorField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridLinesMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsaxisGridTypeGridLinesMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }
}
