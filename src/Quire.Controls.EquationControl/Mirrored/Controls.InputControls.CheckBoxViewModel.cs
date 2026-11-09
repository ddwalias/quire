using System.Windows;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class CheckBoxViewModel
{
    private CheckBoxModel Model => (CheckBoxModel)_model;

    public bool IsChecked
    {
        get => Model.IsChecked;
        set
        {
            Model.IsChecked = value;
            OnPropertyChanged(nameof(IsChecked));
        }
    }

    public string ImagePath
    {
        get => Model.ImagePath;
        set
        {
            Model.ImagePath = value;
            OnPropertyChanged(nameof(ImagePath));
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

    public FlowDirection FlowDirection
    {
        get => Model.FlowDirection;
        set => Model.FlowDirection = value;
    }

    public CheckBoxViewModel(IInputControlDataCalculatedValue data)
        : base(new CheckBoxModel(data))
    {
    }

    protected override void HandleModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "IsChecked":
            case "Text":
            case "FlowDirection":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }
}
