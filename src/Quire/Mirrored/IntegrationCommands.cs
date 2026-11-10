#nullable disable

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Quire.Wpf;

namespace Quire;

public partial class IntegrationCommands
{
    public static readonly DependencyProperty PTCPlacesEnableProperty = DependencyProperty.Register("PTCPlacesEnable", typeof(bool), typeof(IntegrationCommands), new PropertyMetadata((object)false, (PropertyChangedCallback)PtcPlacesEnableChanged));

    private static readonly IntegrationCommands _instance = new IntegrationCommands();

    public static readonly RoutedUICommand AutoCheckIn = new RoutedUICommand();

    public static readonly RoutedUICommand CustomCheckIn = new RoutedUICommand();

    public static readonly RoutedUICommand OpenFromWC = new RoutedUICommand(AddonsProperties.cmdTextOpenFromWC, "OpenFromWC", typeof(SpiritMainWindow));

    public static readonly RoutedUICommand CheckOut = new RoutedUICommand();

    public static readonly RoutedUICommand UndoCheckOut = new RoutedUICommand();

    public static readonly RoutedUICommand WorkgroupManager = new RoutedUICommand();

    public static readonly RoutedUICommand ServerManagerWC = new RoutedUICommand();

    public static readonly RoutedUICommand EventManager = new RoutedUICommand();

    public static readonly RoutedUICommand WorkgroupManagerHelp = new RoutedUICommand();

    public bool PTCPlacesEnable
    {
        get
        {
            return (bool)GetValue(PTCPlacesEnableProperty);
        }
        set
        {
            SetValue(PTCPlacesEnableProperty, value);
        }
    }

    public static IntegrationCommands Instance => _instance;

    public bool WcEnable
    {
        get
        {
            return _wcEnable;
        }
        set
        {
            WpfUtils.SetPropertyAndNotify(ref _wcEnable, value, this, PropertyChanged, "WCEnable");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private IntegrationCommands()
    {
        PTCPlacesEnable = AddonsProperties.GetPtcPlacesConnectionStatus();
        _wcEnable = PTCPlacesEnable;
    }

    private static void PtcPlacesEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IntegrationCommands && SpiritMainWindow.Addons != null)
        {
            SpiritMainWindow.Addons.CallAddonsToEnablePtcPlaces((bool)e.NewValue);
        }
    }
}
