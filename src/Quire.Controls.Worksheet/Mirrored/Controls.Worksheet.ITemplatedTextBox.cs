namespace Quire.Controls.Worksheet;

public partial interface ITemplatedTextBox
{
    string TemplateFormat { get; set; }

    bool IsUnregisterWithCommand { get; set; }

    string Text { get; }
}
