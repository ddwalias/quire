using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls;

public partial class VisualSymbolDescriptorTable
{
    private static readonly InitializationInfo _initInfo = new InitializationInfo();
    public static VisualSymbolDescriptorTable Default = new VisualSymbolDescriptorTable();
    private bool _contentLoaded;

    public VisualSymbolDescriptorTable()
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
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/visualsymboldescriptortable.xaml", UriKind.Relative);
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
