#nullable disable
#pragma warning disable

namespace Quire;

public partial class FunctionBrowserWindow
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/functionbrowserwindow.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            FunctionBrowser = (global::Quire.FunctionBrowser)target;
            return;
        }
        _contentLoaded = true;
    }
}
