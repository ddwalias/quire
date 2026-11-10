#nullable disable

using System.ComponentModel;
using System.Windows;
using Quire.Wpf;

namespace Quire;

public partial class SpiritOptions
{
    private static readonly SpiritOptions _instance = new SpiritOptions();

    public const string EnableGettingStartedTabPropertyName = "EnableGettingStartedTab";

    public static SpiritOptions Instance => _instance;

    public bool EnableGettingStartedTab
    {
        get
        {
            return _EnableGettingStartedTab;
        }
        set
        {
            WpfUtils.SetPropertyAndNotify(ref _EnableGettingStartedTab, value, this, PropertyChanged, "EnableGettingStartedTab");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SpiritOptions()
    {
        _EnableGettingStartedTab = true;
    }
}
