using System;
using System.Windows.Navigation;

namespace Quire;

public partial class AboutWindow
{
    private readonly global::Quire.SpiritMainWindow _spiritMainWindow;
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
}
