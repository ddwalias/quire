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

    [MirrorVerified("Release IL")]
    public CheckBoxData(string message, bool isChecked)
    {
        Message = message;
        IsChecked = isChecked;
    }
}
