#nullable disable
#pragma warning disable

using System.ComponentModel;

namespace Quire.Controls.Text;

public partial class ContextMenuModifyHyperlinkCommandParameter
{
    public ContextMenuModifyHyperlinkCommandParameter() : base(true, null, null)
    {
    }

    public string GenericLabel { get; set; }

    public string SingleLinkEditLabel { get; set; }

    public override bool ResultIsSingleLinkEditOperation
    {
        get => base.ResultIsSingleLinkEditOperation;
        set
        {
            if (value == base.ResultIsSingleLinkEditOperation)
            {
                return;
            }
            base.ResultIsSingleLinkEditOperation = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentLabel"));
        }
    }

    public string CurrentLabel => ResultIsSingleLinkEditOperation ? SingleLinkEditLabel : GenericLabel;
}
