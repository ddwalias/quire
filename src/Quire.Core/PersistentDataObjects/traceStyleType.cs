#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class traceStyleType
{
    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private string colorField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public string color { get => colorField; set => colorField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private traceSymbolType symbolField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public traceSymbolType symbol { get => symbolField; set => symbolField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private double symbolweightField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public double symbolweight { get => symbolweightField; set => symbolweightField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private uint symbolfrequencyField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public uint symbolfrequency { get => symbolfrequencyField; set => symbolfrequencyField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private traceLineStyle linestyleField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public traceLineStyle linestyle { get => linestyleField; set => linestyleField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private double lineweightField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public double lineweight { get => lineweightField; set => lineweightField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private bool automaticStyleField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public bool automaticStyle { get => automaticStyleField; set => automaticStyleField = value; }

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    private traceStyleEnumCommonType valueField;

    [MirrorVerified("PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests")]
    public traceStyleEnumCommonType Value { get => valueField; set => valueField = value; }

    [MirrorVerified("Release IL")]
    public traceStyleType()
    {
        symbolField = traceSymbolType.none;
        symbolweightField = 1.0;
        symbolfrequencyField = 1u;
        linestyleField = traceLineStyle.Solid;
        lineweightField = 1.0;
        automaticStyleField = false;
    }
}
