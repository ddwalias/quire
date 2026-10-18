#nullable disable
using System.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls.Help;

public partial class HelpManager
{
    [MirrorVerified("PtcControlsHelpHelpManagerMechanicalDifferentialTests")]
    public string ActiveToolTipUid { get; set; }

    [MirrorVerified("PtcControlsHelpHelpManagerMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
