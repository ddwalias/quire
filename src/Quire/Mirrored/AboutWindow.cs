using System;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace Quire;

public partial class AboutWindow
{
    private readonly global::Quire.SpiritMainWindow _spiritMainWindow;

    public AboutWindow(global::Quire.SpiritMainWindow owner)
    {
        _spiritMainWindow = owner;
        InitializeComponent();
        Owner = owner;
    }
    public string EngineVersion => _spiritMainWindow?.GetEngineVersion();
    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            global::Quire.AppCommands.DisplayHyperlink.Execute(e.Uri.OriginalString, Owner);
        }
        catch (Exception ex)
        {
            global::Quire.DebugTools.FailAssertion(ex);
        }
    }
    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/aboutwindow.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _this = (global::Quire.AboutWindow)target;
                return;
            case 2:
                ((global::System.Windows.Documents.Hyperlink)target).RequestNavigate += Hyperlink_RequestNavigate;
                return;
        }
        _contentLoaded = true;
    }
}
