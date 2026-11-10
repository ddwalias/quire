using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class ListBoxPropertiesEditor
{
    private readonly IInputControlEditorViewModel _viewModel;

    private bool _contentLoaded;

    public ListBoxPropertiesEditor(IInputControlEditorModel model)
    {
        _viewModel = new ListBoxEditorViewModel(model);
        DataContext = _viewModel;
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptablelistbox/listboxpropertieseditor.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
