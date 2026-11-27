#nullable disable
#pragma warning disable

namespace Quire.Freemium;

public partial class FreemiumAboutBox
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/freemium/freemiumaboutbox.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                benefitTextScrollViewer = (global::System.Windows.Controls.ScrollViewer)target;
                return;
            case 2:
                benefitTextBlock = (global::System.Windows.Controls.TextBlock)target;
                return;
            case 3:
                ((global::System.Windows.Documents.Hyperlink)target).Click += BenefitHyperlinkClick;
                return;
            case 4:
                vCardDisplay = (global::System.Windows.Controls.StackPanel)target;
                return;
            case 5:
                ((global::System.Windows.Controls.Button)target).Click += OnCloseButtonClick;
                return;
        }
        _contentLoaded = true;
    }
}
