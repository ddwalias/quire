using System.Windows;

namespace Quire.Controls.InputControls;

internal partial class CheckBoxPropertiesEditorViewModel
{
    private ICheckBoxPropertiesEditorModel Model => (ICheckBoxPropertiesEditorModel)_model;

    public bool RightToLeft
    {
        get => Model.FlowDirection == FlowDirection.RightToLeft;
        set
        {
            if (value)
            {
                Model.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                Model.FlowDirection = FlowDirection.LeftToRight;
            }

            OnPropertyChanged(nameof(RightToLeft));
        }
    }

    public string Text
    {
        get => Model.Text;
        set
        {
            Model.Text = value;
            OnPropertyChanged(nameof(Text));
        }
    }

    public CheckBoxPropertiesEditorViewModel(IInputControlEditorModel model)
        : base(model)
    {
    }

    protected override void HandleModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "FlowDirection":
                OnPropertyChanged(nameof(RightToLeft));
                break;
            case "RightToLeft":
            case "Text":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }
}
