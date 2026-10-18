#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class regionInWhiteboardBase
{
    [MirrorVerified("PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests")]
    protected regionInWhiteboardBase() { }

    [MirrorVerified("PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests")]
    private double topField;

    [MirrorVerified("PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests")]
    public double top { get => topField; set => topField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests")]
    private double leftField;

    [MirrorVerified("PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests")]
    public double left { get => leftField; set => leftField = value; }

    [MirrorVerified("PtcPersistentDataObjectsregionInWhiteboardBaseMechanicalDifferentialTests")]
    public double MC15TopOffset { get; set; }
}
