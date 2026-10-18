#nullable disable
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.PersistentData;

public partial class worksheetRegionTypeSpectable
{
    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    private bool disablecalcField;

    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    public worksheetRegionTypeSpectable()
    {
        disablecalcField = false;
    }

    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    public IEnumerable<string> MC14ConversionLossAnnotations => MC14ConversionLossAnnotationsLocal;

    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    public List<string> MC14ConversionLossAnnotationsLocal { get; set; }

    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    public bool IsCalculationDisabled => disablecalc;

    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    private mathType[] mathField;

    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    public mathType[] math { get => mathField; set => mathField = value; }

    [MirrorVerified("PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests")]
    public bool disablecalc { get => disablecalcField; set => disablecalcField = value; }
}
