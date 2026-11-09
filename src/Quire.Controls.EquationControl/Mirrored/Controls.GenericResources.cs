using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls;

public partial class GenericResources
{
    public const string MatrixResizeWidgetToolTip = "MatrixResizeWidgetToolTip";

    private bool _contentLoaded;

    public GenericResources()
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
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/genericresources.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
