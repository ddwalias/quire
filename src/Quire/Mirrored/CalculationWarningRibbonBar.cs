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
}
