using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class TextBoxViewModel : InputScriptableControlViewModelWithInputs
{
    private PropertyInfo _backgroundColor;

    private new TextBoxModel Model => (TextBoxModel)_model;

    public TextBoxViewModel(IInputControlDataCalculatedValue data)
        : base(new TextBoxModel(data))
    {
        HasKeyboard = true;
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "TextAlignment":
            case "AcceptsReturn":
            case "IsReadOnly":
            case "Foreground":
            case "Background":
            case "FontSize":
            case "Text":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }

    public Brush Foreground
    {
        get => Model.Foreground;
        set
        {
            Model.Foreground = value;
            OnPropertyChanged("Foreground");
        }
    }

    public Brush Background
    {
        get => Model.Background;
        set
        {
            Model.Background = value;
            OnPropertyChanged("Background");
        }
    }

    public override double FontSize
    {
        get => Model.FontSize;
        set
        {
            Model.FontSize = value;
            OnPropertyChanged("FontSize");
        }
    }

    public bool IsReadOnly
    {
        get => Model.IsReadOnly;
        set
        {
            Model.IsReadOnly = value;
            OnPropertyChanged("IsReadOnly");
        }
    }

    public bool AcceptsReturn
    {
        get => Model.AcceptsReturn;
        set
        {
            Model.AcceptsReturn = value;
            OnPropertyChanged("AcceptsReturn");
        }
    }

    public string Text
    {
        get => Model.Text;
        set => Model.Text = value;
    }

    public TextAlignment TextAlignment
    {
        get => Model.TextAlignment;
        set
        {
            Model.TextAlignment = value;
            OnPropertyChanged("TextAlignment");
        }
    }

    public void OnLostFocus()
    {
        Model.OnLostFocus();
    }
}
