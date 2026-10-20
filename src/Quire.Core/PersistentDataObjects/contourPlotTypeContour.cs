#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class contourPlotTypeContour
{
    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private string colorField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public string color { get => colorField; set => colorField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private traceSymbolType symbolField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public traceSymbolType symbol { get => symbolField; set => symbolField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private double symbolweightField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public double symbolweight { get => symbolweightField; set => symbolweightField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private uint symbolfrequencyField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public uint symbolfrequency { get => symbolfrequencyField; set => symbolfrequencyField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private traceLineStyle linestyleField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public traceLineStyle linestyle { get => linestyleField; set => linestyleField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private double lineweightField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public double lineweight { get => lineweightField; set => lineweightField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private bool automaticStyleField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public bool automaticStyle { get => automaticStyleField; set => automaticStyleField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private colorSchemeType colorschemeField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public colorSchemeType colorscheme { get => colorschemeField; set => colorschemeField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private bool showlablesField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public bool showlables { get => showlablesField; set => showlablesField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private int resultRefField;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public int resultRef { get => resultRefField; set => resultRefField = value; }

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    private bool resultRefFieldSpecified;

    [MirrorVerified("PtcPersistentDataObjectscontourPlotTypeContourMechanicalDifferentialTests")]
    public bool resultRefSpecified { get => resultRefFieldSpecified; set => resultRefFieldSpecified = value; }

    [MirrorVerified("Release IL")]
    public contourPlotTypeContour()
    {
        symbolField = traceSymbolType.none;
        symbolweightField = 1.0;
        symbolfrequencyField = 1u;
        linestyleField = traceLineStyle.Solid;
        lineweightField = 1.0;
        automaticStyleField = false;
        colorschemeField = colorSchemeType.rainbow;
        showlablesField = true;
    }
}
