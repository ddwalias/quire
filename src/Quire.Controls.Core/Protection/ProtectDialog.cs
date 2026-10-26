#nullable disable
using System;
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class ProtectDialog
{
    public DisplayStateEnum DisplayState
    {
        get
        {
            return _displayState;
        }
        set
        {
            if (value == _displayState)
            {
                return;
            }
            _displayState = value;
            FirePropertyChanged("DisplayState");
        }
    }

    public AreaBorderStateEnum AreaBorderState
    {
        get
        {
            return _areaBorderState;
        }
        set
        {
            if (value == _areaBorderState)
            {
                return;
            }
            _areaBorderState = value;
            FirePropertyChanged("AreaBorder");
        }
    }

    protected override bool DoPasswordsMatch => _primaryPasswordBox.Password == _reenterPasswordBox.Password;

    [MirrorVerified("PtcControlsCoreProtectionProtectDialogMechanicalDifferentialTests")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    partial class ProtectProperties
    {
        public const string DisplayState = "DisplayState";

        [MirrorVerified("PtcControlsCoreProtectionProtectDialogMechanicalDifferentialTests")]
        public ProtectProperties() { }
    }
}
