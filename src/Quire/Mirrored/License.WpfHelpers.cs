#nullable disable

namespace Quire.License;

public partial class LicenseError
{
    public LicenseError()
    {
        InitializeComponent();
        SetBinding(
            global::System.Windows.FrameworkElement.MinWidthProperty,
            new global::System.Windows.Data.Binding
            {
                Source = buttonPanel,
                Path = new global::System.Windows.PropertyPath(global::System.Windows.FrameworkElement.ActualWidthProperty)
            });
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        global::System.Uri resourceLocator = new(
            "/Quire;component/license/licenseerror.xaml",
            global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            buttonPanel = (global::System.Windows.Controls.StackPanel)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}

public partial class LicenseMessageDictionary
{
    public LicenseMessageDictionary()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        global::System.Uri resourceLocator = new(
            "/Quire;component/license/licensemessagedictionary.xaml",
            global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
