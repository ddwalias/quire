#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class UnprotectDialog
{
    [MirrorVerified("PtcControlsCoreProtectionUnprotectDialogMechanicalDifferentialTests")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    [MirrorVerified("PtcControlsCoreProtectionUnprotectDialogMechanicalDifferentialTests")]
    private readonly bool _hasPassword;

    [MirrorVerified("PtcControlsCoreProtectionUnprotectDialogMechanicalDifferentialTests")]
    public bool HasPassword => _hasPassword;
}
