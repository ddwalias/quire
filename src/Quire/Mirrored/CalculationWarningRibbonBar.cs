using System.Windows.Controls;
using Quire.Controls.Core;

namespace Quire;

public partial class CalculationWarningRibbonBar
{
    internal Panel SystemRedefinitionWarningsGroup;
    internal Panel UserRedefinitionWarningsGroup;

    public WarningsTypesToShow WarningsTypesFilter
    {
        get
        {
            return (WarningsTypesToShow)GetValue(WarningsTypesFilterProperty);
        }
        set
        {
            SetValue(WarningsTypesFilterProperty, value);
        }
    }

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/calculationwarningribbonbar.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

}
