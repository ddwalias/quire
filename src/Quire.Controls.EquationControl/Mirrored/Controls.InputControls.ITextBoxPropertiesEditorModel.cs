namespace Quire.Controls.InputControls;

public partial interface ITextBoxPropertiesEditorModel
{
    string Text { get; set; }
    bool AcceptsReturn { get; set; }
    bool IsReadOnly { get; set; }
    global::System.Windows.TextAlignment TextAlignment { get; set; }
}
