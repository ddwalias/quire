using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls;

public partial class EquationControlErrorMessages
{
    public const string MaxRecursionDepthExceeded = "EquationControl.Error.XmlParsing.MaxRecursionDepthExceeded";

    private bool _contentLoaded;

    public EquationControlErrorMessages()
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
        Uri resourceLocator = new Uri(
            "/Quire.Controls.EquationControl;component/equationcontrolerrormessages.xaml",
            UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
