#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class ProtectionDialogState
{
    [MirrorVerified("PtcControlsCoreProtectionProtectionDialogStateMechanicalDifferentialTests")]
    public ProtectionDialogState() { }

    [MirrorVerified("PtcControlsCoreProtectionProtectionDialogStateMechanicalDifferentialTests")]
    public bool IsDisplayStateLocked { get; set; }

    [MirrorVerified("PtcControlsCoreProtectionProtectionDialogStateMechanicalDifferentialTests")]
    public string Password { get; set; }

    [MirrorVerified("PtcControlsCoreProtectionProtectionDialogStateMechanicalDifferentialTests")]
    public bool IsCollapsed { get; set; }

    [MirrorVerified("PtcControlsCoreProtectionProtectionDialogStateMechanicalDifferentialTests")]
    public bool ShowTimeStamp { get; set; }

    [MirrorVerified("PtcControlsCoreProtectionProtectionDialogStateMechanicalDifferentialTests")]
    public bool IsShowAreaBorder { get; set; }
}
