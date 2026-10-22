#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface IDialogManager
{
    bool IsDialogOpen { get; set; }
    global::System.Windows.Forms.DialogResult ShowDialog(global::System.Windows.Forms.FileDialog fileDialog) ;
    global::System.Nullable<bool> ShowDialog(global::Microsoft.Win32.FileDialog fileDialog) ;
}
