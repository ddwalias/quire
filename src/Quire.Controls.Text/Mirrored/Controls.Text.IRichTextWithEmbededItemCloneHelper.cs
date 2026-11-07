#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Documents;
using Quire.Controls.Core;

namespace Quire.Controls.Text;

public partial interface IRichTextWithEmbededItemCloneHelper
{
    CopyWidgetBase CreatePopup();

    IWhiteboardItemInText Clone(IWhiteboardItemInText item, Rect? boundsConstraint, CopyWidgetBase popup);

    InlineUIContainer Insert(IWhiteboardItemInText item, TextPointer insertionpoint);
}
