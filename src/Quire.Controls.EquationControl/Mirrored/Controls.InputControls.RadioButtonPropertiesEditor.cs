using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class RadioButtonPropertiesEditor : UserControl, IComponentConnector
{
    private bool _contentLoaded;

    private RadioButtonPropertiesEditorViewModel RadioButtonViewModel => (RadioButtonPropertiesEditorViewModel)DataContext;

    public RadioButtonPropertiesEditor(IInputControlEditorModel controlModel)
    {
        DataContext = new RadioButtonPropertiesEditorViewModel(controlModel);
        InitializeComponent();
    }

    private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        RadioButtonViewModel.HandleButtonsChanged();
    }

    private void DataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Key.Delete == e.Key)
        {
            DataGrid_OnRowEditEnding(sender, null);
        }
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptableradiobutton/radiobuttonpropertieseditor.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            ((DataGrid)target).PreviewKeyDown += DataGrid_OnPreviewKeyDown;
            ((DataGrid)target).RowEditEnding += DataGrid_OnRowEditEnding;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
