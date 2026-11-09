using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class CheckBoxPropertiesEditor
{
    private bool _contentLoaded;

    private CheckBoxPropertiesEditorViewModel CheckBoxViewModel => (CheckBoxPropertiesEditorViewModel)DataContext;

    public CheckBoxPropertiesEditor(IInputControlEditorModel model)
    {
        DataContext = new CheckBoxPropertiesEditorViewModel(model);
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
            "/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptablecheckbox/checkboxpropertieseditor.xaml",
            UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
