using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls;

public partial class VisualModifierDescriptorTable
{
    private static readonly InitializationInfo _initInfo = new InitializationInfo();
    public static VisualModifierDescriptorTable Default = new VisualModifierDescriptorTable();
    private bool _contentLoaded;

    public VisualModifierDescriptorTable()
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
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/visualmodifierdescriptortable.xaml", UriKind.Relative);
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
