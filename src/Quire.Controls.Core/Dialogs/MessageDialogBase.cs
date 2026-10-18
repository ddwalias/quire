#nullable disable
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.Controls.Core.Dialogs;

public partial class MessageDialogBase<T>
{
    [MirrorVerified("PtcControlsCoreDialogsMessageDialogBase1MechanicalDifferentialTests")]
    private Button CancelButton { get; set; }

    [MirrorVerified("PtcControlsCoreDialogsMessageDialogBase1MechanicalDifferentialTests")]
    protected Nullable<T> Result { get; set; }
}
