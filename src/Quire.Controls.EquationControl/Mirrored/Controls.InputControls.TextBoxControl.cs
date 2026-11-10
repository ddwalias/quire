using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class TextBoxControl
{
    private bool _contentLoaded;

    public TextBoxControl(IInputControlDataCalculatedValue data)
    {
        DataContext = new TextBoxViewModel(data);
        InitializeComponent();
    }

    public IInputControlViewModel ViewModel => (TextBoxViewModel)DataContext;

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseRightButtonDown(e);
        e.Handled = true;
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        ((TextBoxViewModel)ViewModel).OnLostFocus();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptabletextbox/textboxcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
