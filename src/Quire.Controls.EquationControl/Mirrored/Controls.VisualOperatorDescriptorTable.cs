using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls;

public partial class VisualOperatorDescriptorTable
{
    private static readonly InitializationInfo _initInfo = new InitializationInfo();
    public static VisualOperatorDescriptorTable Default = new VisualOperatorDescriptorTable();
    private bool _contentLoaded;

    public VisualOperatorDescriptorTable()
    {
        InitializeComponent();
        PostInitializeComponent(_initInfo);
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/visualoperatordescriptortable.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
