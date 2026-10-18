#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisBaseTypeAxisLine
{
    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    private axisBaseTypeAxisLinePosition positionField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    public axisBaseTypeAxisLinePosition position { get => positionField; set => positionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    private string positionticmarkField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    public string positionticmark { get => positionticmarkField; set => positionticmarkField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    private double legendWidthField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    public double legendWidth { get => legendWidthField; set => legendWidthField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    private bool legendWidthFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeAxisLineMechanicalDifferentialTests")]
    public bool legendWidthSpecified { get => legendWidthFieldSpecified; set => legendWidthFieldSpecified = value; }
}
