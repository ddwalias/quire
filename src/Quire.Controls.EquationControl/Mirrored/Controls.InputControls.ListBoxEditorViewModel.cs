using System.ComponentModel;

namespace Quire.Controls.InputControls;

internal partial class ListBoxEditorViewModel
{
    public ListBoxEditorViewModel(IInputControlEditorModel model)
        : base(model)
    {
    }

    private IListBoxPropertiesEditorModel Model => (IListBoxPropertiesEditorModel)_model;

    public bool IsSort
    {
        get => Model.IsSort;
        set => Model.IsSort = value;
    }

    public bool RightToLeft
    {
        get => Model.FlowDirection == System.Windows.FlowDirection.RightToLeft;
        set
        {
            if (value)
            {
                Model.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            }
            else
            {
                Model.FlowDirection = System.Windows.FlowDirection.LeftToRight;
            }

            OnPropertyChanged(nameof(RightToLeft));
        }
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "FlowDirection":
                OnPropertyChanged(nameof(RightToLeft));
                break;
            case "IsSort":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }
}
