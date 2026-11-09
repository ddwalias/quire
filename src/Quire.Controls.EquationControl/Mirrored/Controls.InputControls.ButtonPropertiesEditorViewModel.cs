using System.ComponentModel;

namespace Quire.Controls.InputControls;

internal partial class ButtonPropertiesEditorViewModel
{
    private IButtonPropertiesEditorModel Model => (IButtonPropertiesEditorModel)_model;

    public string Text
    {
        get => Model.Text;
        set
        {
            Model.Text = value;
            OnPropertyChanged(nameof(Text));
        }
    }

    public ButtonPropertiesEditorViewModel(IInputControlEditorModel model)
        : base(model)
    {
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Text))
        {
            OnPropertyChanged(e.PropertyName);
        }
    }
}
