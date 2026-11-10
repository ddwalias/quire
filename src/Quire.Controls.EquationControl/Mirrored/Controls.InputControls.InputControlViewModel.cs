using System.ComponentModel;
using Quire.Calculation.Core;

namespace Quire.Controls.InputControls;

public partial class InputControlViewModel
{
    public InputControlViewModel(IInputControlModel model)
    {
        _model = model;
        _model.PropertyChanged += HandleModelPropertyChanged;
    }

    public IInputControlModel Model => _model;

    public bool IsDirty
    {
        get => Model.IsDirty;
        set => Model.IsDirty = value;
    }

    public bool IsEnabled
    {
        get => Model.IsEnabled;
        set
        {
            Model.IsEnabled = value;
            OnPropertyChanged(nameof(IsEnabled));
        }
    }

    public bool IsInitializeComponent
    {
        get => Model.IsInitializeComponent;
        set => Model.IsInitializeComponent = value;
    }

    public bool IsFunctionalityEnabled => _model.IsFunctionalityEnabled;

    public bool HasKeyboard { get; set; }

    public CalculatableState State
    {
        get => Model.State;
        set
        {
            if (Model is IInputScriptableControlModelWithInputs scriptableModel)
            {
                scriptableModel.SetState(value, false);
            }
        }
    }

    public virtual double FontSize
    {
        get => Model.FontSize;
        set
        {
            Model.FontSize = value;
            OnPropertyChanged(nameof(FontSize));
        }
    }

    public virtual void InitializeComponent()
    {
    }

    protected virtual void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }
}
