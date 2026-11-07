#nullable disable
#pragma warning disable

using System.Windows.Documents;

namespace Quire.Controls.Text;

public partial class ModifyHyperlinkCommandParameter
{
    public ModifyHyperlinkCommandParameter(bool showDialog, string userInputText, string userInputAddress)
    {
        ShowDialog = showDialog;
        UserInputText = userInputText;
        UserInputAddress = userInputAddress;
    }

    public bool ShowDialog { get; }

    public string UserInputText { get; }

    public string UserInputAddress { get; }

    public Hyperlink ResultHyperlink { get; set; }

    public virtual bool ResultIsSingleLinkEditOperation { get; set; }
}
