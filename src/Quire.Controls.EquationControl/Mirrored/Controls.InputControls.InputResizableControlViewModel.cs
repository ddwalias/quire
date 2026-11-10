namespace Quire.Controls.InputControls;

public partial class InputResizableControlViewModel
{
    public InputResizableControlViewModel(IInputResizableControlModel model)
        : base(model)
    {
        OnPropertyChanged(nameof(Height));
        OnPropertyChanged(nameof(Width));
    }

    private IInputResizableControlModel Model => (IInputResizableControlModel)_model;

    public virtual double Height
    {
        get => Model.Height;
        set
        {
            if (Model.Height != value)
            {
                Model.Height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
    }

    public virtual double Width
    {
        get => Model.Width;
        set
        {
            Model.Width = value;
            OnPropertyChanged(nameof(Width));
        }
    }
}
