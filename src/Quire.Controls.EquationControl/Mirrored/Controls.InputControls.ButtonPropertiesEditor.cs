using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class ButtonPropertiesEditor
{
    private bool _contentLoaded;

    public ButtonPropertiesEditor(IInputControlEditorModel model)
    {
        DataContext = new ButtonPropertiesEditorViewModel(model);
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
            "/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptablebutton/buttonpropertieseditor.xaml",
            UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
