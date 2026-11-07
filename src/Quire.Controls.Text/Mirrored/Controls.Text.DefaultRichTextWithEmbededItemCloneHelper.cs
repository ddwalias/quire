#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Documents;
using Quire.Controls.Core;

namespace Quire.Controls.Text;

public partial class DefaultRichTextWithEmbededItemCloneHelper
{
    public CopyWidgetBase CreatePopup() => null;

    public IWhiteboardItemInText Clone(IWhiteboardItemInText item, Rect? boundsConstraint, CopyWidgetBase popup) =>
        item.Clone() as IWhiteboardItemInText;

    public InlineUIContainer Insert(IWhiteboardItemInText item, TextPointer insertionpoint) =>
        new InlineUIContainer(new WhiteboardItemInTextWrapper(item), insertionpoint);

    public DefaultRichTextWithEmbededItemCloneHelper()
    {
    }
}
