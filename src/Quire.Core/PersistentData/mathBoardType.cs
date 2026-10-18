#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class mathBoardType
{
    [MirrorVerified("PtcPersistentDatamathBoardTypeMechanicalDifferentialTests")]
    protected mathBoardType() { }

    [MirrorVerified("PtcPersistentDatamathBoardTypeMechanicalDifferentialTests")]
    private bool disablecalcField;

    [MirrorVerified("PtcPersistentDatamathBoardTypeMechanicalDifferentialTests")]
    public bool disablecalc { get => disablecalcField; set => disablecalcField = value; }

    [MirrorVerified("PtcPersistentDatamathBoardTypeMechanicalDifferentialTests")]
    public bool IsCalculationDisabled { get => disablecalc; set => disablecalc = value; }
}
