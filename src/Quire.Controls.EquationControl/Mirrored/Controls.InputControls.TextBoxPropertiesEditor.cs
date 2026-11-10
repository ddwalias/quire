using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class TextBoxPropertiesEditor
{
    private bool _contentLoaded;

    public TextBoxPropertiesEditor(IInputControlEditorModel model)
    {
        DataContext = new TextBoxPropertiesEditorViewModel(model);
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptabletextbox/textboxpropertieseditor.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
