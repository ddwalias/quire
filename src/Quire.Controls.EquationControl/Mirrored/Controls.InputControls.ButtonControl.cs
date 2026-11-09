using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class ButtonControl
{
    private bool _contentLoaded;

    public ButtonControl(IInputControlDataCalculatedValue data)
    {
        DataContext = new ButtonViewModel(data);
        InitializeComponent();
    }

    public IInputControlViewModel ViewModel => (ButtonViewModel)DataContext;

    private void ButtonControl_OnClick(object sender, RoutedEventArgs e)
    {
        ((ButtonViewModel)ViewModel).OnClick();
    }

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseRightButtonDown(e);
        e.Handled = true;
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri(
            "/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptablebutton/buttoncontrol.xaml",
            UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            ((ButtonControl)target).Click += ButtonControl_OnClick;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
