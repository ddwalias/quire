#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class ProtectDialog
{
    [MirrorVerified("PtcControlsCoreProtectionProtectDialogMechanicalDifferentialTests")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    partial class ProtectProperties
    {
        [MirrorVerified("PtcControlsCoreProtectionProtectDialogMechanicalDifferentialTests")]
        public ProtectProperties() { }
    }
}
