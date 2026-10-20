#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.UI;

public partial class ScrollableMessageBox
{
    [MirrorVerified("PtcUIScrollableMessageBoxMechanicalDifferentialTests")]
    private void OkButton_Click(object sender, RoutedEventArgs e) => base.Close();

    [MirrorVerified("Release IL")]
    public ScrollableMessageBox()
    {
        InitializeComponent();
    }

    [MirrorVerified("Release IL")]
    internal TextBox MessageContainer;

    [MirrorVerified("Release IL")]
    public ScrollableMessageBox(string message, string caption)
    {
        InitializeComponent();
        MessageContainer.Text = message;
        base.Title = caption;
    }
}
