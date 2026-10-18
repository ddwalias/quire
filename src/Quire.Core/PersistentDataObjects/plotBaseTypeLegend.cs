#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class plotBaseTypeLegend
{
    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    private textType[] legendlabelitemsField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    public textType[] legendlabelitems { get => legendlabelitemsField; set => legendlabelitemsField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    private plotBaseTypeLegendPosition positionField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    public plotBaseTypeLegendPosition position { get => positionField; set => positionField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    private bool showborderField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    public bool showborder { get => showborderField; set => showborderField = value; }

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    private double legendWidthField;

    [MirrorVerified("PtcPersistentDataObjectsplotBaseTypeLegendMechanicalDifferentialTests")]
    public double legendWidth { get => legendWidthField; set => legendWidthField = value; }
}
