#nullable disable
using System;
using Quire.Mirror;
using Quire.Undo;

namespace Quire.Controls.Core.Protection;

public partial class UnProtectCommand : ProtectionCommonBase, IUndoableCommand
{
    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public UnProtectCommand(IProtectableModel model, Action<string> showWarningDialogRequest) : base(model, showWarningDialogRequest) { }

    public bool Do()
    {
        _protectionData = _model.ProtectionData;
        Unprotect();
        return true;
    }

    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public void Undo() => Protect();

    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public void Redo() => Unprotect();

    [MirrorVerified("PtcControlsCoreProtectionUnProtectCommandMechanicalDifferentialTests")]
    public string Name => "Remove Control Protection";
}
