#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public abstract partial class PasswordDialog : Window, INotifyPropertyChanged
{
    public partial class Properties
    {
        public const string ShowError = "ShowError";

        public const string AreaBorder = "AreaBorder";

        [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
        public Properties()
        {
        }
    }

    private bool _showError;

    public bool ShowError
    {
        get
        {
            return _showError;
        }
        protected set
        {
            if (_showError == value)
            {
                return;
            }
            _showError = value;
            FirePropertyChanged("ShowError");
        }
    }

    protected abstract bool DoPasswordsMatch { get; }

    [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    protected void FirePropertyChanged(string propName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    protected void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
    protected void OnPasswordChanged(object sender, RoutedEventArgs e) => ShowError = false;

    protected virtual void OnAcceptPasswordButtonClick(object sender, RoutedEventArgs e)
    {
        ShowError = !DoPasswordsMatch;
        if (!ShowError)
        {
            DialogResult = true;
            Close();
        }
    }

    [MirrorVerified("PtcControlsCoreProtectionPasswordDialogMechanicalDifferentialTests")]
    protected PasswordDialog()
    {
    }
}
