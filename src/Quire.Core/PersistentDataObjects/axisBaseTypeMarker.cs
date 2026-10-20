#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class axisBaseTypeMarker
{
    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    private bool displayField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    public bool display { get => displayField; set => displayField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    private string colorField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    public string color { get => colorField; set => colorField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    private traceLineStyle linestyleField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    public traceLineStyle linestyle { get => linestyleField; set => linestyleField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    private double lineweightField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    public double lineweight { get => lineweightField; set => lineweightField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    private double valueField;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    public double value { get => valueField; set => valueField = value; }

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    private bool valueFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectsaxisBaseTypeMarkerMechanicalDifferentialTests")]
    public bool valueSpecified { get => valueFieldSpecified; set => valueFieldSpecified = value; }

    [MirrorVerified("Release IL")]
    public axisBaseTypeMarker()
    {
        displayField = false;
        linestyleField = traceLineStyle.Solid;
        lineweightField = 1.0;
    }
}
