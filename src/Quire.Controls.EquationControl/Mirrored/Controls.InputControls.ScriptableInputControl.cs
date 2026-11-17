using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class ScriptableInputControl : IComponentConnector
{
    public static readonly Type TypeOfThis = typeof(ScriptableInputControl);

    public static readonly RoutedEvent ScriptableEditingModeLostFocusEvent = EventManager.RegisterRoutedEvent("ComboBoxEditingModeLostFocus", RoutingStrategy.Bubble, typeof(RoutedEventHandler), TypeOfThis);

    internal Grid gridCompositeControl;

    private bool _contentLoaded;

    public ScriptableInputControl(UIElement compositeChildControl)
    {
        _viewModel = ((IInputScriptableControlBase)compositeChildControl).ViewModel;
        DataContext = _viewModel;
        InitializeComponent();
        gridCompositeControl.Children.Add(compositeChildControl);
    }

    public new void EditingCompleted(bool shouldKeepFocus)
    {
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/base/scriptableinputcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            gridCompositeControl = (Grid)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
