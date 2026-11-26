#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Net.Sgoliver.NRtfTree.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Worksheet;

public partial class RtfItemWrapperHelper
{
    public RtfItemModel CreateRtfItemModel(IRichTextWithEmbeddedItem textBox)
    {
        Rect flowDocumentRect = textBox.FlowDocumentRect;
        bool isRegionBorderVisible = WhiteboardDependencyProperties.GetIsRegionBorderVisible(textBox as DependencyObject);
        Brush borderColor = RegionBorder.GetRegionBorder(textBox as DependencyObject)?.BorderBrush;
        RtfItemModel rtfItemModel = new RtfItemModel(CreateText(textBox), flowDocumentRect.Size, textBox.Document.Background, isRegionBorderVisible, borderColor);
        Rect clippingRect = CalculateItemMeaningfulPartBounds(textBox, flowDocumentRect, flowDocumentRect.Size);
        bool autoWidth = textBox.WhiteboardItems.IsNotEmpty() && textBox.Lines.Skip(1).IsNullOrEmpty();
        rtfItemModel.Init(_worksheetPageManager.GetPageIndex(clippingRect.Y, roundUpIfNeeded: false), clippingRect, autoWidth);
        return rtfItemModel;
    }

    public RtfTree PrepareRtf(IEnumerable<IRtfItemBase> items, bool isSavingWholeDocument)
    {
        Size pageSize = new Size(_worksheetPageManager.PageLayout.PageWidth, _worksheetPageManager.PageLayout.PageHeight);
        return RtfDocConstructor.PrepareRtf(items, pageSize, ItemLocationStrategy.GetPageBodySize, isSavingWholeDocument);
    }

    public RtfTree CreateText(IRichTextWithEmbeddedItem richText)
    {
        return RtfHelper.TextboxToRtf(richText.WhiteboardItems.Any() ? richText.CloneRichText(_engineeringDocument) : richText, richText.WhiteboardItems.Count);
    }
}
