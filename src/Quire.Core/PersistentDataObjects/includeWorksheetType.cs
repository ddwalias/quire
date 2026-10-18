#nullable disable
using Quire.Mirror;

namespace Quire.PersistentDataObjects;

public partial class includeWorksheetType
{
    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    private includeWorksheetErrorWarningData[] errorField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public includeWorksheetErrorWarningData[] error { get => errorField; set => errorField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    private includeWorksheetErrorWarningData[] warningField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public includeWorksheetErrorWarningData[] warning { get => warningField; set => warningField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    private string hrefField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public string href { get => hrefField; set => hrefField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    private string lastsaveddateField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public string lastsaveddate { get => lastsaveddateField; set => lastsaveddateField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    private bool usedcacheField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public bool usedcache { get => usedcacheField; set => usedcacheField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    private bool disablecalcField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public bool disablecalc { get => disablecalcField; set => disablecalcField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    private string itemidrefField;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public string itemidref { get => itemidrefField; set => itemidrefField = value; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public IEnumerable<string> MC14ConversionLossAnnotations => MC14ConversionLossAnnotationsLocal;

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public List<string> MC14ConversionLossAnnotationsLocal { get; set; }

    [MirrorVerified("PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests")]
    public bool IsCalculationDisabled => disablecalc;
}
