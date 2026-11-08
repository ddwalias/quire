#nullable disable
#pragma warning disable
using System;
using System.Windows;
using System.Windows.Markup;
using Quire.Controls.ExcelComponent.Model;

namespace Quire.Controls.ExcelComponent.View;

public partial class ChartComponentErrorsDictionary
{
    public static readonly Type[] KeysOwnerTypes = new Type[1] { typeof(ChartComponentErrors) };

    private bool _contentLoaded;

    public ChartComponentErrorsDictionary()
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
        Uri resourceLocator = new Uri("/Quire.Controls.ExcelComponent;component/view/chartcomponenterrorsdictionary.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
