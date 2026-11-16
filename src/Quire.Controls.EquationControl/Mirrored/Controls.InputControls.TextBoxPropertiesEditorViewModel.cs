using System.ComponentModel;
using System.Windows;

namespace Quire.Controls.InputControls;

internal partial class TextBoxPropertiesEditorViewModel
{
    private ITextBoxPropertiesEditorModel Model => (ITextBoxPropertiesEditorModel)_model;

    public TextBoxPropertiesEditorViewModel(IInputControlEditorModel model)
        : base(model)
    {
        _model = model;
    }

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "AcceptsReturn":
            case "IsReadOnly":
            case "TextAlignment":
            case "Text":
                OnPropertyChanged(e.PropertyName);
                break;
        }
    }

    public string Text
    {
        get => Model.Text;
        set
        {
            Model.Text = value;
            OnPropertyChanged("Text");
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

    public bool IsReadOnly
    {
        get => Model.IsReadOnly;
        set
        {
            Model.IsReadOnly = value;
            OnPropertyChanged("IsReadOnly");
        }
    }

    public TextAlignmentOptions TextAlignment
    {
        get => (TextAlignmentOptions)Model.TextAlignment;
        set
        {
            Model.TextAlignment = (TextAlignment)value;
            OnPropertyChanged("TextAlignment");
        }
    }
}
