#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.UI;

public partial class ScrollableMessageBox
{
    [MirrorVerified("PtcUIScrollableMessageBoxMechanicalDifferentialTests")]
    private void OkButton_Click(object sender, RoutedEventArgs e) => base.Close();
}
