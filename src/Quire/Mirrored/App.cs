using System.Windows;

namespace Quire;

public partial class App
{
    public void InitializeComponent()
    {
        if (_contentLoaded)
            return;
        _contentLoaded = true;
        Startup += PrimeStartup;
        var resourceLocator = new Uri("/Quire;component/app.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [STAThread]
    public static void Main()
    {
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
