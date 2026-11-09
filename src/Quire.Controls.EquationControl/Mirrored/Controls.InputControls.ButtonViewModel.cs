using System.ComponentModel;
using System.Reflection;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class ButtonViewModel
{
#pragma warning disable CS0169 // Kept so the view model's field layout matches the original assembly.
    private bool _isChecked;
#pragma warning restore CS0169

    private PropertyInfo _backgroundColor;

    private ButtonModel Model => (ButtonModel)_model;

    public PropertyInfo BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            OnPropertyChanged(nameof(BackgroundColor));
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

    public bool IsChecked
    {
        get => Model.IsChecked;
        set => Model.IsChecked = value;
    }

    public ButtonViewModel(IInputControlDataCalculatedValue data)
        : base(new ButtonModel(data))
    {
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "Text":
            case "IsChecked":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }

    internal void OnClick()
    {
        Model.IsChecked = !Model.IsChecked;
    }
}
