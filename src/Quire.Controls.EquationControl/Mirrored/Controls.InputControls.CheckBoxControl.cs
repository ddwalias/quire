using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class CheckBoxControl
{
    private bool _contentLoaded;

    public CheckBoxControl(IInputControlDataCalculatedValue data)
    {
        DataContext = new CheckBoxViewModel(data);
        InitializeComponent();
    }

    public IInputControlViewModel ViewModel => (CheckBoxViewModel)DataContext;

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
            "/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptablecheckbox/checkboxcontrol.xaml",
            UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
