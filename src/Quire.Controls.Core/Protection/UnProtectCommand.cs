#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class UnProtectCommand
{
    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public UnProtectCommand(IProtectableModel model, Action<string> showWarningDialogRequest) : base(model, showWarningDialogRequest) { }

    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public void Undo() => Protect();

    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public void Redo() => Unprotect();

    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public string Name => "Remove Control Protection";
}
