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

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire.Core;component/ui/scrollablemessagebox.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                MessageContainer = (TextBox)target;
                break;
            case 2:
                okButton = (Button)target;
                okButton.Click += OkButton_Click;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
