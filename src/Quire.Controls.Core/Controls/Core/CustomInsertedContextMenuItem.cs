#nullable disable
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class CustomInsertedContextMenuItem
{
    [MirrorVerified("PtcControlsCoreCustomInsertedContextMenuItemMechanicalDifferentialTests")]
    public CustomInsertedContextMenuItem() { }

    [MirrorVerified("PtcControlsCoreCustomInsertedContextMenuItemMechanicalDifferentialTests")]
    public RoutedUICommand NextItemCommand { get; set; }
}
