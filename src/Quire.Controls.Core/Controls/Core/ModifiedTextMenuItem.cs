#nullable disable
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ModifiedTextMenuItem
{
    [MirrorVerified("PtcControlsCoreModifiedTextMenuItemMechanicalDifferentialTests")]
    public ModifiedTextMenuItem() { }

    [MirrorVerified("PtcControlsCoreModifiedTextMenuItemMechanicalDifferentialTests")]
    public RoutedUICommand Command { get; set; }

    [MirrorVerified("PtcControlsCoreModifiedTextMenuItemMechanicalDifferentialTests")]
    public string Header { get; set; }
}
