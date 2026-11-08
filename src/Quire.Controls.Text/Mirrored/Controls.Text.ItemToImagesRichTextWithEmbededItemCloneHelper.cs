#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class ItemToImagesRichTextWithEmbededItemCloneHelper
{
    private readonly IEngineeringDocument _engineeringDocument;
    private readonly ResourceDictionary _resourceDict;

    public CopyWidgetBase CreatePopup() => _engineeringDocument.CreateCopyWidget();

    public IWhiteboardItemInText Clone(IWhiteboardItemInText item, Rect? boundsConstraint, CopyWidgetBase popup)
    {
        IWhiteboardItemInText clone = _engineeringDocument.PrepareRegionCloneForImage(item, RegionImageSettings.DefaultForRtfCopy, popup, _resourceDict, boundsConstraint) as IWhiteboardItemInText;
        if (DebugTools.AssertNotNull(clone).Failed)
        {
            return null;
        }
        TextBlock.SetBaselineOffset((DependencyObject)clone, TextBlock.GetBaselineOffset((DependencyObject)item));
        return clone;
    }

    public InlineUIContainer Insert(IWhiteboardItemInText item, TextPointer insertionpoint)
    {
        FrameworkElement element = item as FrameworkElement;
        if (element == null)
        {
            return null;
        }
        Image image = new Image
        {
            Source = ImageUtils.CreateBitmapFromElementUsingScreenDpi(element, null, default(Rect?)),
        };
        InlineUIContainer container = new InlineUIContainer(image, insertionpoint);
        RichTextWithEmbeddedItem.InsertBaselineOffsetTag(element.ActualHeight - TextBlock.GetBaselineOffset(element), insertionpoint);
        return container;
    }
}
