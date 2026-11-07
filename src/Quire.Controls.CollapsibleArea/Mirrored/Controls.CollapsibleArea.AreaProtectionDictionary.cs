#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.CollapsibleArea;

public partial class AreaProtectionDictionary
{
    private bool _contentLoaded;

    public AreaProtectionDictionary()
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
        Uri resourceLocater = new Uri("/Quire.Controls.CollapsibleArea;component/areaprotectiondictionary.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocater);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
