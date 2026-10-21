#nullable disable
using System.ComponentModel;
using System.ComponentModel;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class CheckBoxData
{
    [MirrorVerified("PtcWpfCheckBoxDataMechanicalDifferentialTests")]
    public string Message { get; private set; }

    [MirrorVerified("PtcWpfCheckBoxDataMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value)
            {
                return;
            }
            _isChecked = value;
            FirePropertyChanged("IsChecked");
        }
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [MirrorVerified("Release IL")]
    public CheckBoxData(string message, bool isChecked)
    {
        Message = message;
        IsChecked = isChecked;
    }
}
