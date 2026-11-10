#nullable disable

using System;
using System.Windows;
using Quire.Wpf;

namespace Quire.MessageBoxes;

public partial class DialogManager
{
    public System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.FileDialog fileDialog)
    {
        System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.Cancel;
        try
        {
            IsDialogOpen = true;
            result = fileDialog.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + " : " + fileDialog.InitialDirectory, WpfUtils.ApplicationFullName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        IsDialogOpen = false;
        return result;
    }

    public bool? ShowDialog(global::Microsoft.Win32.FileDialog fileDialog)
    {
        bool? result = null;
        try
        {
            IsDialogOpen = true;
            result = fileDialog.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show(((ex is UnauthorizedAccessException) ? (ResourceProvider<DialogManagerResourceDictionary>.Instance["AccessDeniedToBrowsePath"] as string) : (ex.Message + " :")) + " " + fileDialog.InitialDirectory, WpfUtils.ApplicationFullName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        IsDialogOpen = false;
        return result;
    }

    public bool IsConnected { get; set; }

    public bool IsDialogOpen { get; set; }

    public DialogManager()
    {
    }
}
