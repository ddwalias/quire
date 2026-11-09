#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class HyperlinkDialog
{
    public static bool ShowHyperlinkDialog(HyperlinkDialogViewModel hyperlinkViewModel, Window owner)
    {
        if (DebugTools.AssertNotNull(hyperlinkViewModel, null).Failed)
        {
            return false;
        }

        HyperlinkDialog dialog = new(hyperlinkViewModel);
        if (owner != null)
        {
            dialog.Owner = owner;
        }

        bool? result = dialog.ShowDialog();
        return result.GetValueOrDefault();
    }

    private void HyperlinkDialogLoaded()
    {
        base.DialogLoaded();
        InsertDisplayTextBox.ClearValue(FrameworkElement.MaxWidthProperty);
        InsertAddressTextBox.ClearValue(FrameworkElement.MaxWidthProperty);
    }

    protected void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        if (DebugTools.DevAssert(!ViewModel.HasErrors, null).Failed)
        {
            return;
        }

        base.OnOkButtonClick(sender, e);
    }
}
