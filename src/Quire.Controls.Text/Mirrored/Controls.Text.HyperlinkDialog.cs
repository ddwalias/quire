#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class HyperlinkDialog
{
    protected void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        if (DebugTools.DevAssert(!ViewModel.HasErrors, null).Failed)
        {
            return;
        }

        base.OnOkButtonClick(sender, e);
    }
}
