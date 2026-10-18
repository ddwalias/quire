#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class UIElementFacade
{
    [MirrorVerified("PtcControlsCoreUIElementFacadeMechanicalDifferentialTests")]
    public UIElementFacade() { }

    [MirrorVerified("PtcControlsCoreUIElementFacadeMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }
}
