using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class SliderPropertiesEditor
{
    private bool _contentLoaded;

    public SliderPropertiesEditor(IInputControlEditorModel model)
    {
        DataContext = new SliderPropertiesEditorViewModel(model);
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptableslider/sliderpropertieseditor.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
