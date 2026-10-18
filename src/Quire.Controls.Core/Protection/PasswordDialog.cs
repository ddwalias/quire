#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class PasswordDialog
{
    [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
    protected void OnPasswordChanged(object sender, RoutedEventArgs e) => ShowError = false;

    [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
    protected PasswordDialog() { }

    [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    partial class Properties
    {
        [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
        public Properties() { }
    }
}
