#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class CheckBoxData
{
    [MirrorVerified("PtcWpfCheckBoxDataMechanicalDifferentialTests")]
    public string Message { get; private set; }

    [MirrorVerified("PtcWpfCheckBoxDataMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
