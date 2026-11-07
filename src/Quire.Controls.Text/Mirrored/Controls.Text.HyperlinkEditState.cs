#nullable disable
#pragma warning disable

using System;

namespace Quire.Controls.Text;

public partial struct HyperlinkEditState
{
    public HyperlinkEditState(LinkEditOperation linkOperation, string inputText, bool userChangedText, Uri inputUri)
    {
        PerformedLinkEditOperation = linkOperation;
        UserChangedText = userChangedText;
        UserInputText = inputText;
        UserInputUri = inputUri;
    }

    public LinkEditOperation PerformedLinkEditOperation { get; }

    public string UserInputText { get; }

    public bool UserChangedText { get; }

    public Uri UserInputUri { get; }
}
